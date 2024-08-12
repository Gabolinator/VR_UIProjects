using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace UI.Interface
{
    public interface IInteractableService<TInteractable, TInteractableComponent>
    {
        TInteractable Interactable { get; set; }
        TInteractableComponent InteractableComponent { get; set; }
        public Action<TInteractable> OnHoverEnter { get; set; }
        public Action<TInteractable> OnHoverExit { get; set; }
        public Action<TInteractable> OnSelectEnter { get; set; }
        public Action<TInteractable> OnSelectExit { get; set; }

        void AddListeners();
        void RemoveListeners();
        
        void AddListeners(TInteractableComponent interactableComponent);
        void RemoveListeners(TInteractableComponent interactableComponent);
        void SetInteractable(TInteractableComponent interactable,  bool state);
        
        void SetInteractable(TInteractable interactable,  bool state);
        void Initialize(TInteractable interatable, TInteractableComponent component);
    }
}