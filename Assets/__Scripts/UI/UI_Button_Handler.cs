using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Button_Handler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Needed to adjust size. If none given - this gameobject's rectTransform will be taken by default")] RectTransform iconRect;
    [SerializeField] bool scaleIcon;

    private void Awake()
    {
        if (iconRect == null)
        {
            iconRect = GetComponent<RectTransform>();
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
            iconRect.transform.localScale *= (20f / 19f);
            return;
        }
        iconRect.sizeDelta += new Vector2(0, 3);
    }

    public void LevelSelect()
    {
        GameManager.Instance.ShowLevelSelect();
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
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
