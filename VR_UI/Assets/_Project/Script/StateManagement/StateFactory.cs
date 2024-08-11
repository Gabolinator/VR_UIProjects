using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.StateManagement
{
    public static class StateFactory
    {
        public static State CreateState(string stateName, Action onEnterState, Action onUpdateState, Action onExitState, StateMachine stateMachine)
        {
            return new State(stateName, stateMachine)
            {
                OnEnterAsync = async () =>
                {
                    onEnterState?.Invoke();
                    Debug.Log($"Entering {stateName} State");
                    await UniTask.CompletedTask;
                },
                OnUpdateAsync = async () =>
                {
                    onEnterState?.Invoke();
                    Debug.Log($"Updating {stateName} State");
                    await UniTask.CompletedTask;
                },
                OnExitAsync = async () =>
                {
                    // HideUIPanel();
                    Debug.Log($"Exiting {stateName} State");
                    await UniTask.CompletedTask;
                }

            };
        }
    }
}