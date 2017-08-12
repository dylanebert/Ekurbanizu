using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Priority_Queue;

public class Grid : MonoBehaviour {

    public static int maxPathCacheAge = 1000; //in iterations

    public Tile emptyTileObj;
    public GameObject roadObj;
    public GameObject cellObj;

    [HideInInspector]
    public List<Cell> cells;
    [HideInInspector]
    public GridData gridData;
    [HideInInspector]
    public int roadsAvailable;
    [HideInInspector]
    public List<Empty> emptyTiles;
    [HideInInspector]
    public List<Residential> residentialTiles;
    [HideInInspector]
    public List<Industrial> industrialTiles;
    [HideInInspector]
    public Stack<Resident> residents;

    GameController gameController;    
    List<Road> roads;
    Dictionary<Cell, List<Path>> pathCache;

    int residentialCapacity;
    int industrialCapacity;
    bool calculatingCommute;

    public void Initialize(GridData gridData, GameController gameController) {
        this.gridData = gridData;
        this.gameController = gameController;

        if(gridData.mapSize.x % 2 == 0) {
            transform.Translate(Vector3.left * HexMetrics.OuterRadius);
        }
        if(gridData.mapSize.y % 2 == 0) {
            transform.Translate(Vector3.down * HexMetrics.OuterRadius);
        }

        cells = new List<Cell>();
        residentialTiles = new List<Residential>();
        industrialTiles = new List<Industrial>();
        pathCache = new Dictionary<Cell, List<Path>>();

        float offsetX = (-(gridData.mapSize.x / 2f) + 1 + (gridData.mapSize.x % 2 == 0 ? .5f : 0)) * HexMetrics.InnerRadius * 2f;
        float offsetY = (-(gridData.mapSize.y / 2f) + .5f) * HexMetrics.InnerRadius * 2f;
        transform.position = new Vector2(offsetX, offsetY);
        foreach(CellData cellData in gridData.cellData) {
            if (!cellData.enabled) continue;
            Cell cell = Instantiate(cellObj, this.transform).GetComponent<Cell>();
            cell.Initialize(cellData);
            Tile tile = Instantiate(emptyTileObj, cell.transform).GetComponent<Tile>();
            tile.Initialize(cell);
            emptyTiles.Add((Empty)tile);
            cell.residentialCapacity = cell.cellData.baseResidentialCapacity;
            cell.industrialCapacity = cell.cellData.baseIndustrialCapacity;
            cell.imaginaryResidentialCapacity = 1;
            cell.roadConnections = new List<Cell>();
            cells.Add(cell);
        }

        roads = new List<Road>();
        List<CellTuple> visited = new List<CellTuple>();
        foreach (Cell cell in cells) {
            cell.CalculateAdjacent();
            foreach (Tile adj in cell.adjacent.Select(t => t.tile).ToList()) {
                CellTuple tuple = new CellTuple(cell, adj.cell);
                if (!visited.Contains(tuple)) {
                    Road road = Instantiate(roadObj, transform).GetComponent<Road>();
                    road.SetPositions(cell.tile.transform.position + Vector3.back * 3, adj.transform.position + Vector3.back * 3);
                    road.Initialize(cell, adj.cell);
                    roads.Add(road);
                    visited.Add(tuple);
                    visited.Add(tuple.Complement());
                }
            }
        }

        roadsAvailable = gridData.roadCount;

        StartCoroutine(ContinousCalculateCommutes());
    }

    public int GetMaxResidentialCapacity() {
        int max = 1;

        foreach(Cell cell in cells) {
            if(cell.cellData.baseResidentialCapacity > max) {
                max = cell.cellData.baseResidentialCapacity;
            }
        }

        return max + 6;
    }

    public int GetMaxIndustrialCapacity() {
        int max = 1;

        foreach(Cell cell in cells) {
            if(cell.cellData.baseIndustrialCapacity > max) {
                max = cell.cellData.baseIndustrialCapacity;
            }
        }

        return max + 6;
    }

    public void ShowRoads(bool show) {
        foreach(Road road in roads) {
            if (!road.active) {
                road.gameObject.SetActive(show);
            }
        }
    }
    
    public void ReplaceTile(GameObject newTileObj, Tile original) {
        Tile tile = Instantiate(newTileObj, this.transform).GetComponent<Tile>();
        tile.Initialize(original.cell);
        tile.UpdateColor();
        switch(tile.typename) {
            case "Residential":
                if (residentialTiles.Count == 0) {
                    gameController.firstResidentialPlaced = true;
                    gameController.SetGameSpeed(1);
                }
                emptyTiles.Remove((Empty)original);
                residentialTiles.Add((Residential)tile);
                break;
            case "Industrial":
                emptyTiles.Remove((Empty)original);
                industrialTiles.Add((Industrial)tile);
                break;
            case "Empty":
                switch(original.typename) {
                    case "Residential":
                        foreach (Cell adj in original.cell.adjacent)
                            adj.tile.DecreaseResidentialCapacity();
                        Stack<Resident> residentStack = new Stack<Resident>(((Residential)original).residents);
                        while (residentStack.Count > 0) {
                            Resident resident = residentStack.Pop();
                            StartCoroutine(resident.Destroy());
                        }
                        residentialTiles.Remove((Residential)original);
                        emptyTiles.Add((Empty)tile);
                        if (industrialTiles.Count == 0 && residentialTiles.Count == 0) {
                            gameController.DeselectPointerButtons();
                        }
                        break;
                    case "Industrial":
                        foreach (Cell adj in original.cell.adjacent)
                            adj.tile.DecreaseIndustrialCapacity();
                        Stack<Resident> workerStack = new Stack<Resident>(((Industrial)original).workers);
                        while (workerStack.Count > 0) {
                            Resident resident = workerStack.Pop();
                            if (resident.status == Resident.Status.CommuteToWork || resident.status == Resident.Status.Work) {
                                StartCoroutine(resident.Destroy());
                            } else {
                                ((Industrial)original).RemoveWorker(resident);
                            }
                        }
                        industrialTiles.Remove((Industrial)original);
                        emptyTiles.Add((Empty)tile);
                        if(industrialTiles.Count == 0 && residentialTiles.Count == 0) {
                            gameController.DeselectPointerButtons();
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        if (emptyTiles.Count == 0)
            gameController.DeselectPointerButtons();
        Destroy(original.gameObject);
    }

    IEnumerator ContinousCalculateCommutes() {
        float prevTime = Time.time;
        while(true) {
            residents = new Stack<Resident>(gameController.residents);
            while(residents.Count > 0) {
                /*Debug.Log((Time.time - prevTime).ToString("f2"));
                prevTime = Time.time;*/
                Resident resident = residents.Pop();
                if (resident.commute != null && (resident.status == Resident.Status.CommuteToWork || resident.status == Resident.Status.CommuteHome))
                    continue;
                else {
                    if (resident.job != null) {
                        StartCoroutine(CalculateCommute(resident, resident.home.cell, resident.job.cell));
                        while (calculatingCommute) yield return null;
                    }
                    else {
                        StartCoroutine(CalculateCommute(resident, resident.home.cell));
                        while (calculatingCommute) yield return null;
                        if (resident.commute != null) {
                            resident.job = (Industrial)resident.commute.cells.Last().tile;
                            resident.job.AddWorker(resident);
                        }
                    }
                }
                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator CalculateCommute(Resident resident, Cell origin, Cell destination = null) {
        calculatingCommute = true;
        float maxPathCost = 48f;
        int iterations = 0;
        if(destination != null) {
            if (pathCache.ContainsKey(origin)) {
                Path path = pathCache[origin].Find(p => p.cells.Last().tile == destination);
                if (path != null) {
                    if (++path.age > maxPathCacheAge)
                        pathCache[origin].Remove(path);
                    resident.commute = path;
                    calculatingCommute = false;
                    yield break;
                }
            }

            SimplePriorityQueue<Path> queue = new SimplePriorityQueue<Path>();
            queue.Enqueue(new Path(new List<Cell> { origin }), 0);
            while (queue.Count > 0) {
                if(++iterations > 10) {
                    iterations = 0;
                    //Debug.Log(queue.Count);
                    yield return null;
                }
                Path path = queue.Dequeue();
                Tile last = path.cells.Last().tile;

                if (destination == last.cell) {
                    if (!pathCache.ContainsKey(origin))
                        pathCache.Add(origin, new List<Path> { path });
                    else
                        pathCache[origin].Add(path);
                    resident.commute = path;
                    calculatingCommute = false;
                    yield break;
                }

                float distFromDest = (last.transform.position - destination.tile.transform.position).magnitude;
                foreach (Tile t in last.cell.adjacent.Select(t => t.tile).ToList()) {
                    if (!path.cells.Any(c => c.tile == t)) {
                        if((t.transform.position - destination.tile.transform.position).magnitude < distFromDest) {
                            int cost = last.cell.roadConnections.Contains(t.cell) ? Mathf.RoundToInt(8f / Resident.DriveSpeed) : Mathf.RoundToInt(8f / Resident.WalkSpeed);
                            if (path.cost + cost <= maxPathCost) {
                                Path p = path.Add(t.cell, cost);
                                queue.Enqueue(p, p.cost);
                            }
                        }
                    }
                }
            }
        } else {
            if(gameController.workers.Count >= industrialCapacity) {
                //Debug.Log("Capacity met");
                resident.commute = null;
                calculatingCommute = false;
                yield break;
            }

            
            SimplePriorityQueue<Path> queue = new SimplePriorityQueue<Path>();
            queue.Enqueue(new Path(new List<Cell> { origin }), 0);
            while (queue.Count > 0) {
                if (++iterations > 10) {
                    iterations = 0;
                    //Debug.Log(queue.Count);
                    yield return null;
                }
                Path path = queue.Dequeue();
                Tile last = path.cells.Last().tile;

                if (last.typename == "Industrial") {
                    if(((Industrial)last).workers.Count < last.cell.industrialCapacity) {
                        if (!pathCache.ContainsKey(origin))
                            pathCache.Add(origin, new List<Path> { path });
                        else
                            pathCache[origin].Add(path);
                        resident.commute = path;
                        calculatingCommute = false;
                        yield break;
                    }
                }

                float distanceFromOrigin = (last.transform.position - origin.tile.transform.position).magnitude;
                foreach (Tile t in last.cell.adjacent.Select(t => t.tile).ToList()) {
                    if (!path.cells.Any(c => c.tile == t)) {
                        if ((t.transform.position - origin.tile.transform.position).magnitude > distanceFromOrigin) {
                            int cost = last.cell.roadConnections.Contains(t.cell) ? Mathf.RoundToInt(8f / Resident.DriveSpeed) : Mathf.RoundToInt(8f / Resident.WalkSpeed);
                            if (path.cost + cost <= maxPathCost) {
                                Path p = path.Add(t.cell, cost);
                                queue.Enqueue(p, p.cost);
                            }
                        }
                    }
                }
            }
        }

        //Debug.Log("No Route found");
        resident.commute = null;
        calculatingCommute = false;
    }

    public int GetResidentialCapacity() {
        int c = 0;
        foreach(Residential t in residentialTiles) {
            c += t.cell.residentialCapacity;
        }
        residentialCapacity = c;
        return c;
    }

    public int GetIndustrialCapacity() {
        int c = 0;
        foreach (Industrial t in industrialTiles) {
            c += t.cell.industrialCapacity;
        }
        industrialCapacity = c;
        return c;
    }
}

public class Path {
    public List<Cell> cells;
    public int cost;
    public int age;

    public Path() {
        cells = new List<Cell>();
    }

    public Path(List<Cell> cells) {
        this.cells = new List<Cell>(cells);
    }

    public Path Add(Cell cell, int cost) {
        Path path = new Path(cells);
        path.cells.Add(cell);
        path.cost = this.cost + cost;
        return path;
    }

    public override string ToString() {
        string s = "";
        foreach (Cell cell in cells) {
            s += cell.tile.typename + ", ";
        }
        s += cost;
        return s;
    }
}

public struct CellTuple {
    public Cell a;
    public Cell b;

    public CellTuple(Cell a, Cell b) {
        this.a = a;
        this.b = b;
    }

    public CellTuple Complement() {
        return new CellTuple(b, a);
    }
}
