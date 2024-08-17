using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Project.Script.UI.Interface
{

    
    [Serializable]
    public struct LerpPreferences
    {
        [FormerlySerializedAs("lerpFade")] [FormerlySerializedAs("shouldFade")] public bool shouldLerp;
       
        [FormerlySerializedAs("fadeDuration")] [ShowIf(nameof(shouldLerp))] public float duration;
        /// <summary>
        /// The alpha minimum alpha value that can be set.
        /// </summary>
        [FormerlySerializedAs("minAlpha")] [ShowIf(nameof(shouldLerp))]public float minValue;

        /// <summary>
        /// The alpha maximum alpha value that can be set.
        /// </summary>
        [FormerlySerializedAs("maxAlpha")] [ShowIf(nameof(shouldLerp))] public float maxValue;
    }
    /// <summary>
    /// Interface defining the UI visual service for managing the visibility and visual transitions of UI components.
    /// </summary>
    /// <typeparam name="TCanvas">The type representing the canvas.</typeparam>
    /// <typeparam name="TSort">The type representing the sort visual component.</typeparam>
    /// <typeparam name="TAlphaControl">The type representing the overall alpha control component.</typeparam>
    /// <typeparam name="TImage">The type representing the image.</typeparam>
    public interface IUIVisualService<TCanvas, TSort, TAlphaControl, TImage>
    {
       
        
        [Serializable]
        public class VisualPreferences
        {
            public VisualComponents visualComponents;

            [FormerlySerializedAs("defaultFadeIn")] public LerpPreferences defaultLerpIn;
            [FormerlySerializedAs("defaultFadeOut")] public LerpPreferences defaultLerpOut;
            public bool hideOnStart;
            [FoldoutGroup("Events")] public  UnityEvent onShow;
            [FoldoutGroup("Events")] public  UnityEvent onHide;
        }
        
        /// <summary>
        /// A generic struct to handle visual components in a UI system.
        /// </summary>
        [Serializable]
        public struct VisualComponents
        {
            /// <summary>
            /// The canvas component.
            /// </summary>
            public TCanvas mainCanvas;

            /// <summary>
            /// The sorting component.
            /// </summary>
            public TSort mainSortingComponent;

            /// <summary>
            /// The alpha control component.
            /// </summary>
            public TAlphaControl mainAlphaControl;
            
        }

        public VisualPreferences MainVisualPreferences { get; set; }

        
        public void Initialize( VisualPreferences components,  UnityEvent onShow = null, UnityEvent onHide = null);

        /// <summary>
    /// Event triggered when the UI is shown.
    /// </summary>
    public UnityEvent OnShow { get; set; }

    /// <summary>
    /// Event triggered when the UI is hidden.
    /// </summary>
    public UnityEvent OnHide { get; set; }
    
    /// <summary>
    /// The Main Canvas Component 
    /// </summary>
    public TCanvas MainCanvas { get; set; }

    /// <summary>
    /// The sorting component.
    /// </summary>
    public TSort MainSortingComponent { get; set; }

    /// <summary>
    /// The alpha control component.
    /// </summary>
    public TAlphaControl MainAlphaControl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the UI is visible.
    /// </summary>
    public bool Visible { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the UI is toggled off.
    /// </summary>
    public bool ToggledOff { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the UI is faded off.
    /// </summary>
    public bool FadedOff { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a show operation is in progress.
    /// </summary>
    public bool ShowInProgress { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a hide operation is in progress.
    /// </summary>
    public bool HideInProgress { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token source used for cancelling ongoing operations.
    /// </summary>
    public CancellationTokenSource CancelToken { get; set; }
    

    /// <summary>
    /// Fades the UI in or out over the specified duration.
    /// </summary>
    /// <param name="fadeIn">If true, fades in; otherwise, fades out.</param>
    /// <param name="fadeDuration">The duration of the fade.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the fade.</param>
    /// <returns>A task that represents the asynchronous fade operation, returning a boolean indicating success.</returns>
    public UniTask<bool> LerpFade(bool fadeIn, float fadeDuration, Action onToggleComplete);


    /// <summary>
    /// Fades the specified UI element in or out over the specified duration.
    /// </summary>
    /// <param name="alphaControl"> The Component on which the fade will happen</param>
    /// <param name="fadeIn">If true, fades in; otherwise, fades out.</param>
    /// <param name="fadeDuration">The duration of the fade.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the fade.</param>
    /// <returns>A task that represents the asynchronous fade operation, returning a boolean indicating success.</returns>
    public UniTask<bool> LerpFade(TAlphaControl alphaControl, bool fadeIn ,float fadeDuration, Action onToggleComplete);
    
    /// <summary>
    /// Set the UI a value to a specified value over a specified duration.
    /// </summary>
    /// <param name="destinationAlpha">The end value of the alpha.</param>
    /// <param name="fadeDuration">The duration of the fade.</param>
    /// <returns>A task that represents the asynchronous fade operation, returning a boolean indicating success.</returns>
    public UniTask<bool> LerpFade(float destinationAlpha , float fadeDuration);

    /// <summary>
    /// Set the UI alpha value to a fadeIn or fadeOut default value.
    /// </summary>
    /// <param name="fadeIn">If true, disables the UI visibilit; otherwise, enables it.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the disable/enable operation.</param>
    /// <returns>A task that represents the asynchronous disable/enable operation, returning a boolean indicating success.</returns>
    public UniTask<bool> FadeNow(bool fadeIn, Action onToggleComplete = null);

    /// <summary>
    /// Set the UI alpha value to a specified value
    /// </summary>
    /// <param name="destinationAlpha">The end value of the alpha.</param>
    /// <returns>A task that represents the asynchronous fade operation, returning a boolean indicating success.</returns>
    public UniTask<bool> FadeNow(float destinationAlpha);
    
    /// <summary>
    /// Toggles the visibility of the UI, with optional fade effect.
    /// </summary>
    /// <param name="state">If true, shows the UI; otherwise, hides it.</param>
    /// <param name="fade">If true, applies a fade effect.</param>
    /// <param name="fadeDuration">The duration of the fade effect.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the toggle operation.</param>
    /// <returns>A task that represents the asynchronous toggle operation, returning a boolean indicating success.</returns>
    public UniTask<bool> ToggleVisibility(bool state, bool fade = false, float fadeDuration = -1, Action onToggleComplete = null);

    /// <summary>
    /// Toggles the visibility of the UI, with optional fade effect.
    /// </summary>
    /// <param name="state">If true, shows the UI; otherwise, hides it.</param>
    /// <param name="fade">If true, applies a fade effect.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the toggle operation.</param>
    /// <returns>A task that represents the asynchronous toggle operation, returning a boolean indicating success.</returns>
    public UniTask<bool> ToggleVisibility(bool state, bool fade = false, Action onToggleComplete = null);
    
    /// <summary>
    /// Toggles the visibility of the UI, using the default VisualPreferences
    /// </summary>
    /// <param name="state">If true, shows the UI; otherwise, hides it.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the toggle operation.</param>
    /// <returns>A task that represents the asynchronous toggle operation, returning a boolean indicating success.</returns>
    public UniTask<bool> ToggleVisibility(bool state, Action onToggleComplete = null);
    
    /// <summary>
    /// Toggles the visibility of the UI, using the specified VisualPreferences
    /// </summary>
    /// <param name="state">If true, shows the UI; otherwise, hides it.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the toggle operation.</param>
    /// <param name="preferences">VisualPreferences to be used for fading values.</param>
    /// <returns>A task that represents the asynchronous toggle operation, returning a boolean indicating success.</returns>
    public UniTask<bool> ToggleVisibility(bool state, VisualPreferences preferences , Action onToggleComplete = null);
    
    
    /// <summary>
    /// Toggles the visibility of the UI, using the specified FadePreference and default VisualComponents
    /// </summary>
    /// <param name="state">If true, shows the UI; otherwise, hides it.</param>
    /// <param name="onToggleComplete">Action to be invoked upon completion of the toggle operation.</param>
    /// <param name="preferences">FadePreferences to be used for fading values.</param>
    /// <returns>A task that represents the asynchronous toggle operation, returning a boolean indicating success.</returns>
    public UniTask<bool> ToggleVisibility(bool state, LerpPreferences preferences , Action onToggleComplete = null);

    /// <summary>
    /// Cancels any ongoing visibility or fade operations.
    /// </summary>
    /// <returns>A task that represents the asynchronous cancel operation, returning a boolean indicating success.</returns>
    public UniTask<bool> Cancel();

    /// <summary>
    /// Delays showing the UI by the specified amount of time, with optional fade effect.
    /// </summary>
    /// <param name="delay">The delay before showing the UI.</param>
    /// <param name="fade">If true, applies a fade effect.</param>
    /// <param name="fadeDuration">The duration of the fade effect.</param>
    /// <param name="onShowComplete">Action to be invoked upon completion of the show operation.</param>
    /// <returns>A task that represents the asynchronous delay show operation.</returns>
    public UniTask DelayShow(float delay, bool fade = false, float fadeDuration = -1, Action onShowComplete = null);

    /// <summary>
    /// Delays showing the UI by the specified amount of time, using default visual preferences.
    /// </summary>
    /// <param name="delay">The delay before hiding the UI.</param>
    /// <param name="onHideComplete">Action to be invoked upon completion of the hide operation.</param>
    /// <returns>A task that represents the asynchronous delay hide operation.</returns>
    public UniTask DelayShow(float delay, Action onHideComplete = null);
    /// <summary>
    /// Delays showing the UI by the specified amount of time, using specified visual preferences.
    /// </summary>
    /// <param name="delay">The delay before hiding the UI.</param>
    /// <param name="preferences">VisualPreferences to be used for fading values.</param>
    /// <param name="onShowComplete">Action to be invoked upon completion of the hide operation.</param>
    /// <returns>A task that represents the asynchronous delay hide operation.</returns>
    public UniTask DelayShow(float delay, VisualPreferences preferences, Action onShowComplete = null);
    
    /// <summary>
    /// Delays hiding the UI by the specified amount of time, with optional fade effect.
    /// </summary>
    /// <param name="delay">The delay before hiding the UI.</param>
    /// <param name="fade">If true, applies a fade effect.</param>
    /// <param name="fadeDuration">The duration of the fade effect.</param>
    /// <param name="onHideComplete">Action to be invoked upon completion of the hide operation.</param>
    /// <returns>A task that represents the asynchronous delay hide operation.</returns>
    public UniTask DelayHide(float delay, bool fade = false, float fadeDuration = -1, Action onHideComplete = null);
    
    /// <summary>
    /// Delays hiding the UI by the specified amount of time, using default visual preferences.
    /// </summary>
    /// <param name="delay">The delay before hiding the UI.</param>
    /// <param name="onHideComplete">Action to be invoked upon completion of the hide operation.</param>
    /// <returns>A task that represents the asynchronous delay hide operation.</returns>
    public UniTask DelayHide(float delay, Action onHideComplete = null);
    /// <summary>
    /// Delays hiding the UI by the specified amount of time, using specified visual preferences.
    /// </summary>
    /// <param name="delay">The delay before hiding the UI.</param>
    /// <param name="preferences">VisualPreferences to be used for fading values.</param>
    /// <param name="onHideComplete">Action to be invoked upon completion of the hide operation.</param>
    /// <returns>A task that represents the asynchronous delay hide operation.</returns>
    public UniTask DelayHide(float delay, VisualPreferences preferences, Action onHideComplete = null);

    /// <summary>
    /// Set the  Max Limit for the alpha value
    /// </summary>
    /// <param name="alpha"></param>
    void SetMaxAlpha(float alpha);


    public UniTask<bool> Enable(bool state, Action onToggleComplete);
    }
}