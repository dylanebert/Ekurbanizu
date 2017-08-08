using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFly : MonoBehaviour {

    [HideInInspector]
    public bool animating;

    GameController gameController;
    float xBound;
    float yBound;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        xBound = HexMetrics.OuterRadius * gameController.grid.gridData.mapSize.x + HexMetrics.OuterRadius;
        yBound = HexMetrics.OuterRadius * gameController.grid.gridData.mapSize.y + HexMetrics.OuterRadius;
        StartCoroutine(FlyIn());
    }

    public IEnumerator FlyIn() {
        animating = true;
        Quaternion rotation = transform.rotation = Quaternion.Euler(0, 0, 45);
        Vector3 origin = transform.position = new Vector3(-xBound - HexMetrics.OuterRadius * 5, yBound + HexMetrics.OuterRadius * 5, 0);
        Vector3 target = Vector3.back * 10f;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            transform.position = Vector3.Lerp(origin, target, v);
            transform.rotation = Quaternion.Lerp(rotation, Quaternion.identity, v);
            Camera.main.orthographicSize = Mathf.Lerp(1, xBound * .75f, v);
            yield return null;
        }
        animating = false;
    }

    public IEnumerator FlyToMenu() {
        animating = true;
        Quaternion rotation = Quaternion.Euler(0, 0, 45);
        Vector3 origin = transform.position;
        Vector3 target = new Vector3(-xBound - HexMetrics.OuterRadius * 5, yBound + HexMetrics.OuterRadius * 5, 0);
        float zoom = Camera.main.orthographicSize;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float v = Mathf.Sin(Mathf.PI * t / 2f + Mathf.PI / 2f);
            transform.position = Vector3.Lerp(target, origin, v);
            transform.rotation = Quaternion.Lerp(rotation, Quaternion.identity, v);
            Camera.main.orthographicSize = Mathf.Lerp(1, zoom, v);
            yield return null;
        }
        animating = false;
        GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>().LoadLevelSelect();
    }

    public IEnumerator FlyToOverview() {
        float zoom = Camera.main.orthographicSize;
        Vector3 origin = Camera.main.transform.position;
        Vector3 target = new Vector3(0, yBound / 4f, -10f);
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            Camera.main.orthographicSize = Mathf.Lerp(zoom, xBound * 1.5f, v);
            Camera.main.transform.position = Vector3.Lerp(origin, target, v);
            yield return null;
        }
    }

    public IEnumerator ReturnToMap() {
        float zoom = Camera.main.orthographicSize;
        Vector3 origin = Vector3.back * 10f;
        Vector3 target = Camera.main.transform.position;
        float t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * (1 - t) / 2f);
            Camera.main.orthographicSize = Mathf.Lerp(xBound * .75f, zoom, v);
            Camera.main.transform.position = Vector3.Lerp(origin, target, v);
            yield return null;
        }
    }
}
