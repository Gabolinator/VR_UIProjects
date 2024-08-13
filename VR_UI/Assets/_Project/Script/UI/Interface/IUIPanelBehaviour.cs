using UnityEngine;

namespace UI.Interface
{
    /// <summary>
    /// Interface representing the behavior of a UI panel, including docking, closing, and interaction preferences.
    /// </summary>
    public interface IUIPanelBehaviour
    {
        /// <summary>
        /// Gets or sets the behavior preferences for the UI panel.
        /// </summary>
        public UIBehaviourPreference BehaviourPreferences { get; set; }

        /// <summary>
        /// Gets the behavior service responsible for managing the UI panel's behavior.
        /// </summary>
        public IUIBehaviourService BehaviourService { get; }

        /// <summary>
        /// Gets or sets the container in which the UI panel is currently docked.
        /// </summary>
        public IUIContainer Container { get; set; }

        /// <summary>
        /// Gets the snap volume GameObject associated with the UI panel. 
        /// The snap volume is used for determining docking areas.
        /// </summary>
        public GameObject SnapVolume { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI panel can be closed.
        /// </summary>
        public bool IsClosable { get; set; }

        /// <summary>
        /// Gets a value indicating whether the UI panel is currently docked within a container.
        /// </summary>
        public bool IsDocked { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI panel can be docked within a container.
        /// </summary>
        public bool IsDockable { get; set; }

        /// <summary>
        /// Docks the UI panel into the specified container.
        /// </summary>
        /// <param name="container">The container to dock the UI panel into.</param>
        public void DockIn(IUIContainer container);

        /// <summary>
        /// Undocks the UI panel from its current container, making it free-floating or ready to be docked elsewhere.
        /// </summary>
        public void UnDock();
    }
}