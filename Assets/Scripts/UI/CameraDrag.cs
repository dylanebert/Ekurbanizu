using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour {

    public int minZoom = 1;
    public int maxZoom = 10;
    public float panSpeed = .5f;
    public float zoomSpeed = 5f;

    GameController gameController;
    Vector3 prevMousePosition;
    float xBound;
    float yBound;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        xBound = HexMetrics.OuterRadius * gameController.grid.gridData.mapSize.x + HexMetrics.OuterRadius;
        yBound = HexMetrics.OuterRadius * gameController.grid.gridData.mapSize.y + HexMetrics.OuterRadius;
    }

    private void Update() {
        if (gameController.pauseMenuShown || gameController.winScreenShown) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }

        if (Input.GetMouseButton(0)) {
            Vector2 move = Vector2.zero;
            move.x = -Input.GetAxis("Mouse X") * panSpeed * (1080f / Screen.height) * Mathf.Sqrt(10f * Camera.main.orthographicSize) / 5f;
            move.y = -Input.GetAxis("Mouse Y") * panSpeed * (1080f / Screen.height) * Mathf.Sqrt(10f * Camera.main.orthographicSize) / 5f;
            transform.Translate(move);
        }
        else {
            if (transform.position.x < -xBound) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-xBound, transform.position.y, transform.position.z), Time.deltaTime * Mathf.Abs(transform.position.x + xBound) * 5f);
            }
            if (transform.position.x > xBound) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(xBound, transform.position.y, transform.position.z), Time.deltaTime * Mathf.Abs(transform.position.x - xBound) * 5f);
            }
            if(transform.position.y < -yBound) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -yBound, transform.position.z), Time.deltaTime * Mathf.Abs(transform.position.y + yBound) * 5f);
            }
            if (transform.position.y > yBound) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yBound, transform.position.z), Time.deltaTime * Mathf.Abs(transform.position.y - yBound) * 5f);
            }
        }
    }
}
