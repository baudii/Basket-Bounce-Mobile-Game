using System.Collections;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	[Header("State Machine")]
	protected State m_CurrentState;

	public void SetState(State newState)
	{
		bool transitionDenied = m_CurrentState.TransitionDenyCondition.Invoke(newState);
		if (transitionDenied)
		{
			this.SmartLog("Transition was denied.", m_CurrentState, newState);
			return;
		}
		StopAllCoroutines();
		m_CurrentState?.OnExit();
		newState.OnEnter();
		m_CurrentState = newState;

		if (m_CurrentState.CanUpdate)
		{
			StartCoroutine(Co_UpdateState(m_CurrentState.UpdateTime));
		}
	}

	IEnumerator Co_UpdateState(float deltaTime)
	{
		YieldInstruction yieldInstruction = null;

		if (deltaTime > 0) yieldInstruction = new WaitForSeconds(deltaTime);

		while (true)
		{
			m_CurrentState.OnUpdate();
			yield return yieldInstruction;
		}
	}
}
