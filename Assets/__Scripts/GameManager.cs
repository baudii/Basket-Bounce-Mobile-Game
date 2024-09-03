using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    enum State
    {
        InGame,
        Completed,
        GameOver,
        Paused
    }

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject levelCompleteScreen;
    [SerializeField] GameObject pauseScreen;
    public static GameManager Instance => instance;
    static GameManager instance;

    [HideInInspector]
    public UnityEvent OnRestart;
    [HideInInspector]
    public UnityEvent OnGameOver;

    State state;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;

        state = State.InGame;
    }

    public void NextLevel()
    {

    }

    public void SelectLevel()
    {

    }

    public void Restart()
    {
        gameOverScreen.SetActive(false);
        levelCompleteScreen.SetActive(false);
        pauseScreen.SetActive(false);

        OnRestart?.Invoke();
        state = State.InGame;
    }

    public void ShowGameOverScreen()
    {
        if (state != State.InGame)
            return;
        state = State.GameOver;

        OnGameOver?.Invoke();

        gameOverScreen.SetActive(true);
    }

    public void ShowLevelCompleteScreen()
    {
        if (state != State.InGame)
            return;
        state = State.Completed;

        levelCompleteScreen.SetActive(true);
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
        if (state == State.InGame)
            return;
        state = State.InGame;

        pauseScreen.SetActive(false);
    }

    public void ShowAd()
    {

    }
}
