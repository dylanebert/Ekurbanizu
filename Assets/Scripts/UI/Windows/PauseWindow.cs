using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseWindow : Window {

    protected override IEnumerator ShowCoroutine() {
        if (gameController.activeWindow != null) {
            gameController.activeWindow.Hide();
            while (gameController.activeWindow != null) {
                yield return null;
            }
        }

        StartCoroutine(base.ShowCoroutine());

        gameController.blur.enabled = true;
        canvasGroup.alpha = 1;

        RectTransform menu = canvasGroup.GetComponent<RectTransform>();
        menu.anchoredPosition = new Vector2(0, -Screen.height);
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float y = -Screen.height - -Screen.height * Mathf.Sin(t * Mathf.PI / 2f);
            menu.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        canvasGroup.blocksRaycasts = true;
    }

    protected override IEnumerator HideCoroutine() {
        StartCoroutine(base.HideCoroutine());

        gameController.blur.enabled = false;

        RectTransform rect = canvasGroup.GetComponent<RectTransform>();
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float y = -Screen.height - -Screen.height * Mathf.Sin((1 - t) * Mathf.PI / 2f);
            rect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        canvasGroup.alpha = 0;

        gameController.activeWindow = null;
    }
}
