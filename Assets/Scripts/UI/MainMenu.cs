using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public SceneController sceneController;

    CanvasGroup main;

    private void Awake() {
        main = GetComponent<CanvasGroup>();
    }

    private void Start() {
        main.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    public void LevelSelect() {
        sceneController.LoadLevelSelect();
    }

    IEnumerator FadeIn(bool fadeIn = true) {
        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 2f;
            main.alpha = fadeIn ? t : 1 - t;
            yield return null;
        }
    }
}
