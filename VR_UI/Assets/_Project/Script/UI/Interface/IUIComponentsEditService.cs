using System.Collections.Generic;
using UnityEngine;

namespace _Project.Script.UI.Interface
{
    public interface IUIComponentsEditService<TText, TButton, TToggle, TDropdown, TSlider>
    {
        public void UpdateText(TText text, string value);

        public void UpdateText(TText  text, float value);

        public void UpdateText(TText  text, int value);
        
        public void SetButtonText(TButton toggle, bool value);

        public void SetTextColor(TText text, Color color);
        
        public void SetToggleText(TToggle toggle, string value);

        public void SetSliderText(TSlider slider,string value);

        public void SetDropDownText(TDropdown dropdown, string value);
        
        public void SetButtonText(TButton text, Color color);
        
        public void SetToggleValue(TToggle toggle, bool value);

        public void SetSliderValue(TSlider slider, float value);

        public void SetDropDownOptionValues(TDropdown dropdown, List<string> values);
        
        public void SetDropDownValue(TDropdown dropdown, int index);

        public void SetButtonColor(TButton text, Color color);
        
    }
}