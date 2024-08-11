using System;
using System.Collections.Generic;
using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInteractableService : IInteractableService<Selectable>
    {
        public void OnSelectListeners(Selectable field, Action method)
        {
           //
        }

        public void AddOnSelectListeners(List<Action> methods)
        {
            //
        }

        public void RemoveOnSelect()
        {
           //
        }

        public void RemoveOnSelect(Selectable field)
        {
            //
        }

        public void SetInteractable(IUIPanelInteractable<Selectable> interactablePanel, bool state)
        {
            if (interactablePanel is IUIPanel panel)
            {
                if(interactablePanel.IsInteractable == state) return;
                interactablePanel.IsInteractable = state;
                Debug.Log($"{panel.Name} interactability set to {state}");
            }
        }

       
    }
}