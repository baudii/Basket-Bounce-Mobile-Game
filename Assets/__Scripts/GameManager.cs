using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Google.Android;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]

public class GameManager : MonoBehaviour
{
    public enum State
    {
        InGame,
        Completed,
        GameOver,
        LevelSelect,
        Paused,
        None
    }

    public static GameManager Instance => instance;
    static GameManager instance;

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] UI_LevelCompleted levelCompleteScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] UI_LevelSelector levelSelector;

    [HideInInspector]
    public UnityEvent OnRestart;
    [HideInInspector]
    public UnityEvent OnGameOver;

    State prevState;

    public State state;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
        state = State.InGame;

        levelSelector.Init();
    }
#if UNITY_EDITOR
    [ContextMenu("Clear Prefs")]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

#endif

    public void Back()
    {
        if (prevState == State.None)
        {
            ShowPauseScreen();
            return;
        }

        DisableAll();

        state = prevState;
        prevState = State.None;

        switch (state)
        {
            case State.Completed:
                levelCompleteScreen.gameObject.SetActive(true);
                break;
            case State.GameOver:
                gameOverScreen.SetActive(true);
                break;
            case State.LevelSelect:
                levelSelector.gameObject.SetActive(true);
                break;
            case State.Paused:
                pauseScreen.SetActive(true);
                break;
        }
    }

    public void SetActiveLoadingScreen(bool isActive)
    {
        loadingScreen.SetActive(isActive);
    }

    public void ShowLevelSelect()
    {
        prevState = state;
        state = State.LevelSelect;

        DisableAll();
        levelSelector.gameObject.SetActive(true);
    }

    public void UpdateLevelSelector() => levelSelector.UpdateLevelSelector();

    public void Restart()
    {
        OnRestart?.Invoke();

        ResumeGame();
    }

    public void ShowGameOverScreen()
    {
        if (state != State.InGame)
            return;
        state = State.GameOver;

        OnGameOver?.Invoke();

        gameOverScreen.SetActive(true);
    }

    public void ShowLevelCompleteScreen(int stars)
    {
        if (state != State.InGame)
            return;
        state = State.Completed;

        levelCompleteScreen.gameObject.SetActive(true);
        levelCompleteScreen.ShowStars(stars);
    }

    public void ShowPauseScreen()
    {
        if (state != State.InGame)
            return;
        state = State.Paused;

        pauseScreen.SetActive(true);
    }
    public void ResumeGame()
    {
        DisableAll();

        state = State.InGame;
    }

    void DisableAll()
    {
        loadingScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        levelCompleteScreen.gameObject.SetActive(false);
        pauseScreen.SetActive(false);
        levelSelector.gameObject.SetActive(false);
    }

    public void ShowAd()
    {

    }
}
