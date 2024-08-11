using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.StateManagement
{
    public class StepRunner
    {
        private List<Step> _steps;

        public StepRunner(Action steps, string description)
        {
            _steps = new List<Step>
            {
                new Step(async () =>
                {
                    steps();
                    await UniTask.CompletedTask;
                }, description)
            };
        }

        public async UniTask RunAsync()
        {
            foreach (var step in _steps)
            {
                Debug.Log("Running step: " + step.Description);
                await step.Execute();
            }
        }

        public Step AsStep(string description)
        {
            return new Step(async () => await RunAsync(), description);
        }
    }
}