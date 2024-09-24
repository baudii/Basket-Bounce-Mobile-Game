using UnityEngine;

public class GameStateManager : StateMachine
{
	public static GameStateManager Instance => instance;
	static GameStateManager instance;

	// Basic states that most likely will be used in any game
	public State InGame, GameOver, LevelComplete, Pause, Loading, GameFinished;

	#region State Events
	public State.StateEvent m_OnIngameStateEnter;
	public State.StateEvent m_OnIngameStateExit;

	public State.StateEvent m_OnGameOverStateEnter;
	public State.StateEvent m_OnGameOverStateExit;
	
	public State.StateEvent m_OnLevelCompleteStateEnter;
	public State.StateEvent m_OnLevelCompleteStateExit;
	
	public State.StateEvent m_OnPauseStateEnter;
	public State.StateEvent m_OnPauseStateExit;
	
	public State.StateEvent m_OnGameFinishStateEnter;
	public State.StateEvent m_OnGameFinishStateExit;

	public State.StateEvent m_OnLoadingStateEnter;
	public State.StateEvent m_OnLoadingStateExit;
	#endregion

	protected virtual void Start()
	{
		if (instance != null)
			Destroy(gameObject);
		instance = this;

		InGame = new State(m_OnIngameStateEnter, m_OnIngameStateExit);
		GameOver = new State(m_OnGameOverStateEnter, m_OnGameOverStateExit);
		LevelComplete = new State(m_OnLevelCompleteStateEnter, m_OnLevelCompleteStateExit);
		GameFinished = new State(m_OnGameFinishStateEnter, m_OnGameFinishStateExit);
		Pause = new State(m_OnPauseStateEnter, m_OnPauseStateExit);
		Loading = new State(m_OnLoadingStateEnter, m_OnLoadingStateExit);
	}
}
