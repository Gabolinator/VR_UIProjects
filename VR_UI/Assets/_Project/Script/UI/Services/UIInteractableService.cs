using System;
using _Project.Script.UI.Interface;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace _Project.Script.UI.Services
{
    public class UIInteractableService : IInteractableService<IUIPanelInteractable<XRSimpleInteractable>, XRSimpleInteractable>
    {
        public IUIPanelInteractable<XRSimpleInteractable> Interactable { get; set; }
        public XRSimpleInteractable InteractableComponent { get; set; }
        
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnHoverEnter { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnHoverExit { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnSelectEnter { get; set; }
        public Action<IUIPanelInteractable<XRSimpleInteractable>> OnSelectExit { get; set; }


        public void AddListeners() => AddListeners(InteractableComponent);

        public void RemoveListeners() => RemoveListeners(InteractableComponent);
        
        public void AddListeners(XRSimpleInteractable interactableComponent)
        {
          if(interactableComponent == null) return;
          Debug.Log($"Adding listeners to :  {interactableComponent.name}");
          interactableComponent.hoverEntered.AddListener(HoverEntered);
          interactableComponent.hoverExited.AddListener(HoverExited);
          interactableComponent.selectEntered.AddListener(SelectEntered);
          interactableComponent.selectExited.AddListener(SelectExited);
        }

        private void SelectExited(SelectExitEventArgs arg0)
        {
          //  Debug.Log($" on select exit");
            OnSelectExit?.Invoke(Interactable);
            EventBus.OnPanelSelectExit?.Invoke(Interactable);
        }

        private void SelectEntered(SelectEnterEventArgs arg0)
        {
           // Debug.Log($" on select enter");
            OnSelectEnter?.Invoke(Interactable);
            EventBus.OnPanelSelectEnter?.Invoke(Interactable);
        }

        private void HoverExited(HoverExitEventArgs arg0)
        {
          //  Debug.Log($" on hover exit");
            OnHoverExit?.Invoke(Interactable);
            EventBus.OnPanelHoverExit?.Invoke(Interactable);
        }

        private void HoverEntered(HoverEnterEventArgs arg0)
        {
          //  Debug.Log($" on hover enter");
            OnHoverEnter?.Invoke(Interactable);
            EventBus.OnPanelHoverEnter?.Invoke(Interactable);
        }

        public void RemoveListeners(XRSimpleInteractable interactableComponent)
        {
            if(interactableComponent == null) return;
            interactableComponent.hoverEntered.RemoveListener(HoverEntered);
            interactableComponent.hoverExited.RemoveListener(HoverExited);
            interactableComponent.selectEntered.RemoveListener(SelectEntered);
            interactableComponent.selectExited.RemoveListener(SelectExited);
        }

        public void SetInteractable(XRSimpleInteractable interactable, bool state)
        {
            if (interactable == null) return;
            interactable.enabled = state;
            
            if(state) AddListeners(interactable);
            else RemoveListeners(interactable);
        }
        
        public void SetInteractable(IUIPanelInteractable<XRSimpleInteractable> interactablePanel, bool state)
        {
            if (interactablePanel is IUIPanel panel)
            {
                if(interactablePanel.IsInteractable == state) return;
                interactablePanel.IsInteractable = state;
                Debug.Log($"{panel.Name} interactability set to {state}");
                SetInteractable(interactablePanel.Interactable, state);
            }
        }

        public void Initialize(IUIPanelInteractable<XRSimpleInteractable> interatable, XRSimpleInteractable interactableComponent)
        {
            if(interatable == null) Debug.LogWarning("interactable panel is null ");
            Interactable = interatable;
            if(interactableComponent == null) Debug.LogWarning("interactable component is null ");
            InteractableComponent = interactableComponent;
            AddListeners();
        }

     
    }
}