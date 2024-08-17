﻿using System;
using UnityEngine;

namespace _Project.Script.UI.Interface
{
    public interface IUIKeyboardService
    {
        [Serializable]
        public struct UIKeyboardPreference
        {
            public GameObject keyBoardMount;
        }

        public UIKeyboardPreference Preferences { get; set; }

        public void Initialize(UIKeyboardPreference prefs);


    }
}