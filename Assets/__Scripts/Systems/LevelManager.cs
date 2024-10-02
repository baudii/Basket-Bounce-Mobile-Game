using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class LevelManager : MonoBehaviour
{
	[SerializeField] DollyCameraController dollyController;
	[SerializeField] List<LevelData> levels;
	[SerializeField] FinishIconHelper finishIconHelper;

	#region Editor functionality
#if UNITY_EDITOR
	[Header("Level state")]
	[SerializeField] bool validate;
	[SerializeField] bool testReflectionMode;
	[Header("Test levels")]
	[SerializeField] int testLevel;
	[SerializeField] bool testLastOpenedLevel;

	LevelData lastOpenedLevel;

	private void OnValidate()
	{
		if (validate)
		{
			ValidateLevels();
		}
	}

	void ValidateLevels()
	{
		int i = 0;

		levels = new List<LevelData>();

		lastOpenedLevel = null;

		if (testReflectionMode)
			ActivateReflection();

		foreach (Transform child in transform)
		{
			if (child.TryGetComponent(out LevelData levelData))
			{
				levelData.gameObject.name = "Level " + i;
				levels.Add(levelData);
				if (levelData.gameObject.activeSelf)
				{
					this.SmartLog("Found active gameobject:", levelData.gameObject.name);
					if (testLastOpenedLevel)
					{
						this.SmartLog("Test Level = ", i);
						lastOpenedLevel?.gameObject.SetActive(false);
						lastOpenedLevel = levelData;
						testLevel = i;
						this.SmartLog("lastOpenedLevel=" ,lastOpenedLevel);
					}
					else
					{
						levelData.gameObject.SetActive(false);
					}
				}
			}
			i++;
		}
	}
#endif
	#endregion

	[HideInInspector]
	public UnityEvent<LevelData> OnLevelSetup;


	public static LevelManager Instance => instance;
	static LevelManager instance;
	public LevelData CurrentLevelData { get; private set; }
	public int CurrentLevel { get; private set; }

	public const string LAST_OPENED_LEVEL_KEY = "Player_Last_Level";
	public const string LEVEL_STARS_KEY = "Level_Stars_";

	// Level specific powerup
	public bool isReflectionMode { get; private set; }
	public void ActivateReflection() => isReflectionMode = true;


	void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		instance = this;

#if UNITY_EDITOR
		ValidateLevels();
#endif

		GameManager.Instance.GetUILevelSelector().Init(levels.Count);
	}
	void Start()
	{
		CurrentLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL_KEY, -1);
		bool isFirstTimeLoad = true;

#if UNITY_EDITOR
		isFirstTimeLoad = false;
		CurrentLevel = testLevel;
#endif

		if (CurrentLevel < 0) 
			CurrentLevel = 0;

		LoadLevel(CurrentLevel, isFirstTimeLoad);
	}

	void OnDestroy()
	{
		OnLevelSetup.RemoveAllListeners();
	}

	void SwapToNewLevel(int level)
	{
		// Переключение уровней через включение и выключение объектов
		if (CurrentLevel >= 0)
		{
			levels[CurrentLevel].gameObject.SetActive(false);
		}
		CurrentLevelData = levels[level];
		CurrentLevelData.ResetLevel();
		CurrentLevelData.gameObject.SetActive(true);
		CurrentLevel = level;

		/*	Переключение через инстансы префабов
				if (CurrentLevelData != null)
					Destroy(CurrentLevelData.gameObject);
				CurrentLevelData = Instantiate(levels[level], transform);
				CurrentLevel = level;
		*/
	}


	public void SetupLevel()
	{
		if (CurrentLevelData == null)
		{
			this.SmartLog("Current level data is NULL");
			return;
		}

		this.SmartLog(CurrentLevelData.gameObject.name);
		OnLevelSetup?.Invoke(CurrentLevelData);
		dollyController.ResetDollyPathPos();
		//isReflectionMode = false;
	}

	public void OnFinish(int stars)
	{
		int savedProgress = PlayerPrefs.GetInt(LEVEL_STARS_KEY + CurrentLevel, 0);
		if (stars > savedProgress)
			PlayerPrefs.SetInt(LEVEL_STARS_KEY + CurrentLevel, stars);

		savedProgress = PlayerPrefs.GetInt(LAST_OPENED_LEVEL_KEY, 0);
		if (CurrentLevel + 1 > savedProgress && CurrentLevel + 1 < levels.Count)
			PlayerPrefs.SetInt(LAST_OPENED_LEVEL_KEY, CurrentLevel + 1);
	}

	public void OnBallReleased()
	{
		CurrentLevelData.OnBallRealeased();
	}


	public void NextLevel()
	{
		this.SmartLog(CurrentLevel, levels.Count - 1);
		if (CurrentLevel == levels.Count - 1)
		{
			GameManager.Instance.FinishGame();
			return;
		}
		LoadLevel(CurrentLevel + 1);
	}

	/// <summary>Loads level</summary>
	/// <param name="level">Level numeration starts from 0</param>
	public void LoadLevel(int level, bool isFirstTimeLoad = false)
	{
		this.SmartLog("Loading level: Level ", level);
		if (level >= levels.Count)
			return;

		GameManager.Instance.SetActiveLoadingScreen(true);
		float loadTime = 1f;
		if (isFirstTimeLoad)
			loadTime = 3f;

		SwapToNewLevel(level);

		// update player's progress
		int lastSavedLevel = PlayerPrefs.GetInt(LAST_OPENED_LEVEL_KEY, -1);
		if (CurrentLevel > lastSavedLevel)
			PlayerPrefs.SetInt(LAST_OPENED_LEVEL_KEY, CurrentLevel);

		// update dependencies
		var finpos = CurrentLevelData.GetFinPos();
		finishIconHelper.Init(finpos);
		dollyController.UpdateDollyWaypoint(finpos);

		SetupLevel();
		GameManager.Instance.UpodateUI();

		this.Co_DelayedExecute(() =>
		{
			GameManager.Instance.SetActiveLoadingScreen(false);
			GameManager.Instance.ResumeGame();
		}, loadTime);
	}
}
