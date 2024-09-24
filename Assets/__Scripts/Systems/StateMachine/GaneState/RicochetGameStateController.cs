using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RicochetGameStateController : GameStateManager
{

	public State.StateEvent m_OnLevelSelectStateEnter;
	public State.StateEvent m_OnLevelSelectStateExit;

	public State LevelSelect;
	protected override void Start()
	{
		base.Start();

		LevelSelect = new State(m_OnLevelSelectStateEnter, m_OnLevelSelectStateExit);

		GameOver.TransitionDenyCondition += (state) =>
		{
			return !(m_CurrentState != InGame && m_CurrentState != LevelSelect);
		};

		SetState(InGame);
	}


}
