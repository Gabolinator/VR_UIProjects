using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;

namespace _Project.Script.Utility
{
    public static class UtilitiesLibrary
    {
        public static async UniTask DelayedMethod(float delay, Action action )
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
            action?.Invoke();
        }
        
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }
        
        
        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            T randomValue = (T)values.GetValue(random.Next(values.Length));
            return randomValue;
        }
        
        
        public static string Print(List<string> list)
        {
            if (list.IsNullOrEmpty()) return "[]";
            var s = "[";
            int i = 0;
            foreach (var element in list)
            {
                s += element;
                if (i != list.Count - 1) s += ',';
            }

            s += "]";
            return s;
        }
    }
}