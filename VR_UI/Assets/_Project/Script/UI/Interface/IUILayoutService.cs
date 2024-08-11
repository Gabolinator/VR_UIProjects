using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace UI.Interface
{

    [Serializable]
    public struct LayoutEvents
    {
        public UnityEvent onStartLayout;
        public UnityEvent onCompletedLayout;
        public UnityEvent onUpdateLayout;
    }

    [Serializable]
    public class LayoutPreference
    {
        public List<IUIPanel> Panels { get; set; }
        public LerpPreferences lerpPrefs;
        public LayoutEvents events;
    }

    public interface IUIContainerLayout
    {
        public LayoutPreference LayoutPreferences { get; set; }
        public void UpdateLayout();
        public void Layout(LayoutPreference preference);
        
        public IUILayoutService LayoutService { get; set; }
    }
    
    public interface IUILayoutService
    {
        
        public LayoutPreference Preferences { get; set; }
        public IUIPanel CentralUIPanel { get; set; }
        int CurrentIndex { get; set; }

        public Action<int> OnStartLayout { get; set; }
        public Action<int> OnFinishLayout { get; set; }
        public Action<float,int> OnLayoutUpdate { get; set; }

        public void Layout(LayoutPreference preference);
        public void Layout(LayoutPreference preference, Action<int> onStart,Action<float,int> onUpdated ,Action<int> onCompleted);
        public void LayoutNow(LayoutPreference preference ,Action<int> onStart = null ,Action<int> onCompleted = null);
        public UniTask LerpLayout(LayoutPreference preference, Action<int> onStart = null ,Action<float,int> onUpdated = null,Action<int> onCompleted = null ,float freq = 0.05f);
        
        public void Layout(int index);
        
        public void Layout();

        public void Initialize(LayoutPreference preference, List<IUIPanel> panels);
        
    }
}