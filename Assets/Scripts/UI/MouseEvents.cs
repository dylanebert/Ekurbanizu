using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvents : MonoBehaviour {

    List<Tile> mouseOverTiles;
    Road mouseOverRoad;
    GameController gameController;
    Vector3 mouseDownPos;
    float touchBeganTime;
    int mouseDownTicks;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        mouseOverTiles = new List<Tile>();
    }

    private void Update() {
        if (gameController.activeWindow != null) return;

        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                Vector2 touchPos = Input.GetTouch(0).position;
                Ray touchRay = Camera.main.ScreenPointToRay(touchPos);
                RaycastHit2D touchHit = Physics2D.Raycast(touchRay.origin, touchRay.direction, 20f);
                if (touchHit.collider != null) {
                    switch (touchHit.transform.gameObject.layer) {
                        case 9:
                            touchHit.transform.GetComponent<Tile>().MouseUp();
                            break;
                        case 10:
                            touchHit.transform.parent.GetComponent<Road>().MouseUp();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D roadHit = Physics2D.Raycast(ray.origin, ray.direction, 20f, 1 << 10);
            if (roadHit.collider != null) {
                if (mouseOverRoad != null)
                    mouseOverRoad.MouseExit();
                mouseOverRoad = roadHit.transform.parent.GetComponent<Road>();
                mouseOverRoad.MouseOver();
            }
            else {
                if (mouseOverRoad != null) {
                    mouseOverRoad.MouseExit();
                    mouseOverRoad = null;
                }
            }

            if (gameController.lens == Lens.Erase && mouseOverRoad != null) {
                foreach (Tile tile in mouseOverTiles) {
                    tile.MouseExit();
                }
                mouseOverTiles.Clear();
            }
            else {
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 20f, 1 << 9);
                List<Tile> hitTiles = new List<Tile>();
                foreach (RaycastHit2D hit in hits) {
                    hitTiles.Add(hit.transform.GetComponent<Tile>());
                }

                int i = 0;
                while (i < mouseOverTiles.Count) {
                    if (!hitTiles.Contains(mouseOverTiles[i])) {
                        mouseOverTiles[i].MouseExit();
                        mouseOverTiles.Remove(mouseOverTiles[i]);
                    }
                    else {
                        hitTiles.Remove(mouseOverTiles[i]);
                        i++;
                    }
                }

                foreach (Tile tile in hitTiles) {
                    mouseOverTiles.Add(tile);
                    tile.MouseOver();
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                mouseDownPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                foreach (Tile tile in mouseOverTiles) {
                    tile.MouseDown();
                }
            }

            if (Input.GetMouseButton(0))
                mouseDownTicks++;


            if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                if ((Camera.main.ScreenToViewportPoint(Input.mousePosition) - mouseDownPos).magnitude < .02f) {
                    if (mouseOverTiles.Count == 0 && mouseOverRoad == null && mouseDownTicks < 100) {
                        gameController.DeselectPointerButtons();
                    }
                    else {
                        if (mouseOverRoad != null)
                            mouseOverRoad.MouseUp();
                        if (mouseOverRoad == null || gameController.lens != Lens.Erase) {
                            foreach (Tile tile in mouseOverTiles) {
                                tile.MouseUp();
                            }
                        }
                    }
                    mouseDownTicks = 0;
                }
            }
        }
    }
}
