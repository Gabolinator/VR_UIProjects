using System;
using System.Collections.Generic;
using TMPro;
using UI.Interface;
using UnityEngine;

namespace UI
{
    public class UIInputService : IInputFieldService<TMP_InputField>
    {
        public IUIKeyboardService UIKeyboardService { get; set; } 
        public void Initialize(IUIKeyboardService.UIKeyboardPreference keyboardService)
        {
            Debug.Log($"Initializing ui keyboard service");
            UIKeyboardService = new UIKeyboardService(keyboardService);
        }

        public void SetInputFieldValue(TMP_InputField field, string value)
        {
           //
        }

        public void AddListeners(TMP_InputField field, Action method)
        {
          //
        }

        public void AddListeners(List<Action> methods)
        {
           //
        }

        public void RemoveListeners()
        {
            //
        }

        public void RemoveListeners(TMP_InputField field)
        {
           //
        }
    }
}