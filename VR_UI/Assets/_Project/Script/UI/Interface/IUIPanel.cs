using System;
using _Project.Script.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Interface
{
    public interface IUIPanel
    {
        public string Name { get; set; }

        public GameObject Go { get;}
        
        void Initialize();
        
        public UniTask Close(bool destroy = true, Action onClose = null);

        public void RegisterToManager(UIManager manager);
        public void UnRegisterFromManager(UIManager manager);

        public void DestroySelf(Action onDestroy = null);
        
        
    }
}