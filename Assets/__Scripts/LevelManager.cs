using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CinemachineSmoothPath dollyTrack;
    public static LevelManager Instance => instance;
    static LevelManager instance;
    [SerializeField] int testLevel;
    [SerializeField] bool validate;
    [SerializeField] GameObject tutorial;

    [SerializeField] List<GameObject> levels;

    public LevelData CurrentLevelData { get; private set; }
    int currentLevel;

    public const string LAST_OPENED_LEVEL = "Player_Last_Level";
    public const string LEVEL_STARS = "Level_Stars_";

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;

        GameManager.Instance.OnRestart.AddListener(OnRestart);

        transform.ForEachChild(child =>
        {
            if (child.name.Contains("Level"))
            {
                child.gameObject.SetActive(false);
            }
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
        transform.ForEachChild(child =>
        {
            if (child.name.Contains("Level"))
                levels.Add(child.gameObject);
        });
    }
#endif
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL, -1);
#if UNITY_EDITOR
        currentLevel = testLevel;
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
        int savedProgress = PlayerPrefs.GetInt(LEVEL_STARS + currentLevel, 0);
        if (savedProgress < stars)
            PlayerPrefs.SetInt(LEVEL_STARS + currentLevel, stars);

        this.SmartLog("Progress saved. Stars: " + stars + ", Level: " + currentLevel);
    }

    public void NextLevel()
    {
        LoadLevel(currentLevel + 1);
    }

    /// <summary>Loads level</summary>
    /// <param name="level">Level numeration starts from 0</param>
    public void LoadLevel(int level, bool isFirstTimeLoad = false) 
    {
        if (level >= levels.Count)
            return;

        float loadTime = 0.5f;
        if (isFirstTimeLoad) loadTime = 4f;
        GameManager.Instance.SetActiveLoadingScreen(true);
        levels[currentLevel].SetActive(false);
        levels[level].SetActive(true);
        if (levels[level].TryGetComponent(out LevelData levelData))
            CurrentLevelData = levelData;
        else
            this.SmartWarning("Couldn't retrieve level data. Level num: " + level + ". leves[level] GO: " + levels[level].name);
        currentLevel = level;
        int lastSavedLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL, -1);
        if (lastSavedLevel < currentLevel)
            PlayerPrefs.SetInt(LAST_OPENED_LEVEL, currentLevel);

        GameManager.Instance.UpdateLevelSelector();

        //update camera dolly
        var waypoint = dollyTrack.m_Waypoints[1];
        waypoint.position = CurrentLevelData.GetFinPos().AddTo(y: -4, z: -10);
        dollyTrack.m_Waypoints[1] = waypoint;

        this.Co_DelayedExecute(() =>
        {
            GameManager.Instance.SetActiveLoadingScreen(false);
            GameManager.Instance.Restart();
            
        }, loadTime);
    }
}
