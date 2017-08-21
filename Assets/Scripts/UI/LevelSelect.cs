using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    public GameObject sceneControllerObj;
    public GameObject levelObj;
    public CanvasGroup main;

    [HideInInspector]
    public bool animating;
    [HideInInspector]
    public SceneController sceneController;

    RectTransform mainRect;
    List<UILevel> uiLevels;
    UILevel focus;
    Vector2 vel;
    int currentLevel;

    private void Awake() {
        mainRect = main.GetComponent<RectTransform>();
    }

    private void Start() {
        try {
            sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
        }
        catch (System.Exception e) {
            sceneController = Instantiate(sceneControllerObj).GetComponent<SceneController>();
        }

        uiLevels = new List<UILevel>();
        for(int i = 0; i < sceneController.levels.Length; i++) {
            UILevel level = Instantiate(levelObj, main.transform).GetComponent<UILevel>();
            level.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 200f, 0f);
            level.Initialize(sceneController.levels[i], this, (i + 1));
            uiLevels.Add(level);
        }

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel > sceneController.levels.Length)
            currentLevel = sceneController.levels.Length;
        focus = uiLevels[currentLevel - 1];
    }

    private void Update() {
        if (Mathf.Abs(mainRect.anchoredPosition.x + focus.rect.anchoredPosition.x) > 5f) {
            mainRect.anchoredPosition = Vector2.SmoothDamp(mainRect.anchoredPosition, -focus.rect.anchoredPosition, ref vel, .2f, float.MaxValue, Time.deltaTime);
        } else if(!focus.selected) {
            focus.selected = true;
        }
    }

    public void MainMenu() {
        sceneController.LoadMenu();
    }

    public void FocusLevel(UILevel level) {
        foreach (UILevel uiLevel in uiLevels)
            uiLevel.selected = false;
        focus = level;
    }
}
