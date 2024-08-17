using System;
using System.Collections.Generic;

namespace _Project.Script.UI.Interface
{
    public interface IInputFieldService<TInputField>
    {
        IUIKeyboardService UIKeyboardService { get; set; }
        
        
        public void Initialize(IUIKeyboardService.UIKeyboardPreference keyboardService);
        
        public void SetInputFieldValue(TInputField field, string value);
        
        void AddListeners(TInputField field, Action method);

        void AddListeners(List<Action> methods);
        void RemoveListeners();
        
        void RemoveListeners(TInputField field);
    }
}