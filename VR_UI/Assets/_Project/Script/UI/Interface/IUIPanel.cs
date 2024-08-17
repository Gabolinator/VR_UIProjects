using System;
using _Project.Script.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.UI.Interface
{
    /// <summary>
    /// Interface representing a UI panel that can be managed and interacted with within a UI system.
    /// </summary>
    public interface IUIPanel
    {
        /// <summary>
        /// Gets or sets the name of the UI panel.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the GameObject associated with the UI panel.
        /// </summary>
        public GameObject Go { get; }

        /// <summary>
        /// Gets a value indicating whether the UI panel is currently visible.
        /// </summary>
        public bool Visible { get; }

        /// <summary>
        /// Initializes the UI panel. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Closes the UI panel, with an option to destroy it afterward.
        /// </summary>
        /// <param name="destroy">If true, the panel is destroyed after closing. Defaults to true.</param>
        /// <param name="onClose">Optional callback invoked after the panel is closed.</param>
        /// <returns>A UniTask representing the asynchronous close operation.</returns>
        public UniTask Close(bool destroy = true, Action onClose = null);

        /// <summary>
        /// Registers the UI panel to a UIManager, allowing it to be managed by that manager.
        /// </summary>
        /// <param name="manager">The UIManager to register the panel to.</param>
        public void RegisterToManager(UIManager manager);

        /// <summary>
        /// Unregisters the UI panel from a UIManager, removing it from the manager's control.
        /// </summary>
        /// <param name="manager">The UIManager to unregister the panel from.</param>
        public void UnRegisterFromManager(UIManager manager);

        /// <summary>
        /// Destroys the UI panel, optionally invoking a callback after destruction.
        /// </summary>
        /// <param name="onDestroy">Optional callback invoked after the panel is destroyed.</param>
        public void DestroySelf(Action onDestroy = null);
    }
}