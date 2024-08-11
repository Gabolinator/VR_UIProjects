using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.StateManagement
{
    public class StateMachine
    {
        private State _currentState;

        public async UniTask Initialize(State initialState)
        {
            Debug.Log("Initializing state machine with state: " + initialState.Name);
            _currentState = initialState;
            await RunStateMachine();
        }

        private async UniTask RunStateMachine()
        {
            if (_currentState != null)
            {
                Debug.Log("Entering state: " + _currentState.Name);
                await _currentState.Enter();
            }
        }

        public async UniTask TransitionToState(State newState)
        {
            if (_currentState != null)
            {
                Debug.Log("Exiting state: " + _currentState.Name);
                await _currentState.Exit();
            }

            _currentState = newState;
            if (_currentState != null)
            {
                Debug.Log("Entering state: " + _currentState.Name);
                await _currentState.Enter();
            }
        }

        public void Update()
        {
            if (_currentState != null)
            {
                Debug.Log("Updating state: " + _currentState.GetType().Name);
                _currentState.Update();
            }
        }
    }
}