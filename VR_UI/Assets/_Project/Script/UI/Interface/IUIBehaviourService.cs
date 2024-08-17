using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Script.UI.Interface
{

    /// <summary>
    /// Container struct for movement-specific behaviour parameters.
    /// </summary>
    [Serializable]
    public struct UIBehaviourMovementPreference
    {
        
        /// <summary>
        /// Enumeration for different UI behaviours.
        /// </summary>
        public enum UIBehaviour
        {
            FollowPlayer,
            DestroyIfPlayerTooFar,
            Sticky,
            undefined
        }

        /// <summary>
        /// The behaviour type.
        /// </summary>
        public UIBehaviour behaviour;
        
        // <summary>
        /// The distance at which the UI element will be destroyed if the player is too far.
        /// </summary>
        [ShowIf("behaviour", UIBehaviour.DestroyIfPlayerTooFar )] public float distanceToDestroy;
        
        /// <summary>
        /// The distance within which the UI element will follow the player.
        /// </summary>
        [ShowIf("behaviour", UIBehaviour.FollowPlayer )] public float distanceToFollow;

        [ShowIf("behaviour", UIBehaviour.FollowPlayer)] public bool clampHeight;  //when following

        [ShowIf("behaviour", UIBehaviour.FollowPlayer) ,ShowIf("clampHeight", true) ]  public float minHeight; // if clampHeigh - relative to player head when following
        [ShowIf("behaviour", UIBehaviour.FollowPlayer ), ShowIf("clampHeight", true)]  public float maxHeight;  //relative to player head  when following
        

    }
    
    /// <summary>
    /// Container class for user manipulation-specific behaviour parameters.
    /// </summary>
    [Serializable]
    public class UIBehaviourManipPreference
    {
        /// <summary>
        /// Indicates whether the UI element can be closed.
        /// </summary>
        public bool isClosable;

        /// <summary>
        /// Indicates whether the UI element can be docked.
        /// </summary>
        public bool isDockable;

        /// <summary>
        /// Indicates whether the UI element can be undocked.
        /// </summary>
        public bool isUndockable;

        /// <summary>
        /// Indicates whether the UI element can be dragged.
        /// </summary>
        public bool isDraggable;

        /// <summary>
        /// Gets whether the UI element is currently docked.
        /// </summary>
        public bool CurrentlyDocked => DockedIn != null;

        /// <summary>
        /// Gets or sets the container in which the UI element is docked.
        /// </summary>
        public IUIContainer DockedIn { get; set; }

        /// <summary>
        /// Indicates whether the UI element snapVolume can be snapped to another snap Volume.
        /// </summary>
        public bool allowSnapping;
        [ShowIf("allowSnapping", true )] public GameObject snapVolume;
    }
    
    
    /// <summary>
    /// Container class for combined UI behaviour preferences.
    /// </summary>
    [Serializable]
    public class UIBehaviourPreference
    {
        /// <summary>
        /// User manipulation-specific behaviour preferences.
        /// </summary>
        public UIBehaviourManipPreference manipPreference;

        /// <summary>
        /// Movement-specific behaviour preferences.
        /// </summary>
        public UIBehaviourMovementPreference movementPreference;

      

    }


    /// <summary>
    /// An interface that handles the behaviour of a UI element.
    /// </summary>
    public interface IUIBehaviourService
    {
        
        public GameObject SnapVolume { get ; set; }
        /// <summary>
        /// Gets or sets the behaviour preferences for the UI element.
        /// </summary>
        public UIBehaviourPreference Behaviour { get; set; }

        /// <summary>
        /// Initializes the behaviour of the UI element.
        /// </summary>
        /// <param name="behaviour">The behaviour preferences to initialize.</param>
        /// <returns>A task representing the asynchronous initialization operation, returning a boolean indicating success.</returns>
        public UniTask<bool> Initialize(UIBehaviourPreference behaviour);

        /// <summary>
        /// Handles the specified movement behaviour of the UI element.
        /// </summary>
        /// <param name="behaviour">The movement behaviour to handle.</param>
        /// <returns>A task representing the asynchronous handling operation, returning a boolean indicating success.</returns>
        public UniTask<bool> HandleBehaviour(UIBehaviourMovementPreference behaviour);

        /// <summary>
        /// Stops the current behaviour of the UI element.
        /// </summary>
        /// <returns>A task representing the asynchronous stop operation, returning a boolean indicating success.</returns>
        public UniTask<bool> StopBehaviour();


        void DockIn(IUIContainer container);
        void UnDock();
    }
}