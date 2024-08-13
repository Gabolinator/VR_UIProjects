using System;
using _Project.Script.Managers;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UI.Interface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;


namespace UI
{
    public class UIPanel : MonoBehaviour, 
        IUIPanel, 
        IUIPanelInteractable<XRSimpleInteractable>,
        IUIPanelVisibility<Canvas, SortingGroup, CanvasGroup, Image>, 
        IUIPanelBehaviour, 
        IUIKeyboardSupport
    {
        [SerializeField] private string name;

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name)) name = gameObject.name;
                return name;
            }
            
            set => name = value;
        }

        [FoldoutGroup("Panel Preferences")] [SerializeField]
        private UIBehaviourPreference behaviourPreferences;

        [FoldoutGroup("Panel Preferences")] [SerializeField]
        private IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences visualPreference;

        [FoldoutGroup("Panel Preferences")] [SerializeField]
        private IUIKeyboardService.UIKeyboardPreference uiKeyboardPreference;

        public IUIKeyboardService.UIKeyboardPreference UIKeyboardPreferences { get => uiKeyboardPreference; set => uiKeyboardPreference = value; }
        public IUIKeyboardService KeyboardService { get; set; } = new UIKeyboardService();
        
        public GameObject Go => gameObject;

        public UnityEvent OnShow
        {
            get => visualPreference.onShow;
            set => visualPreference.onShow = value;
        }

        public UnityEvent OnHide
        {
            get => visualPreference.onHide;
            set => visualPreference.onHide = value;
        }

        [SerializeField] private XRSimpleInteractable _interactable;

        public IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences VisualPreferences { get => visualPreference; set => visualPreference = value; }
        public IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image> VisualService { get; } =
            new UIVisualService();

        public bool Visible => VisualService.Visible;
        
        private readonly IUIComponentsEditService<TMP_Text, Button, Toggle, Slider, Dropdown>
            _componentsEditService; //responsible for edtiting visual of ui element 

        private readonly IInputFieldService<TMP_InputField>
            _inputFieldService = new UIInputService(); //responsible for handling only the input fields 

        public IUIPanel Panel => this;
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnHoverEnter
        {
            get => InteractableService.OnHoverEnter;
            set => InteractableService.OnHoverEnter = value;
        }

        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnHoverExit   {
            get => InteractableService.OnHoverExit;
            set => InteractableService.OnHoverExit = value;
        }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnSelectEnter    {
            get => InteractableService.OnSelectEnter;
            set => InteractableService.OnSelectEnter = value;
        }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnSelectExit    {
            get => InteractableService.OnSelectExit;
            set => InteractableService.OnSelectExit = value;
        }

        public XRSimpleInteractable Interactable => _interactable;

        public IInteractableService<IUIPanelInteractable<XRSimpleInteractable>,XRSimpleInteractable> InteractableService { get; set; } =
            new UIInteractableService(); //responsible for handling event of interactable ui elements and panel itself

        public bool IsInteractable { get; set; } = true;


        UIBehaviourPreference IUIPanelBehaviour.BehaviourPreferences
        {
            get => behaviourPreferences;
            set =>behaviourPreferences = value;
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
        
        private UIManager Manager => UIManager.Instance;
        
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
        
        public GameObject RootPrefab { get; set; }
       

        public void Initialize()
        {
            Debug.Log($"Initializing : {Name}");
            
            VisualService.Initialize(visualPreference);
            _inputFieldService.Initialize(uiKeyboardPreference);
            BehaviourService.Initialize(behaviourPreferences);
            InteractableService.Initialize(this, Interactable);

            InteractableService.OnHoverEnter += delegate
            {
                Debug.Log($"{Name} : on hover enter");
            };
            InteractableService.OnHoverExit += delegate
            {
                Debug.Log($"{Name} : on hover exit");
            };
            InteractableService.OnSelectEnter += delegate
            {
                Debug.Log($"{Name} : on select entered");
            };
            InteractableService.OnSelectExit += delegate
            {
                Debug.Log($"{Name} is select exited");
            };
            
            var startState = visualPreference.hideOnStart ? Hide(false, -1, null) :  Show(false, -1, null);

        }

        public void SetInteractable(bool interactable) => InteractableService.SetInteractable(this, interactable);

        
        public UniTask Show(bool fade, float fadeDuration, Action onShowComplete = null)
            => VisualService.ToggleVisibility(true, fade, fadeDuration, onShowComplete);

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

        public UniTask Hide(Action onHideComplete = null)
            => VisualService.ToggleVisibility(false,onHideComplete);
        
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
        
        public void SetBehaviourPreferences(UIBehaviourPreference preferences)
        {
            BehaviourService.Behaviour = behaviourPreferences = preferences;
        }

        public void SetFadePreferences(LerpPreferences lerpInPreferences, LerpPreferences lerpOutPreferences)
        {
            VisualService.MainVisualPreferences.defaultLerpIn = visualPreference.defaultLerpIn = lerpInPreferences;
            VisualService.MainVisualPreferences.defaultLerpOut = visualPreference.defaultLerpOut = lerpOutPreferences;
        }

        public void SetUIKeyboardPreferences(IUIKeyboardService.UIKeyboardPreference preferences)
        {
            _inputFieldService.UIKeyboardService.Preferences = uiKeyboardPreference = preferences;
        }

        public UniTask Close(bool destroy = true, Action onClose = null)
        {
            if(!IsClosable) return new UniTask();;
            if (!destroy) return Hide(onClose);
            
            DestroySelf(onClose);
            return new UniTask();
        }

        public void RegisterToManager(UIManager manager)
        {
            if (manager == null)
            {
                Debug.Log("manager is null");
                return;
            }
            manager.Register(this);
        }

        public void UnRegisterFromManager(UIManager manager)
        {
            if(manager == null) return;
            manager.UnRegister(this);
        }
        
        private void RegisterToContainer(IUIContainer container)
        {
            container?.Register(this);
        }

        private IUIContainer GetParentContainer()
        {
          return transform.parent.GetComponent<IUIContainer>();
        }

        
        private void UnregisterFromContainer()
        {
            if(Container == null) return;
            Container.UnRegister(this);
        }
        
        
        private void RemoveAllListeners()
        {
            OnShow.RemoveAllListeners();
            OnHide.RemoveAllListeners();
            _inputFieldService.RemoveListeners();
        }
        
        public void DestroySelf(Action onDestroy = null)
        {
            Debug.LogWarning($"Destroying self : {Name}");

            UnregisterFromContainer();
            UnRegisterFromManager(Manager);
            RemoveAllListeners();
            onDestroy?.Invoke();
            Destroy(gameObject);
        }

       


        private void Awake()
        {
           Initialize();
        }

        private  void Start()
        {
            RegisterToManager(Manager);
        }


        private void OnEnable()
        {
            _inputFieldService.AddListeners(null);
        }
        
        private void OnDisable()
        {
            _inputFieldService.RemoveListeners();
        }


      
    }
    
}