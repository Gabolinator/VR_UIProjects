using System.Collections.Generic;
using _Project.Script.StateManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Interface
{
    public interface IUIContainer
    {

        public string Name { get; set; }

        public bool Visible { get; }

        public List<UIPanel> ChildPanels { get; set; }

        public List<UIPanel> GetUIPanels();

        public UIPanel GetUIPanelByName(string name);

        public int NumOfPanels { get; }

        public void Initialize();
        
        public UniTask TogglePanel(UIPanel panel, bool state, bool fade, float fadeDuration = -1);
        
        public UniTask TogglePanel(UIPanel panel, bool state);
        
        public UniTask TogglePanel(UIPanel panel, bool state, LerpPreferences preferences);


        public UniTask ToggleRootPanel(bool state);
        //public UniTask ToggleGui(string panelName, bool state, bool fade, float fadeDuration = -1);
        
        void Register(UIPanel panel);

        void UnRegister(UIPanel panel);
        
        void Register(List<UIPanel> panels);
        void UnRegister(List<UIPanel> panels);
    }
}