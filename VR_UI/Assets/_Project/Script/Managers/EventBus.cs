using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Script.UI.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class EventBus : MonoBehaviour
{
    #region Player

    public static Action<PlayerController> OnPlayerStart { get; set; }

    #region LocomotionEvent

    public static Action<InputAction.CallbackContext> OnLeftControllerInput { get; set; }
    public static Action<InputAction.CallbackContext> OnRightControllerInput { get; set; }
    public static Action<bool> OnPlayerMoving { get; set; }

    public static Action<bool> OnPlayerStoppedMoving { get; set; }

    public static Action<float> OnPlayerMovementSpeedChange { get; set; }

    #endregion




    #endregion

  
    
 


    #region GrabEvents

    public static Action<SelectEnterEventArgs> OnObjectGrabbed { get; set; }
    public static Action<SelectExitEventArgs> OnObjectReleased { get; set; }
    public static Action<XRGrabInteractable> OnGrabbableProcess { get; set; }// i.e. scaling
    public static Action<bool, XRGrabInteractable> OnDoubleGrab { get; set; }
   


    #endregion

    
    #region GUIEvents

    public static Action OnToggleMainMenu { get; set; }

    public static Action<bool> OnPointingGui { get; set; }

    public static Action<SelectEnterEventArgs, XRRayInteractor> OnSelectEnter { get; set; }
    public static Action<SelectExitEventArgs, XRRayInteractor> OnSelectExit { get; set; }
    public static Action<HoverEnterEventArgs, XRRayInteractor> OnHoverEnter { get; set; }
    public static Action<HoverExitEventArgs, XRRayInteractor> OnHoverExit { get; set; }
    
    public static Action<IUIPanelInteractable<XRSimpleInteractable>> OnPanelSelectEnter { get; set; }
    public static Action<IUIPanelInteractable<XRSimpleInteractable>> OnPanelSelectExit { get; set; }
    public static Action<IUIPanelInteractable<XRSimpleInteractable>> OnPanelHoverEnter { get; set; }
    public static Action<IUIPanelInteractable<XRSimpleInteractable>> OnPanelHoverExit { get; set; }
    
    
    #endregion

    public static Action OnObjectSpawnInput;
    
}