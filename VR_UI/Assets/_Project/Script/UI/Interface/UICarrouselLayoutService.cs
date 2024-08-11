using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Interface
{

    public enum Axis
    {
        Up,
        Side,
        Front
    }
    
    [Serializable]
    public class CarrouselLayoutPreferences : LayoutPreference
    {
        
        
        public int frontIndex; // central ui index
        //public int indexToShow=1; //showing index on each side of the central index
        public float startOffset; //add an offset to the circle 
        public float radius = 1; //radius of carrousel
        //public float neighbourGuiAlpha = .3f; //alpha ui on each side of central index
        public Axis axis = Axis.Side; //avis around with the caroussel will be layed out
        [ReadOnly] public float destinationAngle;
        [ReadOnly] public float currentAngle;
    }

    public class UICarrouselLayoutService : IUILayoutService
    {

        private float _anglesPerPanel;
        private float _startOffset;
        private float _currentAngle;
        private float _startCentralPanelOffset;
        public int PanelCount => Preferences != null ? Preferences.Panels.Count : 0;
      
        public LayoutPreference Preferences { get; set; }
        public IUIPanel CentralUIPanel { get; set; }
        
        public int CurrentIndex { get; set; }
        
        public Action<int> OnStartLayout { get; set; }
        public Action<int> OnFinishLayout { get; set; }
        public Action<float,int> OnLayoutUpdate { get; set; }

        public void Layout(LayoutPreference preference) =>
            Layout(preference, OnStartLayout, OnLayoutUpdate, OnFinishLayout);
     

        public void Layout(LayoutPreference preference, Action<int> onStart,Action<float,int> onUpdated,Action<int> onCompleted)
        {
            var carrouselPref = preference as CarrouselLayoutPreferences;
            if (carrouselPref == null)
            {
                Debug.LogWarning("Not valid preferences provided");
                return;
            }
            
            if (preference.Panels == null || preference.Panels.Count == 0)
            {
                Debug.LogWarning("No panels to layout");
                return;
            }
            
            carrouselPref.destinationAngle =GetIndexAngle(carrouselPref.frontIndex);
            
            
            if (!preference.lerpPrefs.shouldLerp)
            {
                LayoutNow(carrouselPref, onStart, onCompleted);
                return;
            }
         

            LerpLayout(carrouselPref, onStart,onUpdated,onCompleted).Forget();
            
        }
        

        public void LayoutNow(LayoutPreference preference ,Action<int> onStart = null ,Action<int> onCompleted = null)
        {
            var carrouselPrefs = preference as CarrouselLayoutPreferences;
            if (carrouselPrefs == null)
            {
                Debug.LogWarning("Not valid preferences provided");
                return;
            }
            
            if (preference.Panels == null || preference.Panels.Count == 0)
            {
                Debug.LogWarning("No panels to layout");
                return;
            }
            
            onStart?.Invoke(CurrentIndex);
            
            // Layout each panel in the circle
            int i = 0;
            foreach (var panel in preference.Panels)
            {
                // Calculate the angle for the current panel by adding the centralPanelOffset to the regular angle step
                float destinationAngle = carrouselPrefs.destinationAngle  + (_anglesPerPanel * i++);
                LayoutNow(panel.Go, destinationAngle, carrouselPrefs.radius, carrouselPrefs.axis);
            }

            carrouselPrefs.currentAngle = carrouselPrefs.destinationAngle;
            
            int wrappedIndex = (carrouselPrefs.frontIndex + PanelCount) % PanelCount;
            CurrentIndex = wrappedIndex;
            CentralUIPanel = carrouselPrefs.Panels[CurrentIndex];
            
            onCompleted?.Invoke(CurrentIndex);

            
        }

        public void Layout(int index)
        {
            int wrappedIndex = (index + PanelCount) % PanelCount;
            
            var carrouselPrefs = Preferences as CarrouselLayoutPreferences;
            if (carrouselPrefs == null)
            {
                Debug.LogWarning("Not valid preferences provided");
                return;
            }

            carrouselPrefs.frontIndex = wrappedIndex;
            Layout();
        }

        public void Layout() => Layout(Preferences, OnStartLayout, OnLayoutUpdate , OnFinishLayout);
        
        private float GetIndexAngle(int index)
        {
            index = PanelCount - index;
            int wrappedIndex = (index + PanelCount) % PanelCount;
           return _startOffset  + (_anglesPerPanel *  wrappedIndex);
        }

        public void Initialize(LayoutPreference preference, List<IUIPanel> panels)
        {
            
            var carrouselPrefs = preference as CarrouselLayoutPreferences;
            if (carrouselPrefs == null)
            {
                Debug.LogWarning("Not valid preferences provided");
                return;
            }
            
            Preferences = carrouselPrefs;
            Preferences.Panels = panels;
            
            _anglesPerPanel = 360f / PanelCount;
            _startOffset = -360f / (PanelCount - 1) - carrouselPrefs.startOffset;
            
            carrouselPrefs.destinationAngle =GetIndexAngle(carrouselPrefs.frontIndex);

            OnStartLayout += delegate { carrouselPrefs.events.onStartLayout?.Invoke();  };
            OnFinishLayout += delegate { carrouselPrefs.events.onCompletedLayout?.Invoke();  };
            OnLayoutUpdate += delegate { carrouselPrefs.events.onUpdateLayout?.Invoke();  };
            
            LayoutNow(carrouselPrefs , null, OnFinishLayout);
        }

        public async UniTask LerpLayout(LayoutPreference preference, Action<int> onStart = null ,Action<float, int> onUpdated = null,Action<int> onCompleted = null ,float freq = 0.05f)
        {
            var carrouselPrefs = preference as CarrouselLayoutPreferences;
            if (carrouselPrefs == null)
            {
                Debug.LogWarning("Not valid preferences provided");
                return;
            }
            
            if (carrouselPrefs.Panels == null ||carrouselPrefs.Panels.Count == 0)
            {
                Debug.LogWarning("No panels to layout");
                return;
            }
            
            var duration = carrouselPrefs.lerpPrefs.duration;
            
            if(Mathf.Approximately(carrouselPrefs.destinationAngle, carrouselPrefs.currentAngle)) return;
            
            if (duration <= 0)
            {
                LayoutNow(carrouselPrefs, onStart, onCompleted);
                return;
            }

            onStart?.Invoke(CurrentIndex);
            
            float elapsedTime = 0;
            float currentAngle = carrouselPrefs.currentAngle;
            float destination = carrouselPrefs.destinationAngle;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                carrouselPrefs.destinationAngle = currentAngle = Mathf.Lerp(currentAngle, destination, t);
                var currentIndex = GetIndexFromAngle(currentAngle);
                
                LayoutNow(carrouselPrefs);
                
                elapsedTime += freq;
                onUpdated?.Invoke(currentAngle, currentIndex);
                await UniTask.Delay(TimeSpan.FromSeconds(freq));
            }

            carrouselPrefs.destinationAngle = destination;
            LayoutNow(carrouselPrefs,null,onCompleted);
            
        }

        private int GetIndexFromAngle(float angle)
        {
            // Subtract the start offset from the angle
            float adjustedAngle = angle - _startOffset;

            // Calculate the wrapped index from the angle
            int wrappedIndex = Mathf.RoundToInt(adjustedAngle / _anglesPerPanel);

            // Reverse the wrapping operation to get the original index
            int index = PanelCount - wrappedIndex;

            // Ensure the index is within the correct range
            index = (index + PanelCount) % PanelCount;

            return index;
        }


        private void LayoutNow(GameObject panelGo, float angle, float radius, Axis axis, Action<int> onFinish = null)
        {
            // Calculate the angle for the current panel
             angle *= Mathf.Deg2Rad; // Convert to radians

            // Calculate the offset position based on the selected axis
            Vector3 offset = Vector3.zero;
            switch (axis)
            {
                case Axis.Up:
                    // Layout around the Up axis (vertical circle)
                    float y = radius * Mathf.Cos(angle);
                    float z = radius * Mathf.Sin(angle);
                    offset = new Vector3(0f, y, z);
                    break;

                case Axis.Side:
                    // Layout around the Side axis (horizontal circle viewed from the side)
                    float xSide = radius * Mathf.Cos(angle);
                    float zSide = radius * Mathf.Sin(angle);
                    offset = new Vector3(xSide, 0f, zSide);
                    break;

                case Axis.Front:
                    // Layout around the Front axis (horizontal circle viewed from the front)
                    float xFront = radius * Mathf.Cos(angle);
                    float yFront = radius * Mathf.Sin(angle);
                    offset = new Vector3(xFront, yFront, 0f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            // Apply the calculated offset to the panel's position
            panelGo.transform.localPosition = offset;

        
            // // Optionally apply spread if it's a factor (spreading could mean additional offset or scale adjustments)
            // if (spread > 0)
            // {
            //     // Here, you might adjust the scale or further modify the position based on the spread.
            //     // For example:
            //     Vector3 spreadOffset = offset.normalized * spread;
            //     panelGo.transform.localPosition += spreadOffset;
            // }
        }
        
    }
}