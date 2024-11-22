using UnityEngine;
using UnityEngine.EventSystems;
using BasketBounce.Systems;
using BasketBounce.Gameplay.Levels;
using KK.Common;
using System.Threading.Tasks;

namespace BasketBounce.UI
{
	public class UI_Button_Handler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IInitializable
	{
		[SerializeField, Tooltip("Needed to adjust size. If none given - this gameobject's rectTransform will be taken by default")] RectTransform iconRect;
		[SerializeField] bool scaleIcon;

		GameManager gameManager;
		LevelManager levelManager;
		UI_Manager uiManager;


		private void Awake()
		{
			if (iconRect == null)
			{
				iconRect = (RectTransform)transform;
			}
		}

		public void Init()
		{
			DIContainer.GetDependency(out gameManager);
			DIContainer.GetDependency(out levelManager);
			DIContainer.GetDependency(out uiManager);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (scaleIcon)
			{
				iconRect.transform.localScale *= 0.95f;
				return;
			}
			iconRect.sizeDelta += new Vector2(0, -3);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (scaleIcon)
			{
				// 20/19 = 1/0.95f - умножаем на обратную дробь, чтобы вернуть изначальный скейл
				iconRect.transform.localScale *= (20f / 19f);
				return;
			}
			iconRect.sizeDelta += new Vector2(0, 3);
			uiManager.PlayButtonClickSound();
		}

		public void LevelSelect()
		{
			uiManager.ShowLevelSelectScreen();
		}

		public void SubmitLevelLoad()
		{
			uiManager.SubmitUiLevelSelector();
		}

		public async Task Test()
		{
			await Task.Delay(1000);
		}

		public void Restart()
		{
			//gm.SetActiveLoadingScreen(true);

			levelManager.SetupLevel();
			levelManager.CurrentLevelData.ResetLevel();
			gameManager.ResumeGame();
		}

		public void Home()
		{
			var task = gameManager.HomeAsync();
		}


		public void NextLevel()
		{
			Utils.SafeExecuteAsync(levelManager.NextLevel);
		}

		public void ResumeGame()
		{
			gameManager.ResumeGame();
		}

		public void Back()
		{
			uiManager.Back();
		}

		public void Pause()
		{
			uiManager.ShowPauseScreen();
		}
	}
}