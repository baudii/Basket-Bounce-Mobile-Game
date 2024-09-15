using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField] DollyCameraController dollyController;
    public static LevelManager Instance => instance;
    static LevelManager instance;
    [SerializeField] int testLevel;
    [SerializeField] bool testLastLevel;
    [SerializeField] bool testLastOpenedLevel;
    [SerializeField] bool validate;
    [SerializeField] bool enumerate;
    [SerializeField] GameObject tutorial;

    [SerializeField] List<GameObject> levels;

    public LevelData CurrentLevelData { get; private set; }
    int currentLevel;

    public const string LAST_OPENED_LEVEL_KEY = "Player_Last_Level";
    public const string LEVEL_STARS_KEY = "Level_Stars_";

    int lastOpenedLevel;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;

        GameManager.Instance.GetUILevelSelector().Init(levels.Count);
        GameManager.Instance.OnRestart.AddListener(OnRestart);

        int i = 0;
        transform.ForEachChild(child =>
        {
            if (child.name.Contains("Level"))
            {
                if (child.gameObject.activeSelf)
                    lastOpenedLevel = i;
                child.gameObject.SetActive(false);
            }
            i++;
        });
    }

    void OnRestart()
    {
        if (CurrentLevelData == null)
        {
            this.SmartLog("Current level data is NULL");
            return;
        }

        this.SmartLog(CurrentLevelData.gameObject.name);
        CurrentLevelData.OnRestart();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!validate)
            return;

        levels = new List<GameObject>();
        int i = 0;
        transform.ForEachChild(child =>
        {
            if (child.TryGetComponent(out LevelData levelData))
            {
                if (enumerate)
                {
                    child.name = "Level " + i;
                }
                levels.Add(child.gameObject);
                levelData.AdjustShadows();
                i++;
            }
        });
    }
#endif
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL_KEY, -1);
#if UNITY_EDITOR
        currentLevel = testLevel;
        if (testLastLevel)
            currentLevel = levels.Count - 1;
        else if (testLastOpenedLevel)
            currentLevel = lastOpenedLevel;
#endif
        tutorial.SetActive(false);
        if (currentLevel < 0)
        {
            currentLevel = 0;
            tutorial.SetActive(true);
        }
        LoadLevel(currentLevel, true);
    }

    public void SaveProgress(int stars)
    {
        int savedProgress = PlayerPrefs.GetInt(LEVEL_STARS_KEY + currentLevel, 0);
        if (savedProgress < stars)
            PlayerPrefs.SetInt(LEVEL_STARS_KEY + currentLevel, stars);

        this.SmartLog("Progress saved. Stars: " + stars + ", Level: " + currentLevel);
    }

    public void NextLevel()
    {
        if (currentLevel == levels.Count - 1)
        {
            // if last level
            GameManager.Instance.FinishGame();
            return;
        }
        LoadLevel(currentLevel + 1);
    }

    /// <summary>Loads level</summary>
    /// <param name="level">Level numeration starts from 0</param>
    public void LoadLevel(int level, bool isFirstTimeLoad = false) 
    {
        if (level >= levels.Count)
            return;

        float loadTime = 1f;
        if (isFirstTimeLoad) 
            loadTime = 3f;

        // gamemanager
        GameManager.Instance.SetActiveLoadingScreen(true);

        // activate level
        levels[currentLevel].SetActive(false);
        levels[level].SetActive(true);

        // update level data
        if (levels[level].TryGetComponent(out LevelData levelData))
            CurrentLevelData = levelData;
        else
            this.SmartError("Couldn't retrieve level data. Level num: " + level + ". levels[level] GO: " + levels[level].name);
        currentLevel = level;

        CurrentLevelData.OnRestart();

        // load player's progress
        int lastSavedLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL_KEY, -1);
        if (lastSavedLevel < currentLevel)
            PlayerPrefs.SetInt(LAST_OPENED_LEVEL_KEY, currentLevel);

        // update camera
        dollyController.UpdateDollyWaypoint(CurrentLevelData.GetFinPos());

        this.Co_DelayedExecute(() =>
        {
        GameManager.Instance.UpdateLevelSelector();
            GameManager.Instance.SetActiveLoadingScreen(false);
            GameManager.Instance.Restart();
        }, loadTime);
    }
}
