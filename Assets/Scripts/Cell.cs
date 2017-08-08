using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    [HideInInspector]
    public Tile tile;
    [HideInInspector]
    public int residentialCapacity;
    [HideInInspector]
    public int imaginaryResidentialCapacity;
    [HideInInspector]
    public int industrialCapacity;
    [HideInInspector]
    public List<Cell> roadConnections;
    [HideInInspector]
    public CellData cellData;
    [HideInInspector]
    public List<Cell> adjacent;
    [HideInInspector]
    public List<Cell> occupiedDirections;

    private void Awake() {
        roadConnections = new List<Cell>();
        adjacent = new List<Cell>();
        occupiedDirections = new List<Cell>();
    }

    public void Initialize(CellData cellData) {
        this.cellData = cellData;
    }

    public void CalculateAdjacent() {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(tile.transform.position, HexMetrics.OuterRadius, 1 << 9)) {
            if (col.gameObject.GetInstanceID() != tile.gameObject.GetInstanceID()) {
                adjacent.Add(col.GetComponent<Tile>().cell);
            }
        }
    }
}
