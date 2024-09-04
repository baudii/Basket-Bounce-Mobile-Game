using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Button_Handler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.GetComponent<RectTransform>().sizeDelta += new Vector2(0, -3);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 3);
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
}
