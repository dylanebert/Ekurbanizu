using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinWindow : Window {

    protected override IEnumerator ShowCoroutine() {
        if (gameController.activeWindow != null) {
            gameController.activeWindow.Hide();
            while (gameController.activeWindow != null) {
                yield return null;
            }
        }

        StartCoroutine(base.ShowCoroutine());
        StartCoroutine(gameController.cameraFly.FlyToOverview());

        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t;
            yield return null;
        }

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);

        canvasGroup.blocksRaycasts = true;
    }

    protected override IEnumerator HideCoroutine() {
        StartCoroutine(base.HideCoroutine());

        StartCoroutine(gameController.cameraFly.ReturnToMap());

        RectTransform rect = canvasGroup.GetComponent<RectTransform>();
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            gameController.mainUI.alpha = t;
            canvasGroup.alpha = 1 - t;
            yield return null;
        }

        gameController.mainUI.alpha = 1;
        canvasGroup.alpha = 0;

        gameController.activeWindow = null;
    }
}
