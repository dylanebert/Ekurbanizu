using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resident : MonoBehaviour {

    public static float WalkSpeed = HexMetrics.OuterRadius / 3f;
    public static float DriveSpeedMultiplier = 4f;

    [HideInInspector]
    public Vector2 homePos;
    [HideInInspector]
    public Vector2 workPos;
    [HideInInspector]
    public bool atHome;
    [HideInInspector]
    public bool atWork;
    [HideInInspector]
    public Status status;
    [HideInInspector]
    public Status goal;
    [HideInInspector]
    public Path commute;
    [HideInInspector]
    public Residential home;
    [HideInInspector]
    public Industrial job;

    GameController gameController;
    Cell currentCell;
    Cell nextCell;
    bool moving;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        StartCoroutine(CreationAnimation());
    }

    private void Update() {
        if (status == Status.Destroy) return;
        switch(goal) {
            case Status.Home:
                if(status != Status.Home) {
                    if(status != Status.CommuteHome) {
                        StartCoroutine(Commute(true));
                    }
                }
                break;
            case Status.Work:
                if(status != Status.Work) {
                    if(status != Status.CommuteToWork) {
                        StartCoroutine(Commute());
                    }
                }
                break;
            default:
                break;
        }
    }

    public void SetGoal(Status goal) {
        this.goal = goal;
    }

    public void AssertWork() {
        if (status != Status.Work) {
            StartCoroutine(Destroy());
        }
    }

    public void AssertHome() {
        if(status != Status.Home) {
            StartCoroutine(Destroy());
        }
    }

    public void SetHome(Residential home) {
        this.home = home;
    }

    IEnumerator Commute(bool toHome = false) {
        status = toHome ? Status.CommuteHome : Status.CommuteToWork;

        while (commute == null || job == null) {
            yield return new WaitForSeconds(1f);
        }

        Vector2 dest = toHome ? job.transform.position : home.transform.position;
        while((Vector2)transform.position != dest) {
            transform.position = Vector2.MoveTowards((Vector2)transform.position, dest, Time.deltaTime * WalkSpeed);
            yield return null;
        }
        (toHome ? (Tile)job : home).Leave(this);
        for (int i = 0; i < commute.cells.Count; i++) {
            currentCell = commute.cells[toHome ? Mathf.Min(commute.cells.Count - 1, commute.cells.Count - i) : Mathf.Max(0, i - 1)];
            nextCell = commute.cells[toHome ? commute.cells.Count - 1 - i : i];
            StartCoroutine(WalkToNext());
            while (moving) yield return null;
        }
        if ((toHome ? (Tile)home : job).Arrive(this)) {
            status = toHome ? Status.Home : Status.Work;
            gameController.SuccessfulCommute();
            dest = toHome ? homePos : workPos;
            while ((Vector2)transform.position != dest) {
                if (status == Status.Destroy)
                    yield break;
                transform.position = Vector2.MoveTowards((Vector2)transform.position, dest, Time.deltaTime * WalkSpeed);
                yield return null;
            }
        } else {
            StartCoroutine(Destroy());
        }
    }

    IEnumerator WalkToNext() {
        moving = true;
        Vector2 dest = nextCell.transform.position;
        if (currentCell != nextCell) {
            while (currentCell.occupiedDirections.Contains(nextCell)) yield return new WaitForSeconds(.1f);
            currentCell.occupiedDirections.Add(nextCell);
            while (Vector2.Distance(transform.position, currentCell.transform.position) < .25f) {
                if (status == Status.Destroy) {
                    currentCell.occupiedDirections.Remove(nextCell);
                    yield break;
                }
                float speed = WalkSpeed * (currentCell.roadConnections.Contains(nextCell) ? DriveSpeedMultiplier : 1f);
                transform.position = Vector2.MoveTowards(transform.position, dest, Time.deltaTime * speed);
                yield return null;
            }
            currentCell.occupiedDirections.Remove(nextCell);
        }
        while ((Vector2)transform.position != dest) {
            if (status == Status.Destroy)
                yield break;
            float speed = WalkSpeed * (currentCell.roadConnections.Contains(nextCell) ? DriveSpeedMultiplier : 1f);
            transform.position = Vector2.MoveTowards(transform.position, dest, Time.deltaTime * speed);
            yield return null;
        }
        moving = false;
    }

    IEnumerator CreationAnimation() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime * 3f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.25f, v);
            yield return null;
        }
        while (t > 0f) {
            t -= Time.deltaTime * 5f;
            float v = Mathf.Sin(Mathf.PI * (1 - t) / 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(1.25f, 1f, v);
            yield return null;
        }
    }

    public IEnumerator Destroy() {
        status = Status.Destroy;
        if(home != null)
            home.RemoveResident(this);
        if (job != null) {
            job.RemoveWorker(this);
        }
        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 5f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.25f, v);
            yield return null;
        }
        t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 3f;
            float v = Mathf.Sin(Mathf.PI * (1 - t) / 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.25f, v);
            yield return null;
        }
        Destroy(this.gameObject);
    }

    public enum Status {
        Home,
        CommuteToWork,
        Work,
        CommuteHome,
        Destroy
    }
}
