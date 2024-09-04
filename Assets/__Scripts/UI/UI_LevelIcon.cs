using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelIcon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelNumText;
    [SerializeField] Image bodyImg;
    [SerializeField] Image starsImg;
    [SerializeField] Sprite lockedSprite, unlockedSprite;
    [SerializeField] Sprite star1, star2, star3;

    public void Init(int stars, int level, bool isOpened)
    {
        bodyImg.sprite = lockedSprite;
        levelNumText.text = "";
        if (isOpened)
        {
            bodyImg.sprite = unlockedSprite;
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
}
