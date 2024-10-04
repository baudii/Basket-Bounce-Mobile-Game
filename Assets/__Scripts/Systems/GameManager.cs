using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CandyCoded.HapticFeedback;

#if UNITY_ANDROID || UNITY_IOS
using CandyCoded;
#endif
[DefaultExecutionOrder(-2)]

public class GameManager : MonoBehaviour
{
	public enum State
	{
		InGame,
		Completed,
		GameOver,
		LevelSelect,
		Paused,
		Finished,
		None
	}

	public static GameManager Instance => instance;
	static GameManager instance;

	[Header("References")]
	[SerializeField] GameObject gameOverScreen;
	[SerializeField] UI_LevelCompleted levelCompleteScreen;
	[SerializeField] UI_PauseController pauseScreen;
	[SerializeField] GameObject loadingScreen;
	[SerializeField] UI_LevelSelector levelSelectorScreen;
	[SerializeField] UI_GameFinished gameFinishedScreen;
	[SerializeField] Image mainUiBgImage;
	[SerializeField] Image gameCompleteBgImage;
	[SerializeField] AudioSource src;
	[SerializeField] UI_BounceCounter bounceCounter;

	[Header("Assets")]
	[SerializeField] GameAssets_SO gameAssets;
	public GameAssets_SO GameAssets => gameAssets;


	[HideInInspector]
	public UnityEvent OnLevelComplete;	
	[HideInInspector]
	public UnityEvent OnGameOver;	
	[HideInInspector]
	public UnityEvent OnRestart;
	[HideInInspector]
	public UnityEvent OnInGameStateEnter;
	[HideInInspector]
	public UnityEvent OnInGameStateExit;
	
	[HideInInspector]
	public State currentState;
	State prevState;


	InputMaster input;


	private void Awake()
	{
		if (instance != null)
			Destroy(gameObject);

		instance = this;
		currentState = State.InGame;
		Application.targetFrameRate = 120;

		input = new InputMaster();
		input.Enable();
		input.Taps.Tap.performed += ctx => Vibrate();
	}

	private void OnDestroy()
	{
		OnLevelComplete = null;
		OnInGameStateEnter = null;
		OnInGameStateExit = null;
	}

#if UNITY_EDITOR
	[ContextMenu("Clear Prefs")]
	public void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

#endif

	void Vibrate()
	{
		src.Play();
#if (UNITY_IOS || UNITY_ANDROID)
		HapticFeedback.LightFeedback();
#endif
	}

	public void PlayButtonClickSound() => src.PlayOneShot(GameAssets.BlopSound, 0.6f);

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
				bounceCounter.gameObject.SetActive(true);
				break;
			case State.Completed:
				src.PlayOneShot(GameAssets.WinSound, 0.3f);
				levelSelectorScreen.UpdateLevelSelector();
				levelCompleteScreen.gameObject.SetActive(true);
				OnLevelComplete?.Invoke();
				break;
			case State.GameOver:
				if (currentState != State.InGame && currentState != State.LevelSelect)
					return;
				OnGameOver?.Invoke();
				gameOverScreen.SetActive(true);
				break;
			case State.LevelSelect:
				levelSelectorScreen.gameObject.SetActive(true);
				break;
			case State.Paused:
				pauseScreen.gameObject.SetActive(true);
				break;
			case State.Finished:
				gameFinishedScreen.gameObject.SetActive(true);
				gameCompleteBgImage.gameObject.SetActive(true);
				break;
			case State.None:
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
				mainUiBgImage.gameObject.SetActive(true);
				OnInGameStateExit?.Invoke();
				bounceCounter.gameObject.SetActive(false);
				break;
			case State.Completed:
				levelCompleteScreen.gameObject.SetActive(false);
				break;
			case State.GameOver:
				gameOverScreen.SetActive(false);
				break;
			case State.LevelSelect:
				levelSelectorScreen.gameObject.SetActive(false);
				break;
			case State.Paused:
				pauseScreen.gameObject.SetActive(false);
				break;
			case State.Finished:
				gameFinishedScreen.gameObject.SetActive(false);
				break;
			case State.None:
				break;
		}
	}

	public void Back()
	{
		/*        if (prevState == State.None)
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
                        levelSelectorScreen.gameObject.SetActive(true);
                        break;
                    case State.Paused:
                        pauseScreen.SetActive(true);
                        break;
                }*/

		SetState(prevState);
		prevState = State.None;
	}

	public void GameOver()
	{
		if (currentState == State.InGame)
			SetState(State.GameOver);
	}

	public void SetActiveLoadingScreen(bool isActive)
	{
		loadingScreen.SetActive(isActive);
	}

	public void ShowLevelSelect()
	{
		SetState(State.LevelSelect);
	}

	public UI_LevelSelector GetUILevelSelector() => levelSelectorScreen;

	public void UpodateUI()
	{
		levelSelectorScreen.UpdateLevelSelector();
	}

	public void ShowLevelCompleteScreen(int stars)
	{
		SetState(State.Completed);
		levelCompleteScreen.SetStars(stars);
	}

	public void ShowPauseScreen()
	{
		pauseScreen.InitPause();
		SetState(State.Paused);
	}

	public void ShowStuckScreen()
	{
		pauseScreen.InitStuck();
		SetState(State.Paused);
	}

	public void ResumeGame()
	{
		SetState(State.InGame);
	}

	public void FinishGame()
	{
		SetState(State.Finished);
	}

	public void SetFinishTime()
	{

	}

	void DisableAll()
	{
		mainUiBgImage.gameObject.SetActive(false);
		gameCompleteBgImage.gameObject.SetActive(false);
		loadingScreen.SetActive(false);
		gameOverScreen.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);
		pauseScreen.gameObject.SetActive(false);
		levelSelectorScreen.gameObject.SetActive(false);
		gameFinishedScreen.gameObject.SetActive(false);
	}
}
