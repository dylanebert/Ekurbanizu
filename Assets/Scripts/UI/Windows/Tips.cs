using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : Window {

    public Text titleText;
    public Image sprite;
    public Text bodyText;
    public GameObject nextButton;
    public Text indexText;

    TipData[] tips;
    int index = -1;

    private void Start() {
        tips = gameController.grid.gridData.tips;

        if (tips.Length > 0 && PlayerPrefs.GetInt("ShowTips", 1) == 1) {
            NextTip();
            Show();
        }
    }

    public void NextTip() {
        if(index < tips.Length - 1) {
            TipData current = tips[++index];
            titleText.text = current.title;
            sprite.sprite = current.sprite;
            sprite.color = current.spriteColor;
            bodyText.text = current.text;
            if (index == tips.Length - 1)
                nextButton.SetActive(false);
            indexText.text = (index + 1) + "/" + tips.Length;
        }
    }

    public void ToggleTips(bool show) {
        PlayerPrefs.SetInt("ShowTips", (show ? 1 : 0));
    }

    protected override IEnumerator ShowCoroutine() {
        if (gameController.activeWindow != null) {
            gameController.activeWindow.Hide();
            while (gameController.activeWindow != null) {
                yield return null;
            }
        }

        StartCoroutine(base.ShowCoroutine());

        float t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.transform.localScale = Vector3.zero;
        gameController.blur.enabled = true;

        t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            canvasGroup.transform.localScale = Vector3.one * v;
            yield return null;
        }

        canvasGroup.blocksRaycasts = true;
    }

    protected override IEnumerator HideCoroutine() {
        StartCoroutine(base.HideCoroutine());

        gameController.blur.enabled = false;

        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            canvasGroup.transform.localScale = Vector3.one * (1 - v);
            yield return null;
        }

        canvasGroup.alpha = 0;

        gameController.activeWindow = null;
    }
}
