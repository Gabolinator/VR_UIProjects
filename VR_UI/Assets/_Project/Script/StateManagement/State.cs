using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.StateManagement
{
    /// <summary>
    /// in use with StateMachine - each State has an OnEnter, OnExit, and OnUpdate method
    /// <br/><br/>
    /// for OnUpdate, a normal function can be passed in, or a <see cref="StateManagement.StepRunner"/> can be passed in,
    /// which will run a list of <see cref="Step"/> objects in order before transitioning to the next state
    /// only one can be used at a time, passing in a <see cref="StateManagement.StepRunner"/> will override the normal update function
    /// <br/><br/>
    /// the final step MUST transition to another state or the state machine will hang
    /// </summary>
    public class State : IState
    {
        public string Name { get; set; }
        public Func<UniTask> OnEnterAsync { get; set; }
        public Func<UniTask> OnExitAsync { get; set; }
        public Func<UniTask> OnUpdateAsync { get; set; }
        public StateMachine StateMachine { protected get; set; }

        public State(string name, StateMachine machine, Func<UniTask> onEnterAsync, Func<UniTask> onUpdateAsync,
            Func<UniTask> onExitAsync)
        {
            Name = name;
            StateMachine = machine;
            OnEnterAsync = onEnterAsync;
            OnUpdateAsync = onUpdateAsync;
            OnExitAsync = onExitAsync;
        }

        public State(StateMachine machine, Action onEnter, Action onUpdate, Action onExit,
            string name = "please name this state for debugging purposes")
        {
            Name = name;
            StateMachine = machine;
            if (onEnter is not null)
            {
                OnEnterAsync = async () =>
                {
                    onEnter();
                    await UniTask.CompletedTask;
                };
            }

            if (onUpdate is not null)
            {
                OnUpdateAsync = async () =>
                {
                    onUpdate();
                    await UniTask.CompletedTask;
                };
            }

            if (onExit is not null)
            {
                OnExitAsync = async () =>
                {
                    onExit();
                    await UniTask.CompletedTask;
                };
            }
        }

        public State(string name, StateMachine stateMachine)
        {
            Name = name;
            StateMachine = stateMachine;
        }


        public virtual async UniTask Enter()
        {
            Debug.Log("Entering state: " + Name);
            if (OnEnterAsync is not null)
            {
                await OnEnterAsync();
            }
            else
            {
                Debug.Log("No OnEnter method found for state: " + Name);
            }
        }

        public virtual async UniTask Exit()
        {
            Debug.Log("Exiting state: " + Name);
            if (OnExitAsync is not null)
            {
                await OnExitAsync();
            }
            else
            {
                Debug.Log("No OnExit method found for state: " + Name);
            }
        }

        public virtual void Update()
        {
            Debug.Log("Updating state: " + Name);
            if (OnUpdateAsync is not null)
            {
                OnUpdateAsync().Forget();
            }
            else
            {
                Debug.Log("No OnUpdate method found for state: " + Name);
            }
        }


        public void TransitionToState(IState newState)
        {
            Debug.Log("Transitioning from state " + Name + "  to state: " + newState.Name);
            StateMachine.TransitionToState(newState as State).Forget();
        }
    }

    public interface IState
    {
        UniTask Enter();
        UniTask Exit();
        void Update();
        void TransitionToState(IState newState);
        string Name { get; set; }
    }
}