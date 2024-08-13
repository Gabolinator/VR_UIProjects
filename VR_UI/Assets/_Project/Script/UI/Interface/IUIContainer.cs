using System.Collections.Generic;
using _Project.Script.StateManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Interface
{
        /// <summary>
        /// Interface representing a UI container that can hold and manage multiple UI panels.
        /// </summary>
        public interface IUIContainer
        {
            /// <summary>
            /// Gets or sets the name of the UI container.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets a value indicating whether the UI container is currently visible.
            /// </summary>
            public bool Visible { get; }

            /// <summary>
            /// Gets or sets the list of child panels within the UI container.
            /// </summary>
            public List<IUIPanel> ChildPanels { get; set; }

            /// <summary>
            /// Gets all the UI panels contained within this UI container.
            /// </summary>
            /// <returns>A list of UI panels within the container.</returns>
            public List<IUIPanel> GetUIPanels();

            /// <summary>
            /// Gets a specific UI panel by its name.
            /// </summary>
            /// <param name="name">The name of the UI panel to retrieve.</param>
            /// <returns>The UI panel with the specified name, or null if not found.</returns>
            public IUIPanel GetUIPanelByName(string name);

            /// <summary>
            /// Gets the number of panels within the UI container.
            /// </summary>
            public int NumOfPanels { get; }

            /// <summary>
            /// Initializes the UI container and its associated panels.
            /// </summary>
            public void Initialize();

            /// <summary>
            /// Toggles the visibility of a specific UI panel.
            /// </summary>
            /// <param name="panel">The UI panel to toggle.</param>
            /// <param name="state">The desired visibility state of the panel.</param>
            /// <param name="fade">Whether to fade the panel in or out.</param>
            /// <param name="fadeDuration">The duration of the fade effect. Defaults to -1 for default duration.</param>
            /// <returns>A UniTask representing the asynchronous toggle operation.</returns>
            public UniTask TogglePanel(IUIPanel panel, bool state, bool fade, float fadeDuration = -1);

            /// <summary>
            /// Toggles the visibility of a specific UI panel without fade preferences.
            /// </summary>
            /// <param name="panel">The UI panel to toggle.</param>
            /// <param name="state">The desired visibility state of the panel.</param>
            /// <returns>A UniTask representing the asynchronous toggle operation.</returns>
            public UniTask TogglePanel(IUIPanel panel, bool state);

            /// <summary>
            /// Toggles the visibility of a specific UI panel with customized lerp preferences.
            /// </summary>
            /// <param name="panel">The UI panel to toggle.</param>
            /// <param name="state">The desired visibility state of the panel.</param>
            /// <param name="preferences">The lerp preferences to use for the toggle operation.</param>
            /// <returns>A UniTask representing the asynchronous toggle operation.</returns>
            public UniTask TogglePanel(IUIPanel panel, bool state, LerpPreferences preferences);

            /// <summary>
            /// Toggles the visibility of the root panel within the UI container.
            /// </summary>
            /// <param name="state">The desired visibility state of the root panel.</param>
            /// <returns>A UniTask representing the asynchronous toggle operation.</returns>
            public UniTask ToggleRootPanel(bool state);

            /// <summary>
            /// Registers a UI panel with the container, making it a child panel.
            /// </summary>
            /// <param name="panel">The UI panel to register.</param>
            void Register(IUIPanel panel);

            /// <summary>
            /// Unregisters a UI panel from the container, removing it as a child panel.
            /// </summary>
            /// <param name="panel">The UI panel to unregister.</param>
            void UnRegister(IUIPanel panel);

            /// <summary>
            /// Registers multiple UI panels with the container.
            /// </summary>
            /// <param name="panels">The list of UI panels to register.</param>
            void Register(List<IUIPanel> panels);

            /// <summary>
            /// Unregisters multiple UI panels from the container.
            /// </summary>
            /// <param name="panels">The list of UI panels to unregister.</param>
            void UnRegister(List<IUIPanel> panels);
        }
    }