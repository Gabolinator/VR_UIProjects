using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace _Project.Script.UI.Helpers
{
    [RequireComponent(typeof(XRRayInteractor))]
    public class InteractorHelper : MonoBehaviour
    {
        [SerializeField] private XRRayInteractor _interactor;
        public Action<SelectEnterEventArgs, XRRayInteractor> OnSelectEnter => EventBus.OnSelectEnter;
        public Action<SelectExitEventArgs, XRRayInteractor> OnSelectExit => EventBus.OnSelectExit;
        public Action<HoverEnterEventArgs, XRRayInteractor> OnHoverEnter => EventBus.OnHoverEnter;
        public Action<HoverExitEventArgs, XRRayInteractor> OnHoverExit => EventBus.OnHoverExit;
        
        
        private void Initialize()
        {
            if (!_interactor) _interactor = GetComponent<XRRayInteractor>();
            AddListeners(_interactor);
        }

        private void AddListeners(XRRayInteractor interactor)
        {
            if(!interactor) return;
            interactor.selectEntered.AddListener(OnSelectEntered);
            interactor.selectExited.AddListener(OnSelectExited);
            interactor.hoverEntered.AddListener(OnHoverEntered);
            interactor.hoverExited.AddListener(OnHoverExited);
        }

        private void RemoveListeners(XRRayInteractor interactor)
        {
            if(!interactor) return;
            interactor.selectEntered.RemoveListener(OnSelectEntered);
            interactor.selectExited.RemoveListener(OnSelectExited);
            interactor.hoverEntered.RemoveListener(OnHoverEntered);
            interactor.hoverExited.RemoveListener(OnHoverExited);
        }

        private void OnHoverExited(HoverExitEventArgs arg)
        {
            Debug.Log($"Hover exited for : {arg.interactableObject}");
            OnHoverExit?.Invoke(arg,_interactor);
        }

        private void OnHoverEntered(HoverEnterEventArgs arg)
        {
            Debug.Log($"Hover enter for : {arg.interactableObject}");
            OnHoverEnter?.Invoke(arg,_interactor);
        }

        private void OnSelectExited(SelectExitEventArgs arg)
        {
            Debug.Log($"Select exit for : {arg.interactableObject}");
            OnSelectExit?.Invoke(arg,_interactor);
        }

        private void OnSelectEntered(SelectEnterEventArgs arg)
        {
            Debug.Log($"Selectr enter for : {arg.interactableObject}");
            OnSelectEnter?.Invoke(arg,_interactor);
        }

        private void Awake()
        {
            Initialize();
        }
    }
}