
using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Script.UI.Interface;
using UnityEngine;
using Utils;

namespace _Project.Script.Managers
{
    public class UIManager : MonoBehaviour
    {
        private List<IUIPanel> panels = new List<IUIPanel>();
        private List<IUIContainer> containers = new List<IUIContainer>();
        
        static UIManager _instance;

        public static UIManager Instance => _instance;

        public void Register(IUIPanel panel)
        {
            if(panel == null) return;

            if (panel is IUIContainer container)
            {
                if (containers.Any(c => c == panel)) return;
                Debug.Log($"Registered container to manager: {panel.Name}");
                containers.Add(container);
                return;
            }
            
            Debug.Log($"Registered panel to manager: {panel.Name}");
            if (panels.Any(p => p == panel)) return;
            panels.Add(panel);
        }
        
        public void UnRegister(IUIPanel panel)
        {
            if(panel == null) return;
            if (panel is IUIContainer container)
            {
                if (containers.Any(c => c == container))
                {
                    Debug.Log($"UnRegistered container from manager: {container.Name}");
                    containers.Remove(container);
                }
                
                return;
            }

            if (panels.Any(p => p == panel))
            {
                Debug.Log($"UnRegistered panel from manager: {panel.Name}");
                panels.Remove(panel);
            }
        }

        private void Awake()
        {
            _instance = this;
        }
    }
  
}