using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UI_LevelSelector : MonoBehaviour
{
    [SerializeField] int levelAmount;
    [SerializeField] UI_LevelIcon levelIconPrefab;
    [SerializeField] Transform gridParent;
    List<UI_LevelIcon> levels;

    public void Init()
    {
        levels = new List<UI_LevelIcon>();
        for (int i = 0; i < levelAmount; i++)
        {
            var levelIcon = Instantiate(levelIconPrefab, gridParent);
            levels.Add(levelIcon);
        }
    }

    public void UpdateLevelSelector()
    {
        int lastLevel = PlayerPrefs.GetInt(LevelManager.LAST_OPENED_LEVEL, 0);

        for (int i = 0; i < levels.Count; i++)
        {
            int stars = PlayerPrefs.GetInt(LevelManager.LEVEL_STARS + i, 0);

            levels[i].Init(stars, i, i <= lastLevel);
        }
    }

    public void LoadLevel()
    {
        int selectedLevel = UI_LevelIcon.SelectedLevel;
        print("trying to load " + selectedLevel);
        if (selectedLevel == -1)
        {
            print("WTF");
            return;
        }
        //gridParent.GetChild(selectedLevel).GetComponent<UI_LevelIcon>().StopAllCoroutines();
        LevelManager.Instance.LoadLevel(selectedLevel);
    }
}
