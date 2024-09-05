using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Finish : MonoBehaviour
{

    [SerializeField] UnityEvent OnFinish;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Ball circle))
        {
            if (circle.finished)
                return;

            int bounces = circle.OnFinish();
            OnFinish?.Invoke();

            LevelData levelData = LevelManager.Instance.CurrentLevelData;
            int stars = levelData.ConvertToStars(bounces);

            LevelManager.Instance.SaveProgress(stars);
            
            GameManager.Instance.UpdateLevelSelector();
            GameManager.Instance.ShowLevelCompleteScreen(stars);
        }
    }
}
