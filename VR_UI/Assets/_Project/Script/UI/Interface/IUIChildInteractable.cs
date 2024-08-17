using System;
using System.Collections.Generic;

namespace _Project.Script.UI.Interface
{
    public interface IUIChildInteractable<TInteractable>
    {
       
        public IUIPanelInteractable<TInteractable> SelectedChildPanel { get; }
        public int SelectedChildIndex { get; }
        public Action<IUIPanelInteractable<TInteractable>> OnChildHoverEnter { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnChildHoverExit { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnChildSelectEnter { get; set; }
        public Action<IUIPanelInteractable<TInteractable>> OnChildSelectExit { get; set; }

        public List<IUIPanelInteractable<TInteractable>> ChildInteractables { get; set; }
        public bool AllowChildInteractability { get; set; }
        public void SetInteractable(IUIPanelInteractable<TInteractable> childPanel, bool interactable);
        
        public void AddInteractListeners();

        public void RemoveInteractListeners();
        
        public void AddInteractListeners(List<IUIPanelInteractable<TInteractable>> childPanels,
            Action<IUIPanelInteractable<TInteractable>> onSelectEnter,
            Action<IUIPanelInteractable<TInteractable>> onSelectExit,
            Action<IUIPanelInteractable<TInteractable>> onHoverEnter,
            Action<IUIPanelInteractable<TInteractable>> onHoverExit);

        public void RemoveInteractListeners(List<IUIPanelInteractable<TInteractable>> childPanels,
            Action<IUIPanelInteractable<TInteractable>> onSelectEnter,
            Action<IUIPanelInteractable<TInteractable>> onSelectExit,
            Action<IUIPanelInteractable<TInteractable>> onHoverEnter,
            Action<IUIPanelInteractable<TInteractable>> onHoverExit);
    }
}