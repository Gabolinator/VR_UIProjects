using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class InputManager : MonoBehaviour
{
   
    [SerializeField] InputActionReference _translateAnchorRef;
    private InputAction _translateAnchorInput;

    [SerializeField] InputActionReference _rotateAnchorRef;
    private InputAction _rotateAnchorInput;
    

    [SerializeField] InputActionReference _toggleMainMenuRef;
    private InputAction _toggleMainMenuInput;
    private PlayerController _player;

   
    Action OnToggleMainMenu => EventBus.OnToggleMainMenu;

    Action OnObjectSpawnInput => EventBus.OnObjectSpawnInput;
    
    //from  ActionBasedControllerManager
    InputAction GetInputAction(InputActionReference actionReference)
    {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
        return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
    }

    public void ToggleAction(InputAction action, bool state) 
    {
        if (action == null) return;
        Debug.Log("[Input Manager] Toggle Action : " + action.name +"/ state: " + state);
       
        if(state) action.Enable();
        else action.Disable();
    }


    public void ToggleMainMenu(InputAction.CallbackContext obj)
    {
        OnToggleMainMenu?.Invoke();
    }


    public void SpawnObject(InputAction.CallbackContext obj)
    {
        OnObjectSpawnInput?.Invoke();
    }

    private void DisableAnchorTransform(bool state)
    {
        
        ToggleAction(_translateAnchorInput, !state);
        ToggleAction(_rotateAnchorInput, !state);
    }

 

   
    public void Awake()
    {
    
        _translateAnchorInput = GetInputAction(_translateAnchorRef);
        _rotateAnchorInput = GetInputAction(_rotateAnchorRef);
        _toggleMainMenuInput = GetInputAction(_toggleMainMenuRef);
        
        DisableAnchorTransform(false);
    }

    
    private void OnEnable()
    {
        _toggleMainMenuInput.performed += ToggleMainMenu;
 
    }

  
    
    private void OnDisable()
    {
        _toggleMainMenuInput.performed -= ToggleMainMenu;
       
    }

  

  
}
