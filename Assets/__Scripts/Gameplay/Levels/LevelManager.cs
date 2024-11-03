using System;
using UnityEngine;
using UnityEngine.Events;
using BasketBounce.Systems;
using BasketBounce.Models;
using UnityEngine.AddressableAssets;
using KK.Common;
using System.Threading.Tasks;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelManager : MonoBehaviour
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

		[SerializeField] LevelSet currentLevelSet;

		[HideInInspector]
		public UnityEvent<LevelData> OnLevelSetupEvent;
		[HideInInspector]
		public UnityEvent<LevelData> OnLevelIsLoadedEvent;		
		[HideInInspector]
		public UnityEvent OnFinishedGameEvent;
		[HideInInspector]
		public UnityEvent<ScoreData> OnFinishedLevelEvent;
		[HideInInspector]
		public UnityEvent<LevelSet> OnLevelSetAvailable;

		public LevelData CurrentLevelData { get; private set; }
		public int CurrentLevel { get; private set; }

		GameManager gameManager;

		// Last opened means the last level that can be chosen from level select - Switches to next when level is finished
		public const string LEVEL_SET_KEY = "LevelSet-";
		//public string LastDiscoverdLevelKey => 

		public bool isReflectionMode { get; private set; }
		public void ActivateReflection() => isReflectionMode = true;

		public int LevelSetId => currentLevelSet.LevelSetId;

		public void Init(GameManager gameManager)
		{
			this.gameManager = gameManager;
		}

		void OnDestroy()
		{
			OnLevelSetupEvent.RemoveAllListeners();
		}

		public static string GetEarnedStarsKey(int currentLevel, int levelSetId)
		{
			return "Level-Stars-" + currentLevel + "Set-" + levelSetId;
		}

		public static string GetLastDiscoveredLevelKey(int levelSetId)
		{
			return "Player-Last-Level-" + levelSetId;
		}

		public async Task ActivateLevelSet(int levelSet)
		{
			await gameManager.StartLoading().AsTask(destroyCancellationToken);
			if (levelSet <= 0)
				throw new ArgumentOutOfRangeException($"Provided level set index={levelSet} is invalid. Should be at least 1");
			var op = Addressables.LoadAssetAsync<GameObject>(LEVEL_SET_KEY + levelSet);
			var delay = Task.Delay(1000);
			await op.Task;
			await delay;
			var levelSetPrefab = op.Result;
			var levelSetGo = Instantiate(levelSetPrefab, Vector3.zero, Quaternion.identity, transform);
			currentLevelSet = levelSetGo.GetComponent<LevelSet>();
			OnLevelSetAvailable?.Invoke(currentLevelSet);
			await ActivateLastSavedLevel();
		}

		public Task ActivateLastSavedLevel()
		{
			var key = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
			CurrentLevel = PlayerPrefs.GetInt(key, 0);
			currentLevelSet.Init(CurrentLevel);
			return LoadLevelAsync(CurrentLevel, true);
		}

		void SwapLevelTo(int level)
		{
			// Переключение уровней через включение и выключение объектов
			this.Log($"Swapping to level {level}");
			CurrentLevelData?.gameObject.SetActive(false);
			CurrentLevelData = currentLevelSet.GetLevel(level);
			CurrentLevelData.gameObject.SetActive(true);
			CurrentLevelData.Init(this, level);
			CurrentLevel = level;
		}

		void CallOnFirstTimeLoad(int currentLevel)
		{
			var key = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
			// update player's progress
			int lastOpenedLevel = PlayerPrefs.GetInt(key, -1);
			if (currentLevel >= lastOpenedLevel)
			{
				// Level is not yet finished!
				CurrentLevelData.OnFirstTimeLoad();
			}
		}

		void UpdateLevelSaveData(int stars)
		{
			var earnedStarsKey = GetEarnedStarsKey(CurrentLevel, currentLevelSet.LevelSetId);
			int savedProgress = PlayerPrefs.GetInt(earnedStarsKey, 0);
			if (stars > savedProgress)
				PlayerPrefs.SetInt(earnedStarsKey, stars);

			var lastLevelKey = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
			savedProgress = PlayerPrefs.GetInt(lastLevelKey, 0);
			if (CurrentLevel + 1 > savedProgress)
				PlayerPrefs.SetInt(lastLevelKey, CurrentLevel + 1);
		}
		
		public void SetupLevel()
		{
			OnLevelSetupEvent?.Invoke(CurrentLevelData);
		}

		public void OnFinishedLevel(ScoreData scoreData)
		{
			if (CurrentLevel + 1 < currentLevelSet.LevelCount)
				UpdateLevelSaveData(scoreData.stars);
			OnFinishedLevelEvent?.Invoke(scoreData);
		}

		public void OnBallReleased()
		{
			CurrentLevelData.OnBallReleased();
		}

		public void NextLevel()
		{
			if (CurrentLevel == currentLevelSet.LevelCount - 1)
			{
				OnFinishedGameEvent?.Invoke();
				return;
			}
			_ = LoadLevelAsync(CurrentLevel + 1);
		}

		public async Task LoadLevelAsync(int level, bool isFirstTimeLoad = false)
		{
			await gameManager.StartLoading().AsTask(destroyCancellationToken);

			this.Log("Loading level: Level ", level);
			if (level >= currentLevelSet.LevelCount)
				throw new ArgumentException($"Incorrect level={level} was provided");


			float loadTime = 1f;
			if (isFirstTimeLoad)
				loadTime = 3f;

			SwapLevelTo(level);
			SetupLevel();
			CallOnFirstTimeLoad(level);

			await Task.Delay((int)(loadTime * 1000), destroyCancellationToken);
			if (destroyCancellationToken.IsCancellationRequested)
				return;

			OnLevelIsLoadedEvent?.Invoke(CurrentLevelData);
			gameManager.ResumeGame();
		}
	}
}
