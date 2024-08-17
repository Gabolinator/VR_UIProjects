using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace _Project.Script.UI.Interface
{
    public interface IUIPanelVisibility<TCanvas, TSortingGroup, TCanvasGroup, TImage>
    {
        public IUIVisualService<TCanvas, TSortingGroup, TCanvasGroup, TImage>.VisualPreferences VisualPreferences { get; set; }
        public IUIVisualService<TCanvas, TSortingGroup, TCanvasGroup, TImage> VisualService { get; }

        public UnityEvent OnShow { get; set; }

        public UnityEvent OnHide { get; set; }

        public bool Visible { get; }
        
        UniTask Show(bool fade, float fadeDuration, Action onShowComplete = null);

        UniTask DelayShow(float delay, bool fade, float fadeDuration, Action onShowComplete = null);

        UniTask DelayHide(float delay, bool fade, float fadeDuration, Action onHideComplete = null);

        UniTask Hide(bool fade, float fadeDuration, Action onHideComplete = null);

        UniTask Show(LerpPreferences preferences, Action onShowComplete = null);

        UniTask Show(Action onShowComplete = null);
        
        
        UniTask DelayShow(float delay, Action onShowComplete = null);

        UniTask DelayHide(float delay, Action onHideComplete = null);

        UniTask Hide(Action onHideComplete = null);

        UniTask Hide(LerpPreferences preferences, Action onShowComplete = null);
        UniTask CancelShow();

        UniTask CancelHide();

        void SetAlpha(float alpha, bool fade, float duration);
        public void SetMaxAlpha(float alpha);




    }
}