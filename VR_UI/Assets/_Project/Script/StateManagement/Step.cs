using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.StateManagement
{
    public class Step
    {
        public Func<UniTask> ExecuteAsync { get; set; }

        // public Action ExecuteSync { get; set; }
        public string Description { get; set; }
        public List<Step> NestedSteps { get; private set; }

        public Step(Func<UniTask> executeAsync, string description, params Step[] nestedSteps)
        {
            ExecuteAsync = executeAsync;
            Description = description;
            NestedSteps = new List<Step>(nestedSteps);
        }

        /// <summary>
        /// If you need a synchronous line of code to run that depends on an async method, put it all in one step
        /// </summary>
        public async UniTask Execute()
        {
            Debug.Log("Executing step: " + Description);
            try
            {
                if (ExecuteAsync is not null)
                {
                    await ExecuteAsync();
                }

                foreach (var nestedStep in NestedSteps)
                {
                    await nestedStep.Execute();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing step '{Description}': {ex}");
            }
        }
    }
}