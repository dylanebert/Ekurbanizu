using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public LevelData[] levels;

    public int currentLevel;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        Screen.orientation = ScreenOrientation.Landscape;
        currentLevel = Mathf.Min(PlayerPrefs.GetInt("CurrentLevel", 1), levels.Length);
    }

    public void ResetPlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }

    public void LoadMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadLevelSelect() {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void LoadLevel(int level) {
        Time.timeScale = 1;
        if (level <= levels.Length) {
            currentLevel = level;
            PlayerPrefs.SetInt("CurrentLevel", level);
            SceneManager.LoadScene(1);
        } else {
            LoadMenu();
        }
    }

    public void Exit() {
        Application.Quit();
    }

    public void ToggleFullScreen() {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
