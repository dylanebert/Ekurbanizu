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

        Vector2 position;
        position.x = (cellData.coords.x + Mathf.Abs(cellData.coords.y) % 2 / 2f - .5f) * HexMetrics.InnerRadius * 2f;
        position.y = (cellData.coords.y) * HexMetrics.OuterRadius * 1.5f;
        transform.localPosition = position;
    }

    public void Initialize(Vector2 coords) {
        Vector2 position;
        position.x = (coords.x + Mathf.Abs(coords.y) % 2 / 2f - .5f) * HexMetrics.InnerRadius * 2f;
        position.y = (coords.y) * HexMetrics.OuterRadius * 1.5f;
        transform.localPosition = position;
    }

    public void CalculateAdjacent() {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, HexMetrics.OuterRadius, 1 << 12)) {
            if (tile == null || col.gameObject.GetInstanceID() != tile.gameObject.GetInstanceID()) {
                adjacent.Add(col.GetComponent<Cell>());
            }
        }
    }
}
