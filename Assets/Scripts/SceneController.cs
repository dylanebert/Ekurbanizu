using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public LevelData[] levels;

    public int currentLevel = 1;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        Screen.orientation = ScreenOrientation.Landscape;
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
        currentLevel = level;
        PlayerPrefs.SetInt("CurrentLevel", level);
        SceneManager.LoadScene(1);
    }

    public void Exit() {
        Application.Quit();
    }
}
