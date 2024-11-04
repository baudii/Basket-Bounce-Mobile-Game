using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KK.Common;
namespace BasketBounce.UI
{
	public class UI_LevelIcon : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		[SerializeField] Button button;
		[SerializeField] TextMeshProUGUI levelNumText;
		[SerializeField] Image bodyImg;
		[SerializeField] Image starsImg;
		[SerializeField] GameObject currentLevelBorder;
		[SerializeField] Sprite lockedSprite, unlockedSprite, selectedSprite;
		[SerializeField] Sprite star1, star2, star3;

		int thisLevel;
		bool isCurrentLevel;

		public static int SelectedLevel { get; private set; } = -1;

		public void UpdateCell(int stars, int level, bool isOpened, bool isCurrentLevel)
		{
			levelNumText.text = "";
			this.isCurrentLevel = isCurrentLevel;

			if (isOpened)
			{
				bodyImg.sprite = unlockedSprite;
				button.interactable = true;
				currentLevelBorder.SetActive(isCurrentLevel);
			}
			else
			{
				bodyImg.sprite = lockedSprite;
				button.interactable = false;
				currentLevelBorder.SetActive(false);
			}

			thisLevel = level;
			if (isOpened)
			{
				levelNumText.text = (level + 1).ToString();
			}

			starsImg.enabled = true;

			if (stars < 1)
				starsImg.enabled = false;
			else if (stars == 1)
				starsImg.sprite = star1;
			else if (stars == 2)
				starsImg.sprite = star2;
			else if (stars == 3)
				starsImg.sprite = star3;

		}

		public void OnSelect(BaseEventData eventData)
		{
			if (isCurrentLevel)
			{
				SelectedLevel = -1;
				return;
			}
			SelectedLevel = thisLevel;
			bodyImg.sprite = selectedSprite;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			this.Co_DelayedExecute(() =>
			{
				if (SelectedLevel == thisLevel)
				{
					button.Select();
				}
				else
				{
					bodyImg.sprite = unlockedSprite;
				}
			}, 1);
		}
	}
}