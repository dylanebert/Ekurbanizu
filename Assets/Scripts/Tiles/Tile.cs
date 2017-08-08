using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public SpriteRenderer glow;
    public string typename;

    [HideInInspector]
    public Cell cell;
    [HideInInspector]
    public int presentCount;

    protected Dictionary<Vector2, Resident> residentPositions;
    protected SpriteRenderer sprite;
    protected GameController gameController;
    protected Color baseColor = Palette.EmptyTile;
    protected Color highlightedColor = Palette.EmptyTile;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        residentPositions = new Dictionary<Vector2, Resident>();
        foreach(Vector2 pos in HexMetrics.ResidentPositions) {
            residentPositions.Add(pos, null);
        }
    }

    public void Initialize(Cell cell) {
        this.cell = cell;
        cell.tile = this;

        Vector2 position;
        position.x = (cell.cellData.coords.x + Mathf.Abs(cell.cellData.coords.y) % 2 / 2f - .5f) * HexMetrics.InnerRadius * 2f;
        position.y = (cell.cellData.coords.y) * HexMetrics.OuterRadius * 1.5f;
        transform.localPosition = position;
    }

    public Vector2 FillPosition(Resident resident) {
        foreach (Vector2 pos in residentPositions.Keys) {
            if (residentPositions[pos] == null) {
                residentPositions[pos] = resident;
                return transform.TransformPoint(pos);
            }
        }
        return transform.position;
    }

    public virtual bool Arrive(Resident resident) {
        return true;
    }

    public virtual void Leave(Resident resident) {

    }

    public void VacatePosition(Vector2 pos) {
        residentPositions[transform.InverseTransformPoint(pos)] = null;
    }

    public void IncreaseResidentialCapacity() {
        if(cell.imaginaryResidentialCapacity < 1) {
            cell.imaginaryResidentialCapacity++;
        } else {
            cell.residentialCapacity++;
        }
    }

    public void DecreaseResidentialCapacity() {
        if(cell.residentialCapacity > 1) {
            cell.residentialCapacity--;
        } else {
            cell.imaginaryResidentialCapacity--;
        }
    }

    public void IncreaseIndustrialCapacity() {
        cell.industrialCapacity++;
    }

    public void DecreaseIndustrialCapacity() {
        cell.industrialCapacity--;
    }

    public virtual void UpdateColor() {
        SetColor(baseColor);
    }

    public void SetColor(Color col) {
        if(sprite != null)
            sprite.color = col;
    }

    public virtual void MouseOver() {
        if (!Input.GetMouseButton(0))
            SetColor(highlightedColor);
    }

    public virtual void MouseExit() {
        SetColor(baseColor);
    }

    public virtual void MouseDown() {
        
    }

    public virtual void MouseUp() {

    }
}