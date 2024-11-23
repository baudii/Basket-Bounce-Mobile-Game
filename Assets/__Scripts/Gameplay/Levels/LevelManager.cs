using System;
using UnityEngine;
using UnityEngine.Events;
using BasketBounce.Systems;
using BasketBounce.Models;
using UnityEngine.AddressableAssets;
using KK.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BasketBounce.Gameplay.Levels
{
	public class LevelManager : MonoBehaviour
	{
		#region Unity Events

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

		#endregion

		LevelSet currentLevelSet;
		public LevelData CurrentLevelData { get; private set; }
		public int CurrentLevel { get; private set; }

		GameManager gameManager;

		public int LevelSetId => currentLevelSet.LevelSetId;

		IList<GameObject> levelSetPrefabs;

		public async Task Init(GameManager gameManager)
		{
			this.gameManager = gameManager;
			var op = Addressables.LoadAssetsAsync<GameObject>("LevelSets", null);
			await op.Task;
			levelSetPrefabs = op.Result;

#if UNITY_EDITOR
			var lvl = gameManager.GetLevel();
			this.Log("get level = ", lvl);
			if (lvl == -1)
			{
				LevelSet lastLevelSet = null;
				foreach (Transform child in transform)
				{
					if (child.gameObject.activeSelf && child.TryGetComponent(out LevelSet levelSet))
					{
						if (lastLevelSet != null)
							lastLevelSet.gameObject.SetActive(false);

						lastLevelSet = levelSet;
					}
				}
				currentLevelSet = lastLevelSet;

				this.Log($"Found current level set: {currentLevelSet}");

				LevelChunk lastChunk = null;

				foreach (Transform child in lastLevelSet.transform)
				{
					if (child.gameObject.activeSelf && child.TryGetComponent(out LevelChunk chunk))
					{
						if (lastChunk != null)
							lastChunk.gameObject.SetActive(false);

						lastChunk = chunk;
					}
				}
				this.Log($"Found current chunk: {lastChunk}");

				int levelNum = 0;
				int i = 0;

				LevelData lastLevel = null;

				foreach (Transform child in lastChunk.transform)
				{
					if (child.gameObject.activeSelf && child.TryGetComponent(out LevelData levelData))
					{
						if (lastLevel != null)
							lastLevel.gameObject.SetActive(false);

						lastLevel = levelData;
						levelNum = i;
					}
					i++;
				}
				this.Log($"Found current level: {levelNum}");
				currentLevelSet.InitChunk(levelNum, lastChunk);
				OnLevelSetAvailable?.Invoke(currentLevelSet);
				await LoadLevelAsync(levelNum);
			}
#endif
		}

		void OnDestroy()
		{
			OnLevelSetupEvent.RemoveAllListeners();
		}


		public async Task ActivateLevelSet(int levelSet, int? level = null)
		{
			destroyCancellationToken.ThrowIfCancellationRequested();

			if (levelSetPrefabs == null)
				throw new ArgumentNullException($"Level set prefabs collections is null. It should be initialized.");

			if (levelSet < 0 || levelSet >= levelSetPrefabs.Count)
				throw new ArgumentOutOfRangeException($"Provided level set index={levelSet} is invalid. Should be at least between [0, {levelSetPrefabs.Count - 1}] inclusive");
			
			var levelSetPrefab = levelSetPrefabs[levelSet];
			var levelSetGo = Instantiate(levelSetPrefab, Vector3.zero, Quaternion.identity, transform);
			currentLevelSet = levelSetGo.GetComponent<LevelSet>();
			OnLevelSetAvailable?.Invoke(currentLevelSet);

			if (level == null)
			{
				var key = GetLastDiscoveredLevelKey(currentLevelSet.LevelSetId);
				var lastLevel = PlayerPrefs.GetInt(key, 0);
				await ActivateLevel(lastLevel);
			}
			else
			{
				await ActivateLevel(level.Value);
			}
		}

		public Task ActivateLevel(int level)
		{
			destroyCancellationToken.ThrowIfCancellationRequested();
			CurrentLevel = level;
			currentLevelSet.InitChunk(CurrentLevel);
			return LoadLevelAsync(CurrentLevel);
		}

		void SwapLevelTo(int level)
		{
			this.Log($"Swapping to level {level}");
			CurrentLevelData?.gameObject.SetActive(false);
			CurrentLevelData = currentLevelSet.GetLevel(level);
			CurrentLevelData.gameObject.SetActive(true);
			CurrentLevelData.Init();
			CurrentLevelData.LevelNum = level;
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
				await ActivateLevelSet(LevelSetId + 1, 0);
				return;
			}
			await LoadLevelAsync(CurrentLevel + 1);
		}

		public async Task LoadLevelAsync(int level)
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

		#region Save
		// ==================================================================================================================
		public static string GetEarnedStarsKey(int currentLevel, int levelSetId) => "Level-Stars-" + currentLevel + "Set-" + levelSetId;

		public static string GetLastDiscoveredLevelKey(int levelSetId) => "Player-Last-Level-" + levelSetId;

		public static string GetLastLevelSetIdKey() => "Last-Played-Level-Set";

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
		// ==================================================================================================================
		#endregion

	}
}
