using UnityEngine;
using UnityEngine.Events;
using KK.Common;
using System;

namespace BasketBounce.Systems
{
	[DefaultExecutionOrder(-1)]
	public class LevelManager : Singleton<LevelManager>
	{
		#region Editor functionality

#if UNITY_EDITOR
		[Header("Level state")]
		[SerializeField] bool validate;
		[SerializeField] bool testReflectionMode;
		[Header("Test levels")]
		[SerializeField] int testLevel;
		[SerializeField] bool testLastOpenedLevel;

		private void OnValidate()
		{
			if (validate)
			{
				foreach (Transform t in transform)
				{
					if (t.TryGetComponent(out LevelSet levelSet))
					{
						currentLevelSet = levelSet;
						levelSet.gameObject.name = "Level Set 1";
						return;
					}
				}
			}
		}
#endif

		#endregion

		[SerializeField] DollyCameraController dollyController;
		[SerializeField] LevelSet currentLevelSet;
		[SerializeField] GameObject scrollGO;
		[SerializeField] UnityEvent OnClickAnywhereEvent;

		[HideInInspector]
		public UnityEvent<LevelData> OnLevelSetup;
		[HideInInspector]
		public UnityEvent<LevelData> OnLevelIsLoaded;

		public LevelData CurrentLevelData { get; private set; }
		public int CurrentLevel { get; private set; }

		// Last opened means the last level that can be chosen from level select - Switches to next when level is finished
		public const string LAST_DISCOVERED_LEVEL_KEY = "Player_Last_Level";
		public const string LEVEL_EARNED_STARS_KEY = "Level_Stars_";
		public bool isReflectionMode { get; private set; }
		public void ActivateReflection() => isReflectionMode = true;

		protected override void OnAwake()
		{
			GameManager.Instance.GetUILevelSelector().Init(currentLevelSet.LevelCount);
		}

		void Start()
		{
			CurrentLevel = PlayerPrefs.GetInt(LAST_DISCOVERED_LEVEL_KEY, -1);
			bool isFirstTimeLoad = true;

#if UNITY_EDITOR
			isFirstTimeLoad = false;
			CurrentLevel = testLevel;
#endif

			if (CurrentLevel < 0)
				CurrentLevel = 0;

			currentLevelSet.Init(CurrentLevel);

			LoadLevel(CurrentLevel, isFirstTimeLoad);
		}

		void OnDestroy()
		{
			OnLevelSetup.RemoveAllListeners();
		}

		void SwapLevelTo(int level)
		{
			// Переключение уровней через включение и выключение объектов
			CurrentLevelData?.gameObject.SetActive(false);
			CurrentLevelData = currentLevelSet.GetLevel(level);
			CurrentLevelData.gameObject.SetActive(true);
			CurrentLevelData.Init();
			CurrentLevel = level;

			/*Переключение через инстансы префабов
					if (CurrentLevelData != null)
				Destroy(CurrentLevelData.gameObject);
			CurrentLevelData = Instantiate(levels[level], transform);
			CurrentLevel = level;*/
		}

		void UpdateSaveData()
		{
			// update player's progress
			int lastOpenedLevel = PlayerPrefs.GetInt(LAST_DISCOVERED_LEVEL_KEY, -1);
			if (CurrentLevel >= lastOpenedLevel)
			{
				// Level is not yet finished!
				this.SmartLog("First time loading level", CurrentLevel);
				CurrentLevelData.OnFirstTimeLoad();
				PlayerPrefs.SetInt(LAST_DISCOVERED_LEVEL_KEY, CurrentLevel);
			}
		}

		public void SetupLevel()
		{
			if (CurrentLevelData == null)
			{
				this.SmartLog("Current level data is NULL");
				return;
			}

			this.SmartLog(CurrentLevelData.gameObject.name);
			dollyController.ResetDollyPathPos();
			dollyController.UpdateDollyWaypoint(CurrentLevelData.GetFinPos());
			OnLevelSetup?.Invoke(CurrentLevelData);
			//isReflectionMode = false;
		}

		public void OnClickAnywhere()
		{
			CurrentLevelData?.OnClickAnywhere();
			OnClickAnywhereEvent?.Invoke();
		}

		public void OnFinish(int stars)
		{
			int savedProgress = PlayerPrefs.GetInt(LEVEL_EARNED_STARS_KEY + CurrentLevel, 0);
			if (stars > savedProgress)
				PlayerPrefs.SetInt(LEVEL_EARNED_STARS_KEY + CurrentLevel, stars);

			savedProgress = PlayerPrefs.GetInt(LAST_DISCOVERED_LEVEL_KEY, 0);
			if (CurrentLevel + 1 > savedProgress)
				PlayerPrefs.SetInt(LAST_DISCOVERED_LEVEL_KEY, CurrentLevel + 1);
		}

		public void OnBallReleased()
		{
			CurrentLevelData.OnBallReleased();
		}

		public void NextLevel()
		{
			if (CurrentLevel == currentLevelSet.LevelCount - 1)
			{
				GameManager.Instance.FinishLevelSet();
				return;
			}
			LoadLevel(CurrentLevel + 1);
		}

		public void LoadLevel(int level, bool isFirstTimeLoad = false)
		{
			this.SmartLog("Loading level: Level ", level);
			if (level >= currentLevelSet.LevelCount)
				throw new ArgumentException("Incorrect level was provided");

			GameManager.Instance.SetActiveLoadingScreen(true);

			float loadTime = 1f;
			if (isFirstTimeLoad)
				loadTime = 3f;

			SwapLevelTo(level);
			SetupLevel();
			UpdateSaveData();

			GameManager.Instance.UpodateUI();

			this.Co_DelayedExecute(() =>
			{
				OnLevelIsLoaded?.Invoke(CurrentLevelData);
				GameManager.Instance.SetActiveLoadingScreen(false);
				GameManager.Instance.OnLevelLoad(CurrentLevelData.LevelHeader, CurrentLevel + 1);
				GameManager.Instance.ResumeGame();
			}, loadTime);
		}
	}
}
