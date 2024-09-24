using System;

public class State
{
	public delegate void StateEvent();
	event StateEvent _OnEnterEvent;
	event StateEvent _OnExitEvent;
	event StateEvent _OnUpdateEvent;
	public float UpdateTime { get; private set;}
	public bool CanUpdate => UpdateTime >= 0;

	public Func<State, bool> TransitionDenyCondition;

	public State(StateEvent onEnterEvent = null, StateEvent onExitEvent = null)
	{
		_OnEnterEvent = onEnterEvent;
		_OnExitEvent = onExitEvent;
		UpdateTime = -1;
	}

	~State()
	{
		_OnEnterEvent = null;
		_OnExitEvent = null;
		_OnUpdateEvent = null;
		TransitionDenyCondition = null;
	}

	

	/// <summary> Set update event </summary>
	/// <param name="onUpdate">Specify update event</param>
	/// <param name="updateTime">Specify update time. Must be >= 0. NOTE: 0 will set update time to standard unity update tick</param>
	public void SetUpdateEvent(StateEvent onUpdate, float updateTime)
	{
		if (updateTime < 0)
		{
			this.SmartError("Update time can not be less than zero!");
			return;
		}
		_OnUpdateEvent = onUpdate;
		UpdateTime = updateTime;
	}

	public virtual void OnEnter()
	{
		_OnEnterEvent?.Invoke();
	}

	public virtual void OnUpdate()
	{
		_OnUpdateEvent?.Invoke();
	}

	public virtual void OnExit()
	{
		_OnExitEvent?.Invoke();
	}
}
