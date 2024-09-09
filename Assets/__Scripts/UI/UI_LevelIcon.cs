using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LevelIcon : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI levelNumText;
    [SerializeField] Image bodyImg;
    [SerializeField] Image starsImg;
    [SerializeField] Sprite lockedSprite, unlockedSprite;
    [SerializeField] Sprite star1, star2, star3;

    int thisLevel;

    public static int SelectedLevel { get; private set; }

    public void Init(int stars, int level, bool isOpened)
    {
        levelNumText.text = "";
        button.interactable = false;
        thisLevel = level;
        if (isOpened)
        {
            levelNumText.text = (level + 1).ToString();
            button.interactable = true;
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
        SelectedLevel = thisLevel;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.Co_DelayedExecute(() => {
            if (SelectedLevel == thisLevel)
            {
                print("-1");
                SelectedLevel = -1;
            }
        }, 1f);
    }
}
