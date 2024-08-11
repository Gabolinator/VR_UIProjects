using System;
using System.Collections.Generic;

namespace UI.Interface
{
    public interface IInteractableService<TInteractable>
    {
        void OnSelectListeners(TInteractable field, Action method);

        void AddOnSelectListeners(List<Action> methods);
        
        void RemoveOnSelect();
        
        void RemoveOnSelect(TInteractable field);
        void SetInteractable(IUIPanelInteractable<TInteractable> interactablePanel,  bool state);
    }
}