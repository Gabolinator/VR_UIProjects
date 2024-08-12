using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UI
{
    public class InteractableHelper : MonoBehaviour, IXRHoverInteractable, IXRSelectInteractable
    {
        public HoverEnterEvent firstHoverEntered { get; }
        public HoverExitEvent lastHoverExited { get; }
        public HoverEnterEvent hoverEntered { get; }
        public HoverExitEvent hoverExited { get; }
        public SelectEnterEvent firstSelectEntered { get; }
        public SelectExitEvent lastSelectExited { get; }
        public SelectEnterEvent selectEntered { get; }
        public SelectExitEvent selectExited { get; }
        
        public List<IXRSelectInteractor> interactorsSelecting { get; }
        public IXRSelectInteractor firstInteractorSelecting { get; }
       
        public InteractableSelectMode selectMode { get; }
        public InteractionLayerMask interactionLayers { get; }
        
        public List<Collider> colliders { get; }
       
        public event Action<InteractableRegisteredEventArgs> registered;
        public event Action<InteractableUnregisteredEventArgs> unregistered;
      
        public List<IXRHoverInteractor> interactorsHovering { get; }
       
        public bool isHovered { get; }
        public bool isSelected { get; }
        public Transform GetAttachTransform(IXRInteractor interactor)
        {
            return this.transform;
        }

        public void OnRegistered(InteractableRegisteredEventArgs args)
        {
           
        }

        public void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
       
        }

        public void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
          
        }

        public float GetDistanceSqrToInteractor(IXRInteractor interactor)
        {
            return 1f;
        }

   
        public bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return true;
        }

        public void OnHoverEntering(HoverEnterEventArgs args)
        {
          
        }

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
          Debug.Log($"{this.name} is hoverer enter {args}");
        }

        public void OnHoverExiting(HoverExitEventArgs args)
        {
          
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
            Debug.Log($"{this.name} is hoverer enter {args}");
        }

      
        public bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return true;
        }

        public Pose GetAttachPoseOnSelect(IXRSelectInteractor interactor)
        {
            return Pose.identity;
        }

        public Pose GetLocalAttachPoseOnSelect(IXRSelectInteractor interactor)
        {
            return Pose.identity;
        }

        public void OnSelectEntering(SelectEnterEventArgs args)
        {
           
        }

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log($"{this.name} is select enter {args.interactableObject}");
        }

        public void OnSelectExiting(SelectExitEventArgs args)
        {
           
        }

        public void OnSelectExited(SelectExitEventArgs args)
        {
            Debug.Log($"{this.name} is select exited {args.interactableObject}");
        }

      
    }
}