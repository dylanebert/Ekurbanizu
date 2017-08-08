using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {

    public Image clockHand;
    public Image homeArc;
    public Image rushHourArc;
    public Image workArc;
    public Image rushHourArc2;
    public Text clockText;

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        homeArc.fillAmount = gameController.commuteToWorkTime / gameController.assertHomeTime;
        rushHourArc.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, -360f, homeArc.fillAmount));
        rushHourArc.fillAmount = (gameController.assertWorkTime - gameController.commuteToWorkTime) / gameController.assertHomeTime;
        workArc.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, -360f, homeArc.fillAmount + rushHourArc.fillAmount));
        workArc.fillAmount = (gameController.commuteHomeTime - gameController.assertWorkTime) / gameController.assertHomeTime;
        rushHourArc2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, -360f, homeArc.fillAmount + rushHourArc.fillAmount + workArc.fillAmount));
        rushHourArc2.fillAmount = (gameController.assertHomeTime - gameController.commuteHomeTime) / gameController.assertHomeTime;
    }

    private void Update() {
        float v = gameController.GetTimer() / gameController.assertHomeTime;
        clockHand.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -360f, v));
    }

    public IEnumerator UpdateText(string text) {
        clockText.transform.localScale = Vector3.zero;
        clockText.text = text;
        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 5f;
            clockText.transform.localScale = Vector3.one * Mathf.Lerp(0f, .575f, t);
            yield return null;
        }
        t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 5f;
            clockText.transform.localScale = Vector3.one * Mathf.Lerp(.575f, .5f, t);
            yield return null;
        }
        clockText.text = text;
    }
}
