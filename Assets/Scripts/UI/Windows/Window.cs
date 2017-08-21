using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    public CanvasGroup canvasGroup;

    protected GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Show() {
        StartCoroutine(ShowCoroutine());
    }

    public void Hide() {
        StartCoroutine(HideCoroutine());
    }

    protected virtual IEnumerator ShowCoroutine() {
        gameController.activeWindow = this;
        gameController.SetGameSpeed(GameSpeed.Paused);
        gameController.uiBlock.SetActive(true);
        yield return null;
    }

    protected virtual IEnumerator HideCoroutine() {
        gameController.SetGameSpeed(GameSpeed.Normal);
        gameController.uiBlock.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        yield return null;
    }

    public void Deactivate() {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }
}
