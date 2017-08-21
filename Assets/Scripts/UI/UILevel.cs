using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour {

    public Text title;
    public Image image;
    public Text description;
    public Text goal;

    [HideInInspector]
    public RectTransform rect;
    [HideInInspector]
    public bool selected;

    LevelSelect levelSelect;
    int level;

    private void Awake() {
        rect = GetComponent<RectTransform>();
    }

    public void Initialize(LevelData levelData, LevelSelect levelSelect, int level) {
        this.levelSelect = levelSelect;
        title.text = levelData.name;
        image.sprite = levelData.image;
        description.text = levelData.description;
        this.level = level;
        goal.text = "Goal: " + levelData.gridData.goalCommutes + " daily commutes";
    }

    public void Select() {
        if (!selected) {
            levelSelect.FocusLevel(this);
        } else {
            levelSelect.sceneController.LoadLevel(level);
        }
    }
}
