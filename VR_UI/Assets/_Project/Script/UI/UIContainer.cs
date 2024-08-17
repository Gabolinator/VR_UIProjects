using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Script.Managers;
using _Project.Script.UI.Interface;
using _Project.Script.UI.Services;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace _Project.Script.UI
{
    public class UIContainer : MonoBehaviour,
        IUIContainer, 
        IUIPanel, 
        IUIPanelBehaviour, 
        IUIPanelVisibility<Canvas, SortingGroup, CanvasGroup, Image>, 
        IUIKeyboardSupport, 
        IUIChildInteractable<XRSimpleInteractable>
    {
        [SerializeField] private string name; 
        [FoldoutGroup("Container Preferences")][SerializeField] private IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences visualPreference;
        [FoldoutGroup("Container Preferences")][SerializeField] private UIBehaviourPreference behaviourPreference;
        [FoldoutGroup("Container Preferences")][SerializeField] private bool overrideFadePreferences;
         
        [FormerlySerializedAs("overridenFadeInPreferences")]
        [ShowIf(nameof(overrideFadePreferences))]
        [FoldoutGroup("Container Preferences")][SerializeField] private LerpPreferences overridenFadeInPreferences;

        [FormerlySerializedAs("overridenFadeOutPreferences")]
        [ShowIf(nameof(overrideFadePreferences))]
        [FoldoutGroup("Container Preferences")][SerializeField] private LerpPreferences overridenFadeOutPreferences;

        [FoldoutGroup("Container Preferences")][SerializeField] private bool overrideBehaviourPreferences;

        [ShowIf(nameof(overrideBehaviourPreferences))]
        [FoldoutGroup("Container Preferences")][SerializeField] private UIBehaviourPreference overridenBehaviourPreferences;

        [FoldoutGroup("Container Preferences")][SerializeField] private bool overrideUIKeyboardPreference;

        [ShowIf(nameof(overrideUIKeyboardPreference))]
        [FoldoutGroup("Container Preferences")][SerializeField] private IUIKeyboardService.UIKeyboardPreference overridenUIKeyboardPreference;
        
        [SerializeField] private GameObject _uiContainer;
        public int NumOfPanels => ChildPanels?.Count ?? 0;
        
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name)) name = this.gameObject.name;
                
                return name;
            }
            set => name = value;
        }

        public GameObject Go => this.gameObject;

        public bool Visible => AnyPanelVisible() && VisualService.Visible;

        public IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences VisualPreferences { get => visualPreference; set => visualPreference = value; }
        
        public IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image> VisualService { get; } =
            new UIVisualService();
        public UnityEvent OnShow { get => visualPreference.onShow; set => visualPreference.onShow = value; }
        
        public UnityEvent OnHide { get => visualPreference.onHide; set => visualPreference.onHide = value; }
        
        private UIManager Manager => UIManager.Instance;

        UIBehaviourPreference IUIPanelBehaviour.BehaviourPreferences
        {
            get => behaviourPreference;
            set =>behaviourPreference = value;
        }
        
        

        public IUIBehaviourService BehaviourService { get; } = new UIBehaviourService();

        public GameObject SnapVolume => BehaviourService.SnapVolume;

        public bool IsClosable
        {
            get => BehaviourService.Behaviour.manipPreference.isClosable;
            set => BehaviourService.Behaviour.manipPreference.isClosable = value;
        }

        public bool IsDocked => BehaviourService.Behaviour.manipPreference.CurrentlyDocked;
        public bool IsDockable
        {
            get => BehaviourService.Behaviour.manipPreference.isDockable;
            set => BehaviourService.Behaviour.manipPreference.isDockable = value;
        }
        
        public IUIContainer Container
        {
            get => BehaviourService.Behaviour.manipPreference.DockedIn;
            set {
                if (value == null)
                {
                    BehaviourService.UnDock();
                    return;
                }
                
                BehaviourService.DockIn(value); }
        }


        public IUIPanelInteractable<XRSimpleInteractable> SelectedChildPanel { get; }
        public int SelectedChildIndex { get; }
        
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnChildHoverEnter { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnChildHoverExit { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnChildSelectEnter { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnChildSelectExit { get; set; }
        public List<IUIPanelInteractable<XRSimpleInteractable>> ChildInteractables { get; set; }
        public List<IUIPanel> ChildPanels { get; set; } = new List<IUIPanel>();
        
        
        
        public UIPanel GetUIPanel(GameObject obj)
        {
            return obj.GetComponent<UIPanel>();
        }

        public List<IUIPanel> GetUIPanels()
        {
            var panels = GetComponentsInChildren<UIPanel>().ToList();
          var ipanels =  panels.ConvertAll<IUIPanel> (panel => panel);
          return ipanels;
        }

        public IUIPanel GetUIPanelByName(string name)
        {
            return ChildPanels
                .FirstOrDefault(panel => panel.Name == name);
        }
        
        protected int GetUIPanelIndex(IUIPanel panel)
        {
            return ChildPanels.FindIndex(p => p == panel);
        }
        
        protected IUIPanel GetUIPanelByIndex(int index)
        {
            if (ChildPanels.Count == 0) return null;
            index = Mathf.Min(index, ChildPanels.Count - 1);
            index = Mathf.Max(0, index);
            
            return ChildPanels[index];
        }
        
        private bool AnyPanelVisible()
        {
            foreach (var panel in ChildPanels)
            {
                if (panel.Visible) return true;
            }

            return false;
        }

        public void Initialize()
        {
            VisualService.Initialize(visualPreference);
            BehaviourService.Initialize(behaviourPreference);
            var panels = _uiContainer ? _uiContainer.GetComponentsInChildren<UIPanel>().ToList() : new List<UIPanel>();
            ChildPanels = panels.ConvertAll<IUIPanel>(p => p);
            ChildInteractables = panels.ConvertAll<IUIPanelInteractable<XRSimpleInteractable>>(p=> p);
      
            AddInteractListeners();
            Register(ChildPanels);
            
            OnInitialize();
        }

        protected virtual void ChildSelectExit(IUIPanelInteractable<XRSimpleInteractable> obj)
        {
          //  Debug.Log($"{Name} : child on select exited");
            OnChildSelectExit?.Invoke(obj);
        }

        protected virtual void ChildHoverExit(IUIPanelInteractable<XRSimpleInteractable> obj)
        {
           // Debug.Log($"{Name} : child on  hover exit");
            OnChildHoverExit?.Invoke(obj);
        }

        protected virtual void ChildSelectEnter(IUIPanelInteractable<XRSimpleInteractable> obj)
        {
         //   Debug.Log($"{Name} : child on select entered");
            OnChildSelectEnter?.Invoke(obj);
        }
   
        
        protected virtual void ChildHoverEnter(IUIPanelInteractable<XRSimpleInteractable> obj)
        {
            
          //  Debug.Log($"{Name} : child on hover enter");
            OnChildHoverEnter?.Invoke(obj);
        }

        public UniTask TogglePanel(IUIPanel panel, bool state, bool fade, float fadeDuration = -1)
        {
            if (panel is not UIPanel uiPanel) return new UniTask(); 
            return TogglePanel(uiPanel, state, fade, fadeDuration);
        }

        public UniTask TogglePanel(IUIPanel panel, bool state)
        {
               if (panel is not UIPanel uiPanel) return new UniTask(); 
               return TogglePanel(uiPanel, state);
        }

        public UniTask TogglePanel(IUIPanel panel, bool state, LerpPreferences preferences)
        {
            if (panel is not UIPanel uiPanel) return new UniTask(); 
            return TogglePanel(uiPanel, state, preferences);
        }


        public UniTask Show(bool fade, float fadeDuration, Action onShowComplete = null)
            => ToggleRootPanel(true);

        public UniTask DelayShow(float delay, bool fade , float fadeDuration, Action onShowComplete = null)
            => VisualService.DelayShow(delay, fade, fadeDuration, onShowComplete);

        public UniTask DelayHide(float delay, bool fade, float fadeDuration, Action onHideComplete = null)
            => VisualService.DelayHide(delay, fade, fadeDuration, onHideComplete);

        public UniTask Hide(bool fade, float fadeDuration, Action onHideComplete = null)
        => VisualService.ToggleVisibility(false, fade, fadeDuration, onHideComplete);

        public UniTask Show(LerpPreferences preferences, Action onShowComplete = null)
        =>VisualService.ToggleVisibility(true,preferences,onShowComplete);

        public UniTask Show(bool fade, Action onShowComplete = null)
            =>   VisualService.ToggleVisibility(true, fade, onShowComplete);
        

        public UniTask Show(Action onShowComplete = null)
            => VisualService.ToggleVisibility(true,onShowComplete);

        public UniTask DelayShow(float delay, Action onShowComplete = null)
            => VisualService.DelayShow(delay, onShowComplete);

        public UniTask DelayHide(float delay, Action onHideComplete = null)
            => VisualService.DelayHide(delay, onHideComplete);
        
        
        public UniTask Hide(bool fade, Action onHideComplete = null)
            => VisualService.ToggleVisibility(false,fade,onHideComplete);

        public UniTask Hide(LerpPreferences preferences, Action onShowComplete = null)
            =>VisualService.ToggleVisibility(false,preferences,onShowComplete);

        public UniTask CancelShow() => VisualService.Cancel();

        public UniTask CancelHide() =>VisualService.Cancel();

        public virtual void SetMaxAlpha(float alpha) => VisualService.SetMaxAlpha(alpha);
        
        public virtual void SetAlpha(float alpha, bool fade , float duration)
        {
            if (fade) VisualService.LerpFade(alpha, duration);
            else VisualService.FadeNow(alpha);
        }
        
        public void DockIn(IUIContainer container) => Container = container;
        
        public void UnDock() => Container = null;

        public UniTask Hide(Action onHideComplete = null)
            => ToggleRootPanel(false);
        
        
        public UniTask Close(bool destroy = true, Action onClose = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterToManager(UIManager manager)
        {
           if(manager == null) return;
           manager.Register((IUIPanel)this );
        }

        public UniTask TogglePanel(UIPanel panel, bool state, bool fade, float fadeDuration = -1)
        {
            return TogglePanel(panel, state, new LerpPreferences { shouldLerp = fade, duration = -1, minValue =  0, maxValue = 1});
        }

        public UniTask TogglePanel(UIPanel panel, bool state)
        {
            if (panel == null) return new UniTask();

            if (state) return panel.Show();
            return panel.Hide();
        }
        
     
        
        public UniTask TogglePanel(GameObject panelObj, bool state)
        {
           return TogglePanel(panelObj.GetComponent<UIPanel>(), state);
        }
        
        public UniTask TogglePanel(GameObject panelObj, bool state, bool fade)
        {
            return TogglePanel(panelObj.GetComponent<UIPanel>(), state, fade);
        }

        public UniTask TogglePanel(UIPanel panel, bool state, bool fade)
        {
            if (panel == null) return new UniTask();

            if (state) return panel.Show(fade);
            return panel.Hide(fade);
        }
        
        public UniTask TogglePanel(UIPanel panel, bool state, LerpPreferences preferences)
        {
            if (panel == null) return default;

            if (state) return panel.Show(preferences);
            return panel.Hide(preferences);
        }

        public async UniTask ToggleRootPanel(bool state)
        =>VisualService.ToggleVisibility(state, null);


      

        public async void ClosePanel(UIPanel panel)
        {
            if(panel == null) return;
            
            await panel.Close();

            OnClosePanel();
        }

        protected virtual void OnClosePanel()
        {
            
        }


        public void Register(IUIPanel panel)
        {
           if(panel == null) return;

           if (ChildPanels.Any(p => p == panel)) return;
           
           ChildPanels.Add(panel);
           OverridePreferences(panel);
           if(panel is not IUIPanelBehaviour panelBehaviour) return;
           panelBehaviour.DockIn(this);
        }

        public void UnRegister(IUIPanel panel)
        {
            throw new NotImplementedException();
        }

        private void OverridePreferences(IUIPanel iPanel)
        {
            if(iPanel == null) return;
            if(iPanel is not UIPanel panel) return;
            
            if (overrideBehaviourPreferences) panel.SetBehaviourPreferences(overridenBehaviourPreferences);
            if (overrideFadePreferences) panel.SetFadePreferences(overridenFadeInPreferences, overridenFadeOutPreferences );
            if (overrideUIKeyboardPreference) panel.SetUIKeyboardPreferences(overridenUIKeyboardPreference);
            
        }
        
        public void SetMaxGuiAlpha(GameObject gui, float alpha, bool fade = true)
        {
            if (!gui) return;

            //Debug.Log("Setting alpha");
            var guiLogic = gui.GetComponent<UIPanel>();

            if (!guiLogic) return;
            guiLogic.SetMaxAlpha(alpha);
            guiLogic.SetAlpha(alpha, fade, .2f);

        }

        public void UnRegister(UIPanel panel)
        {
            if(panel == null) return;
            if (ChildPanels.Any(p => p == panel))
            {
                ChildPanels.Remove(panel);
                panel.UnDock();
            }
        }
        
        public void UnRegisterFromManager(UIManager manager)
        {
            if(manager == null) return;
            foreach (var panel in ChildPanels)
            {
                panel.UnRegisterFromManager(manager);    
            }
            
            manager.UnRegister(this);
        }

        public void Register(List<IUIPanel> panels)
        {
            foreach (var panel in panels)
            {
               Register(panel);
            }
        }

        public void UnRegister(List<IUIPanel> panels)
        {
            throw new NotImplementedException();
        }

        public void UnRegister(List<UIPanel> panels)
        {
            foreach (var panel in panels)
            {
                UnRegister(panel);
            }
        }

        public void DestroySelf(Action onDestroy = null)
        {
            Debug.LogWarning($"Destroying self : {Name}");

            
            UnRegisterFromManager(Manager);
          
            onDestroy?.Invoke();
            Destroy(gameObject);
        }

        protected virtual void OnInitialize()
        {
            
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
            
        }


        private void Awake()
        {
            Initialize();
            
            OnAwake();
        }
        
        private void Start()
        {
            RegisterToManager(Manager);
            ToggleRootPanel(!visualPreference.hideOnStart);
            OnStart();
        }

        public IUIKeyboardService.UIKeyboardPreference UIKeyboardPreferences { get; set; }
        public IUIKeyboardService KeyboardService { get; set; }
        //public IUIPanel SelectedUIPanel { get; }
        //public int SelectedIndex { get; }
        //public Action<IUIPanelInteractable<XRSimpleInteractable>> OnSelectedPanel { get; set; }
        //public IInteractableService<IUIPanelInteractable<XRSimpleInteractable>,XRSimpleInteractable> InteractableService { get; set; } = new UIInteractableService();
        //public bool IsInteractable { get; set; }
        public bool AllowChildInteractability { get; set; }

        public void SetInteractable(IUIPanelInteractable<XRSimpleInteractable> childPanel, bool interactable)
        {
            if(childPanel == null) return;
            childPanel.SetInteractable(interactable);
        }

        public void AddInteractListeners() 
            => AddInteractListeners(ChildInteractables, ChildSelectEnter, 
                ChildSelectExit, ChildHoverEnter, ChildHoverExit);
        
        

        public void RemoveInteractListeners()
        {
            throw new NotImplementedException();
        }
        

        public void AddInteractListeners(List<IUIPanelInteractable<XRSimpleInteractable>> childPanels, 
            Action<IUIPanelInteractable<XRSimpleInteractable>> onSelectEnter,
            Action<IUIPanelInteractable<XRSimpleInteractable>> onSelectExit,
            Action<IUIPanelInteractable<XRSimpleInteractable>> onHoverEnter,
            Action<IUIPanelInteractable<XRSimpleInteractable>> onHoverExit)
        {
            if(childPanels.Count == 0) return;
            
            foreach (var panel in childPanels)
            {
                if(panel is not IUIPanelInteractable<XRSimpleInteractable> interactable) continue;
                interactable.OnHoverEnter += onHoverEnter;
                interactable.OnHoverExit += onHoverExit;
                interactable.OnSelectEnter += onSelectEnter;
                interactable.OnSelectExit += onSelectExit;
            }
        }

        public void RemoveInteractListeners(List<IUIPanelInteractable<XRSimpleInteractable>> childPanels, Action<IUIPanelInteractable<XRSimpleInteractable>> onSelectEnter, Action<IUIPanelInteractable<XRSimpleInteractable>> onSelectExit, Action<IUIPanelInteractable<XRSimpleInteractable>> onHoverEnter,
            Action<IUIPanelInteractable<XRSimpleInteractable>> onHoverExit)
        {
            if(childPanels.Count == 0) return;
            foreach (var panel in childPanels)
            {
                if(panel is not IUIPanelInteractable<XRSimpleInteractable> interactable) continue;
                interactable.OnHoverEnter -= onHoverEnter;
                interactable.OnHoverExit -= onHoverExit;
                interactable.OnSelectEnter -= onSelectEnter;
                interactable.OnSelectExit -= onSelectExit;
            }
        }
    }


}