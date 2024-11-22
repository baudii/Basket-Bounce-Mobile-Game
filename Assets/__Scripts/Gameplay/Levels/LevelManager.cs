using System;
using UnityEngine;
using UnityEngine.Events;
using BasketBounce.Systems;
using BasketBounce.Models;
using UnityEngine.AddressableAssets;
using KK.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelManager : MonoBehaviour
	{
		LevelSet currentLevelSet;

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

		IList<GameObject> levelSetPrefabs;

		public async Task Init(GameManager gameManager)
		{
			this.gameManager = gameManager;
			var op = Addressables.LoadAssetsAsync<GameObject>("LevelSets", null);
			await op.Task;
			levelSetPrefabs = op.Result;
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

		public static string GetLastLevelSetIdKey()
		{
			return "Last-Played-Level-Set";
		}

		public async Task ActivateLevelSet(int levelSet)
		{
			destroyCancellationToken.ThrowIfCancellationRequested();

			await gameManager.StartLoading(destroyCancellationToken);

			if (levelSetPrefabs == null)
				throw new ArgumentNullException($"Level set prefabs collections is null. It should be initialized.");

			if (levelSet < 0 || levelSet >= levelSetPrefabs.Count)
				throw new ArgumentOutOfRangeException($"Provided level set index={levelSet} is invalid. Should be at least between [0, {levelSetPrefabs.Count - 1}] inclusive");
			
			var levelSetPrefab = levelSetPrefabs[levelSet];
			var levelSetGo = Instantiate(levelSetPrefab, Vector3.zero, Quaternion.identity, transform);
			currentLevelSet = levelSetGo.GetComponent<LevelSet>();
			OnLevelSetAvailable?.Invoke(currentLevelSet);
			await ActivateLastSavedLevel();
		}

		public Task ActivateLastSavedLevel()
		{
			destroyCancellationToken.ThrowIfCancellationRequested();

			var key = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
			CurrentLevel = PlayerPrefs.GetInt(key, 0);
			currentLevelSet.Init(CurrentLevel);
			return LoadLevelAsync(CurrentLevel);
		}

		void SwapLevelTo(int level)
		{
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

		void TrySaveStars(int stars)
		{
			var earnedStarsKey = GetEarnedStarsKey(CurrentLevel, currentLevelSet.LevelSetId);
			int savedProgress = PlayerPrefs.GetInt(earnedStarsKey, 0);
			if (stars > savedProgress)
				PlayerPrefs.SetInt(earnedStarsKey, stars);
		}

		void TrySaveLastDiscoveredLevel()
		{
			var lastLevelKey = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
			int savedProgress = PlayerPrefs.GetInt(lastLevelKey, 0);
			if (CurrentLevel + 1 > savedProgress)
				PlayerPrefs.SetInt(lastLevelKey, CurrentLevel + 1);
		}

		void TrySaveLastLevelSet()
		{
			var lastLevelSetIdKey = GetLastLevelSetIdKey();
			int savedProgress = PlayerPrefs.GetInt(lastLevelSetIdKey, 0);
			if (LevelSetId > savedProgress)
				PlayerPrefs.SetInt(lastLevelSetIdKey, LevelSetId);
		}
		
		public void SetupLevel()
		{
			OnLevelSetupEvent?.Invoke(CurrentLevelData);
		}

		public void OnFinishedLevel(ScoreData scoreData)
		{
			if (CurrentLevel + 1 < currentLevelSet.LevelCount)
				TrySaveLastDiscoveredLevel();

			TrySaveStars(scoreData.stars);
			TrySaveLastLevelSet();

			OnFinishedLevelEvent?.Invoke(scoreData);
		}

		public void OnBallReleased()
		{
			CurrentLevelData.OnBallReleased();
		}

		public async Task NextLevel()
		{
			if (CurrentLevel == currentLevelSet.LevelCount - 1)
			{
				OnFinishedGameEvent?.Invoke();
				await ActivateLevelSet(LevelSetId + 1);
				return;
			}
			await LoadLevelAsync(CurrentLevel + 1);
		}

		public async Task LoadLevelAsync(int level)
		{
			try
			{
				destroyCancellationToken.ThrowIfCancellationRequested();
				await gameManager.StartLoading(destroyCancellationToken);

				this.Log("Loading level: Level", level);
				if (level >= currentLevelSet.LevelCount)
					throw new ArgumentException($"Incorrect level={level} was provided");

				SwapLevelTo(level);
				SetupLevel();
				CallOnFirstTimeLoad(level);

				await Task.Delay(1000, destroyCancellationToken);

				OnLevelIsLoadedEvent?.Invoke(CurrentLevelData);
				await gameManager.ResumeGame(destroyCancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				this.Log(GameManager.GetOperationCancelledLog(ex));
			}
		}
	}
}
