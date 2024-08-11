using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class EventBus : MonoBehaviour
{
    #region Player

    public static Action<PlayerController> OnPlayerStart;

    #region LocomotionEvent

    public static Action<InputAction.CallbackContext> OnLeftControllerInput;
    public static Action<InputAction.CallbackContext> OnRightControllerInput;
    public static Action<bool> OnPlayerMoving;

    public static Action<bool> OnPlayerStoppedMoving;

    public static Action<float> OnPlayerMovementSpeedChange;

    #endregion




    #endregion

  
    
 


    #region GrabEvents

    public static Action<SelectEnterEventArgs> OnObjectGrabbed;
    public static Action<SelectExitEventArgs> OnObjectReleased;
    public static Action<XRGrabInteractable> OnGrabbableProcess; // i.e. scaling
    public static Action<bool, XRGrabInteractable> OnDoubleGrab;
   


    #endregion

    
    #region GUIEvents

    public static Action OnToggleMainMenu;

    public static Action<bool> OnPointingGui;
    
    #endregion

    public static Action OnObjectSpawnInput;

}