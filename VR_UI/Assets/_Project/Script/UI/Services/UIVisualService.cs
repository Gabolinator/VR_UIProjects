using System;
using System.Threading;
using _Project.Script.UI.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace _Project.Script.UI.Services
{
    public class UIVisualService : IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image >
    {
        public IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences MainVisualPreferences { get; set; }

        public void Initialize(IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences components, UnityEvent onShow = null, UnityEvent onHide = null)
        {
            Debug.Log($"Initializing visual service");
            MainVisualPreferences = components;
            OnShow = onShow;
            OnHide = onHide;
        }

        public UnityEvent OnShow { get; set; }
        public UnityEvent OnHide { get; set; }
        
        public Canvas MainCanvas { get => MainVisualPreferences.visualComponents.mainCanvas; set =>MainVisualPreferences.visualComponents.mainCanvas = value; }
        public SortingGroup MainSortingComponent { get => MainVisualPreferences.visualComponents.mainSortingComponent; set =>MainVisualPreferences.visualComponents.mainSortingComponent = value; }
        public CanvasGroup MainAlphaControl { get =>MainVisualPreferences.visualComponents.mainAlphaControl; set =>MainVisualPreferences.visualComponents.mainAlphaControl= value; }


        public bool Visible => !ToggledOff && !FadedOff && !HideInProgress;
        public bool ToggledOff { get; set; }
        public bool FadedOff { get; set; }
        public bool ShowInProgress { get; set; }
        public bool HideInProgress { get; set; }
        
        
        
        public CancellationTokenSource CancelToken { get; set; }
       

        public async UniTask<bool> LerpFade(bool fadeIn, LerpPreferences prefs, Action onToggleComplete
        )
        {
            CancelToken = new CancellationTokenSource();
            return await LerpFade(fadeIn, prefs, onToggleComplete, CancelToken.Token);
        }

        public async UniTask<bool> LerpFade(bool fadeIn, LerpPreferences lerpPref, Action onToggleComplete,
            CancellationToken token)
        {
            if (!lerpPref.shouldLerp) return await Enable(fadeIn, onToggleComplete);
            float duration = lerpPref.duration;
            
            if (lerpPref.duration <= 0) return await FadeNow(fadeIn, onToggleComplete);
            
            float elapsedTime = 0f;
            float startAlpha = lerpPref.minValue;
            float endAlpha = lerpPref.maxValue;
            
            if(fadeIn)Enable(true, onToggleComplete);
            
            while (elapsedTime < duration)
            {
                if (token.IsCancellationRequested)
                {
                    MainAlphaControl.alpha = startAlpha;
                    break;
                }
            
                ShowInProgress = fadeIn;
                HideInProgress = !fadeIn;
                
                float t = elapsedTime / duration;
            
                
                MainAlphaControl.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                
                elapsedTime += Time.deltaTime;
                
                
                await UniTask.Yield();
            }
            
            MainAlphaControl.alpha = endAlpha;
            ShowInProgress = HideInProgress = false;
            FadedOff = !fadeIn;
            if(!fadeIn)Enable(false, onToggleComplete);
            return true;
        }

     
        public  UniTask<bool> Enable(bool state, Action onToggleComplete)
        {
            if (!MainCanvas && !MainAlphaControl)  return new UniTask<bool>(false);
            if (MainCanvas)
            {
                MainCanvas.transform.parent.gameObject.SetActive(state);
                onToggleComplete?.Invoke();
                ToggledOff = !state;
                return new UniTask<bool>(true);
            }
            
            MainAlphaControl.gameObject.SetActive(state);
            onToggleComplete?.Invoke();
            ToggledOff = !state;
            return new UniTask<bool>(true);
        }

        public async UniTask<bool> LerpFade(bool fadeIn, float fadeDuration, Action onToggleComplete)
        {
            if (MainAlphaControl == null) return await Enable(fadeIn, onToggleComplete);
            
            LerpPreferences prefs = new LerpPreferences
            {
                shouldLerp = true,
                duration = fadeDuration,
                minValue = fadeIn ? 0:1,
                maxValue = fadeIn? 1:0
            };
            
            
            return await LerpFade(fadeIn, prefs, onToggleComplete);
        }

        public UniTask<bool> LerpFade(CanvasGroup alphaControl, bool fadeIn, float fadeDuration, Action onToggleComplete)
        {
            throw new NotImplementedException();
        }

        public UniTask<bool> LerpFade(float destinationAlpha, float duration)
        {
            var currentAlpha = MainAlphaControl.alpha;
            if (Mathf.Approximately(currentAlpha, destinationAlpha)) return new UniTask<bool>(false);

            CancelToken = new CancellationTokenSource();

            return LerpFade(currentAlpha, destinationAlpha, duration, CancelToken.Token);
        }




        public async UniTask<bool> LerpFade(float startAlpha, float destinationAlpha, float duration,  CancellationToken token)
        {
            float elapsedTime = 0f;
            if (duration <= 0)
            {
                FadeNow(destinationAlpha);
                return true;
            }
            while (elapsedTime < duration)
            {
                if (token.IsCancellationRequested)
                {
                    MainAlphaControl.alpha = startAlpha;
                    break;
                }
                
                
                float t = elapsedTime / duration;
            
                
                MainAlphaControl.alpha = Mathf.Lerp(startAlpha,  destinationAlpha, t);
                
                elapsedTime += Time.deltaTime;
                
                
                await UniTask.Yield();
            }

            FadeNow(destinationAlpha);
            return true;
        }


        public UniTask<bool> FadeNow(bool fadeIn, Action onToggleComplete = null)
        {
            if (MainAlphaControl == null) return Enable(!fadeIn, onToggleComplete);;

            var fadePref = fadeIn ? MainVisualPreferences.defaultLerpIn : MainVisualPreferences.defaultLerpOut;
            
            float endAlpha = fadePref.maxValue;
            FadeNow(endAlpha);
            
            return new UniTask<bool>(true);
        }

        public UniTask<bool> FadeNow(float destinationAlpha)
        {
            
            FadedOff = destinationAlpha == 0;
            MainAlphaControl.alpha = destinationAlpha;
            ShowInProgress = HideInProgress = false;
             Enable(!FadedOff, null);
           
             return new UniTask<bool>(true);
             
        }

        public UniTask<bool> ToggleVisibility(bool state, bool fade = false, float fadeDuration = -1, Action onToggleComplete = null)
        {
            if (!fade) return Enable(state, onToggleComplete);
            
            return LerpFade(state, fadeDuration, onToggleComplete);
        }

        public UniTask<bool> ToggleVisibility(bool state, bool fade = false, Action onToggleComplete = null)
        {
            var fadePref = state ? MainVisualPreferences.defaultLerpIn : MainVisualPreferences.defaultLerpOut;
            fadePref.shouldLerp = fade;
            return ToggleVisibility(state, fadePref, onToggleComplete);
        }

        public UniTask<bool> ToggleVisibility(bool state, Action onToggleComplete = null)
        {
            return ToggleVisibility(state, MainVisualPreferences, onToggleComplete);
        }

        public UniTask<bool> ToggleVisibility(bool state, IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences preferences, Action onToggleComplete = null)
        {
            if (MainVisualPreferences == null) return new UniTask<bool>(false);
            
            var fadePref = state ? MainVisualPreferences.defaultLerpIn : MainVisualPreferences.defaultLerpOut;
            var fade = fadePref.shouldLerp;
            
            if (!fade) return Enable(state, onToggleComplete);
            
            return LerpFade(state, fadePref, onToggleComplete);
        }

        public UniTask<bool> ToggleVisibility(bool state, LerpPreferences preferences, Action onToggleComplete = null)
        {
            var fade =  preferences.shouldLerp;
            
            if (!fade) return Enable(state, onToggleComplete);
            
            return LerpFade(state, preferences, onToggleComplete);
        }

        public UniTask<bool> Cancel()
        {
            throw new NotImplementedException();
        }

        public UniTask DelayShow(float delay, bool fade = false, float fadeDuration = -1, Action onShowComplete = null)
        { 
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(true, fade, fadeDuration, onShowComplete);
        }

        public UniTask DelayShow(float delay, Action onShowComplete = null)
        {
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(true, onShowComplete);
        }

        public UniTask DelayShow(float delay, IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences preferences, Action onShowComplete = null)
        {
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(true, preferences, onShowComplete);
        }

        public UniTask DelayHide(float delay, bool fade = false, float fadeDuration = -1, Action onHideComplete = null)
        { 
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(false, fade, fadeDuration, onHideComplete);
        }

        public UniTask DelayHide(float delay, Action onHideComplete = null)
        {
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(false, onHideComplete);
        }

        public UniTask DelayHide(float delay, IUIVisualService<Canvas, SortingGroup, CanvasGroup, Image>.VisualPreferences preferences, Action onHideComplete = null)
        {
            UniTask.Delay(TimeSpan.FromSeconds(delay));
            return ToggleVisibility(false, preferences ,onHideComplete);
        }

        public void SetMaxAlpha(float alpha)
        {
            MainVisualPreferences.defaultLerpIn.maxValue = alpha;
            MainVisualPreferences.defaultLerpOut.minValue = alpha;
        }

        public void Toggle(bool state)
        {
            throw new NotImplementedException();
        }
    }
}