using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button_Handler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField, Tooltip("Needed to adjust size. If none given - this gameobject's rectTransform will be taken by default")] RectTransform iconRect;
	[SerializeField] bool scaleIcon;

	private void Awake()
	{
		if (iconRect == null)
		{
			iconRect = (RectTransform)transform;
		}
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
		GameManager.Instance.PlayButtonClickSound();
	}

	public void LevelSelect()
	{
		GameManager.Instance.ShowLevelSelect();
	}

	public void SubmitLevelLoad()
	{
		GameManager.Instance.GetUILevelSelector().LoadLevel();
	}

	public void Restart()
	{
		var gm = GameManager.Instance;
		//gm.SetActiveLoadingScreen(true);

		LevelManager.Instance.SetupLevel();
		LevelManager.Instance.CurrentLevelData.ResetLevel();
		gm.SetActiveLoadingScreen(false);
		gm.ResumeGame();

		// в будущем добавить лоадинг скрин в момент рестарта
		/*		gm.Co_DelayedExecute(() =>
				{
				}, 0.3f);*/
	}

	public void NextLevel()
	{
		LevelManager.Instance.NextLevel();
	}

	public void ResumeGame()
	{
		GameManager.Instance.ResumeGame();
	}

	public void Back()
	{
		GameManager.Instance.Back();
	}

	public void Pause()
	{
		GameManager.Instance.ShowPauseScreen();
	}
}
