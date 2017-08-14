using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public UnityStandardAssets.ImageEffects.BlurOptimized blur;
    public SceneController sceneControllerObj;
    public LensButton[] pointerButtons;
    public GameObject emptyTileObj;
    public GameObject residentialTileObj;
    public GameObject industrialTileObj;
    public GameObject residentObj;
    public GameObject residentialLegend;
    public GameObject industrialLegend;
    public GameObject pauseMenuCanvas;
    public GameObject uiBlock;
    public CameraFly cameraFly;
    public Grid grid;
    public Text residentPopulationText;
    public Text workerPopulationText;
    public Text roadsAvailableText;
    public Text maxResidentialCapacityText;
    public Text maxIndustrialCapacityText;
    public Text tooltipTitle;
    public Text tooltipText;
    public Text successfulDailyCommutesText;
    public Text commutesWindowCountText;
    public Text commutesWindowGoalText;
    public Text winCommutesText;
    public Image pauseButton;
    public Image playButton;
    public Image fastForwardButton;
    public CanvasGroup commutesWindow;
    public CanvasGroup pauseMenu;
    public CanvasGroup winScreen;
    public CanvasGroup mainUI;
    public float commuteToWorkTime = 10f;
    public float assertWorkTime = 20f;
    public float commuteHomeTime = 30f;
    public float assertHomeTime = 40f;
    public float birthInterval = 10f;
    public Clock clock;
    public bool touchMode;

    [HideInInspector]
    public Lens pointerState;
    [HideInInspector]
    public Lens lens;
    [HideInInspector]
    public List<Resident> residents;
    [HideInInspector]
    public List<Resident> workers;
    [HideInInspector]
    public int residentsHomeCount;
    [HideInInspector]
    public int workersAtWorkCount;
    [HideInInspector]
    public Stage stage;
    [HideInInspector]
    public int maxResidentialCapacity;
    [HideInInspector]
    public int maxIndustrialCapacity;
    [HideInInspector]
    public bool firstResidentialPlaced;
    [HideInInspector]
    public bool pauseMenuShown;
    [HideInInspector]
    public bool winScreenShown;

    SceneController sceneController;
    float timer;
    float commutesTextAnimationTimer;
    int goalCommutes;
    int successfulDailyCommutes;
    bool pauseMenuAnimating;
    bool won;
    bool timeControlsEnabled;

    private void Awake() {
        if(Application.platform == RuntimePlatform.Android) {
            mainUI.GetComponent<CanvasScaler>().scaleFactor = 1.2f;
        }
    }

    private void Start() {        
        try {
            sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
        } catch(System.Exception e) {
            sceneController = Instantiate(sceneControllerObj).GetComponent<SceneController>();
        }
        GridData data = sceneController.levels[sceneController.currentLevel - 1].gridData;
        grid.Initialize(data, this);
        roadsAvailableText.text = " x" + grid.roadsAvailable.ToString();
        maxResidentialCapacity = grid.GetMaxResidentialCapacity();
        maxIndustrialCapacity = grid.GetMaxIndustrialCapacity();
        maxResidentialCapacityText.text = maxResidentialCapacity.ToString();
        maxIndustrialCapacityText.text = maxIndustrialCapacity.ToString();
        goalCommutes = data.goalCommutes;
        commutesWindowGoalText.text = goalCommutes + " to Win";

        if (grid.roadsAvailable == 0)
            pointerButtons[2].gameObject.SetActive(false);

        //StartCoroutine(DailyCommutesAnimation());
        //StartCoroutine(WinAnimation());
    }

    private void Update() {
        if (firstResidentialPlaced) {
            timer += Time.deltaTime;
            switch (stage) {
                case Stage.Home:
                    if (timer > commuteToWorkTime) {
                        foreach (Resident resident in residents) {
                            resident.SetGoal(Resident.Status.Work);
                        }
                        stage = Stage.CommutingToWork;
                        StartCoroutine(clock.UpdateText("To Work!"));
                    }
                    break;
                case Stage.CommutingToWork:
                    if (timer > assertWorkTime) {
                        Stack<Resident> residentStack = new Stack<Resident>(residents);
                        while (residentStack.Count > 0) {
                            Resident resident = residentStack.Pop();
                            resident.AssertWork();
                        }
                        stage = Stage.Working;
                        StartCoroutine(clock.UpdateText("At Work"));
                    }
                    break;
                case Stage.Working:
                    if (timer > commuteHomeTime) {
                        foreach (Resident resident in residents) {
                            resident.SetGoal(Resident.Status.Home);
                        }
                        stage = Stage.CommutingToHome;
                        StartCoroutine(clock.UpdateText("To Home!"));
                    }
                    break;
                case Stage.CommutingToHome:
                    if (timer > assertHomeTime) {
                        Stack<Resident> residentStack = new Stack<Resident>(residents);
                        while (residentStack.Count > 0) {
                            Resident resident = residentStack.Pop();
                            resident.AssertHome();
                        }
                        stage = Stage.Home;
                        StartCoroutine(clock.UpdateText("At Home"));
                        timer = 0f;
                        if (!won && successfulDailyCommutes >= goalCommutes) {
                            StartCoroutine(WinAnimation());
                        }
                        else {
                            StartCoroutine(DailyCommutesAnimation());
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        if(commutesTextAnimationTimer > 0f) {
            commutesTextAnimationTimer -= Time.unscaledDeltaTime * 5f;
            if (commutesTextAnimationTimer > .5f) {
                successfulDailyCommutesText.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.25f, (1f - commutesTextAnimationTimer) / .5f);
            } else {
                successfulDailyCommutesText.transform.localScale = Vector3.one * Mathf.Lerp(1.25f, 1f, (.5f - commutesTextAnimationTimer) / .5f);
            }
        }

        if (winScreenShown) {
            if (!pauseMenuAnimating && pauseMenuShown) {
                StartCoroutine(HidePauseMenu());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
    }

    public void AddRoad() {
        if(--grid.roadsAvailable == 0) {
            pointerButtons[2].gameObject.SetActive(false);
            DeselectPointerButtons();
        }
        roadsAvailableText.text = " x" + grid.roadsAvailable.ToString();
    }

    public void RemoveRoad() {
        if(grid.roadsAvailable++ == 0) {
            pointerButtons[2].gameObject.SetActive(true);
        }
        roadsAvailableText.text = " x" + grid.roadsAvailable.ToString();
    }

    public void SuccessfulCommute() { 
        successfulDailyCommutes++;
        commutesTextAnimationTimer = 1f;
    }

    IEnumerator DailyCommutesAnimation() {
        commutesTextAnimationTimer = 0f;

        commutesWindowCountText.text = successfulDailyCommutes.ToString();
        commutesWindow.alpha = 1;

        RectTransform rect = commutesWindow.GetComponent<RectTransform>();
        Vector2 origin = rect.anchoredPosition = new Vector2(0f, 60f);
        Vector2 target = new Vector2(0f, -40f);
        float t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            rect.anchoredPosition = Vector2.Lerp(origin, target, v);
            yield return null;
        }

        t = 0f;
        while(t < 2.5f) {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * (1 - t) / 2f);
            rect.anchoredPosition = Vector2.Lerp(origin, target, v);
            yield return null;
        }
        commutesWindow.alpha = 0;

        t = 0f;
        while (t < .5f) {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        successfulDailyCommutes = 0;
        commutesTextAnimationTimer = 1f;
    }

    public void MainMenu() {
        StartCoroutine(cameraFly.FlyToMenu());
        mainUI.alpha = 0;
        winScreen.alpha = 0;
        pauseMenu.alpha = 0;
    }


    public void Pause() {
        if (pauseMenuAnimating || winScreenShown) return;
        if (!pauseMenuShown) {
            StartCoroutine(ShowPauseMenu());
        }
        else {
            StartCoroutine(HidePauseMenu());
        }
    }

    IEnumerator ShowPauseMenu() {
        pauseMenuAnimating = true;
        SetGameSpeed(GameSpeed.Paused);
        pauseMenuShown = true;
        blur.enabled = true;
        pauseMenuCanvas.SetActive(true);
        uiBlock.SetActive(true);

        RectTransform menu = pauseMenu.GetComponent<RectTransform>();
        menu.anchoredPosition = new Vector2(0, -Screen.height);
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float y = -Screen.height - -Screen.height * Mathf.Sin(t * Mathf.PI / 2f);
            menu.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        pauseMenu.blocksRaycasts = true;

        pauseMenuAnimating = false;
    }

    IEnumerator HidePauseMenu() {
        pauseMenuAnimating = true;

        RectTransform menu = pauseMenu.GetComponent<RectTransform>();
        pauseMenu.blocksRaycasts = false;
        blur.enabled = false;

        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float y = -Screen.height - -Screen.height * Mathf.Sin((1 - t) * Mathf.PI / 2f);
            menu.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        pauseMenuShown = false;
        SetGameSpeed(GameSpeed.Normal);
        pauseMenuCanvas.SetActive(false);
        uiBlock.SetActive(false);

        pauseMenuAnimating = false;
    }

    IEnumerator WinAnimation() {
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            Time.timeScale = 1 - v;
            yield return null;
        }
        SetGameSpeed(GameSpeed.Paused);

        winScreenShown = true;
        won = true;
        uiBlock.SetActive(true);
        winCommutesText.text = successfulDailyCommutes.ToString();
        StartCoroutine(cameraFly.FlyToOverview());
        commutesWindowGoalText.enabled = false;

        mainUI.alpha = 0;
        t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime;
            winScreen.alpha = t;
            yield return null;
        }

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);

        winScreen.blocksRaycasts = true;
    }

    public void HideWinScreen() {
        StartCoroutine(HideWinScreenCoroutine());
    }

    IEnumerator HideWinScreenCoroutine() {
        winScreen.blocksRaycasts = false;
        commutesTextAnimationTimer = 0;
        uiBlock.SetActive(false);
        StartCoroutine(cameraFly.ReturnToMap());

        RectTransform rect = winScreen.GetComponent<RectTransform>();
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            winScreen.alpha = 1 - t;
            mainUI.alpha = t;
            yield return null;
        }
        winScreen.alpha = 0;

        winScreenShown = false;
        SetGameSpeed(GameSpeed.Normal);
        successfulDailyCommutes = 0;
        commutesTextAnimationTimer = 1f;
    }

    private void OnGUI() {
        residentPopulationText.text = grid.GetResidentialCapacity().ToString();
        workerPopulationText.text = grid.GetIndustrialCapacity().ToString();
        successfulDailyCommutesText.text = successfulDailyCommutes.ToString();
    }

    public void UpdateTileColors() {
        foreach(Cell cell in grid.cells) {
            cell.tile.UpdateColor();
        }
    }

    public void SetLens(Lens lens) {
        this.lens = lens;
        residentialLegend.SetActive(false);
        industrialLegend.SetActive(false);
        grid.ShowRoads(false);

        switch (lens) {
            case Lens.Road:
                grid.ShowRoads(true);
                break;
            case Lens.Residential:
                residentialLegend.SetActive(true);
                break;
            case Lens.Industrial:
                industrialLegend.SetActive(true);
                break;
        }

        UpdateTileColors();
    }

    public void SetPointerState(Lens pointerState) {
        this.pointerState = pointerState;
        tooltipTitle.text = "";
        tooltipText.text = "";

        string cancelText = (touchMode ? "Tap " : "Click ") + "outside to cancel";
        switch (pointerState) {
            case Lens.Road:
                tooltipTitle.text = "Place roads";
                tooltipText.text = cancelText;
                break;
            case Lens.Residential:
                tooltipTitle.text = "Assign residential tiles";
                tooltipText.text = cancelText;
                break;
            case Lens.Industrial:
                tooltipTitle.text = "Assign workplace tiles";
                tooltipText.text = cancelText;
                break;
            case Lens.Erase:
                tooltipTitle.text = "Erase residential/workplace tiles";
                tooltipText.text = cancelText;
                break;
        }
    }

    public void SetGameSpeed(GameSpeed gameSpeed) {
        SetGameSpeed((int)gameSpeed);
    }

    public void SetGameSpeed(int gameSpeed) {
        if(!timeControlsEnabled) {
            StartCoroutine(ShowTimeControls());            
        }

        pauseButton.GetComponent<GameSpeedButton>().selected = false;
        playButton.GetComponent<GameSpeedButton>().selected = false;
        fastForwardButton.GetComponent<GameSpeedButton>().selected = false;

        Time.timeScale = gameSpeed;
        pauseButton.color = playButton.color = fastForwardButton.color = Palette.OffBlack;
        switch(gameSpeed) {
            case 0:
                pauseButton.GetComponent<GameSpeedButton>().selected = true;
                pauseButton.color = Palette.Gray;
                break;
            case 1:
                playButton.GetComponent<GameSpeedButton>().selected = true;
                playButton.color = Palette.Gray;
                break;
            case 2:
                fastForwardButton.GetComponent<GameSpeedButton>().selected = true;
                fastForwardButton.color = Palette.Gray;
                break;
            default:
                break;
        }
    }

    IEnumerator ShowTimeControls() {
        timeControlsEnabled = true;

        Transform[] transforms = new Transform[] { pauseButton.transform, playButton.transform, fastForwardButton.transform };
        foreach(Transform tr in transforms) {
            tr.gameObject.SetActive(true);
        }

        float t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime * 2f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            foreach(Transform tr in transforms) {
                tr.localScale = Vector3.one * v;
            }
            yield return null;
        }
    }

    public void DeselectPointerButtons() {
        foreach(LensButton pointerButton in pointerButtons) {
            pointerButton.Deselect();
        }
    }

    public float GetTimer() {
        return timer;
    }
}

public enum Lens {
    Default,
    Residential,
    Industrial,
    Road,
    Erase
}

public enum Stage {
    Home,
    CommutingToWork,
    Working,
    CommutingToHome
}

public enum GameSpeed {
    Paused = 0,
    Normal = 1,
    Fast = 2
}