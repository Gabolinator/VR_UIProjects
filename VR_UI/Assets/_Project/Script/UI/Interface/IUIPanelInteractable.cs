using System;

namespace UI.Interface
{
    public interface IUIPanelInteractable<TInteractable>
    {
        public IUIPanel Panel { get; }
        public Action<IUIPanelInteractable<TInteractable>> OnHoverEnter { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnHoverExit { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnSelectEnter { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnSelectExit { get; set; }
        public TInteractable Interactable { get; }

        public IInteractableService<IUIPanelInteractable<TInteractable>,TInteractable> InteractableService { get; set; }
        bool IsInteractable { get; set; }
        public void SetInteractable(bool interactable);
        
        
    }
}