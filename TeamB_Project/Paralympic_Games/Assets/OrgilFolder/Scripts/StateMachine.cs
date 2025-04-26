using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    public class StateHooks
    {
        public Action<T> onEnter;
        public Action<T> onExit;
        public Action onUpdate;
    }

    private Dictionary<T, StateHooks> _states = new();
    private (T current, T previous) previousFrame = (default(T), default(T));

    public StateHooks this[T statename]
    {
        get
        {
            if (!_states.TryGetValue(statename, out StateHooks state))
            {
                state = new StateHooks();
                _states[statename] = state;
            }
            return state;
        }
    }

    public void Update(T currentState, T previousState)
    {
        if (_states.TryGetValue(previousFrame.current, out var current))
        {
            current.onUpdate?.Invoke();
        }

        if (Equals(currentState, previousFrame.current) && Equals(previousState, previousFrame.previous))
            return;

        if (_states.TryGetValue(previousState, out StateHooks previous))
        {
            previous.onExit?.Invoke(currentState);
        }

        if (_states.TryGetValue(currentState, out StateHooks next))
        {
            next.onEnter?.Invoke(previousState);
        }
        previousFrame = (currentState, previousState);
    }
}
