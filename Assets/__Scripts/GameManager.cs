using UnityEngine;
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
    [HideInInspector]
    public UnityEvent OnInGameStateEnter;
    [HideInInspector]
    public UnityEvent OnInGameStateExit;
    State prevState;

    public State currentState;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
        currentState = State.InGame;

        levelSelector.Init();

#if UNITY_IOS

        Application.targetFrameRate = 120;

#endif

    }
#if UNITY_EDITOR
    [ContextMenu("Clear Prefs")]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

#endif

    void SetState(State state)
    {
        this.SmartLog("Current state: " + currentState + ". New state: " + state);
        OnExitState(currentState);

        switch (state)
        {
            case State.InGame:
                DisableAll();
                Time.timeScale = 1;
                OnInGameStateEnter?.Invoke();
                break;
            case State.Completed:
                levelCompleteScreen.gameObject.SetActive(true);
                break;
            case State.GameOver:
                if (currentState != State.InGame)
                    return;
                gameOverScreen.SetActive(true);
                OnGameOver?.Invoke();
                break;
            case State.LevelSelect:
                levelSelector.gameObject.SetActive(true);
                break;
            case State.Paused:
                pauseScreen.SetActive(true);
                break;
        }

        prevState = currentState;
        currentState = state;
    }

    void OnExitState(State state)
    {
        switch (state)
        {
            case State.InGame:
                Time.timeScale = 0;
                OnInGameStateExit?.Invoke();
                break;
            case State.Completed:
                levelCompleteScreen.gameObject.SetActive(false);
                break;
            case State.GameOver:
                gameOverScreen.SetActive(false);
                break;
            case State.LevelSelect:
                levelSelector.gameObject.SetActive(false);
                break;
            case State.Paused:
                pauseScreen.SetActive(false);
                break;
        }
    }

    public void Back()
    {
        if (prevState == State.None)
        {
            ShowPauseScreen();
            return;
        }

        DisableAll();

        currentState = prevState;
        prevState = State.None;

        switch (currentState)
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

    public void GameOver()
    {
        SetState(State.GameOver);
    }

    public void SetActiveLoadingScreen(bool isActive)
    {
        loadingScreen.SetActive(isActive);
    }

    public void ShowLevelSelect()
    {
        prevState = currentState;
        currentState = State.LevelSelect;

        DisableAll();
        levelSelector.gameObject.SetActive(true);
    }

    public void UpdateLevelSelector() => levelSelector.UpdateLevelSelector();

    public void Restart()
    {
        OnRestart?.Invoke();

        ResumeGame();
    }

    public void ShowLevelCompleteScreen(int stars)
    {
        SetState(State.Completed);
        levelCompleteScreen.SetStars(stars);
    }

    public void ShowPauseScreen()
    {
        SetState(State.Paused);
    }
    public void ResumeGame()
    {
        SetState(State.InGame);
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
