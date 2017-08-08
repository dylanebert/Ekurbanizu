using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {

    public LineRenderer line;
    public SpriteRenderer a;
    public SpriteRenderer b;
    public CapsuleCollider2D col;

    [HideInInspector]
    public bool active;

    Cell cellA;
    Cell cellB;
    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        SetColor(Palette.EmptyTileHighlighted);
    }

    public void Initialize(Cell cellA, Cell cellB) {
        this.cellA = cellA;
        this.cellB = cellB;
    }

    public void SetColor(Color c) {
        line.startColor = line.endColor = c;
    }

    public void SetPositions(Vector3 a, Vector3 b) {
        this.a.transform.position = a + Vector3.back;
        this.b.transform.position = b + Vector3.back;
        line.SetPositions(new Vector3[] { a, b });
        Vector3 dif = b - a;
        col.size = new Vector2(line.widthMultiplier * 5f, dif.magnitude * .75f);
        col.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90);
        col.transform.position = a + dif / 2f;
    }

    public void MouseOver() {
        if(!active)
            SetColor(Palette.OffBlack);
        else if(gameController.lens == Lens.Erase) {
            SetColor(Color.Lerp(Palette.OffBlack, Palette.Gray, .5f));
        }
    }

    public void MouseExit() {
        if(!active)
            SetColor(Palette.EmptyTileHighlighted);
        else if(gameController.lens == Lens.Erase) {
            SetColor(Palette.OffBlack);
        }
    }

    public void MouseUp() {
        if (!active) {
            SetColor(Palette.OffBlack);
            active = true;
            cellA.roadConnections.Add(cellB);
            cellB.roadConnections.Add(cellA);
            gameController.AddRoad();
        } else if(gameController.lens == Lens.Erase) {
            cellA.roadConnections.Remove(cellB);
            cellB.roadConnections.Remove(cellA);
            active = false;
            SetColor(Palette.EmptyTileHighlighted);
            gameController.RemoveRoad();
            this.gameObject.SetActive(false);
        }
    }
}
