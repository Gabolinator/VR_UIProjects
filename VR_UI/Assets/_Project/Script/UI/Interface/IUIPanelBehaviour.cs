using UnityEngine;

namespace UI.Interface
{
    public interface IUIPanelBehaviour
    {
       public UIBehaviourPreference BehaviourPreferences { get; set; }
        public IUIBehaviourService BehaviourService { get; }

        public IUIContainer Container { get; set; }
        
        public GameObject SnapVolume => BehaviourService.SnapVolume;
        public bool IsClosable => BehaviourService.Behaviour.manipPreference.isClosable;
        public bool IsDocked => BehaviourService.Behaviour.manipPreference.CurrentlyDocked;
        public bool IsDockable => BehaviourService.Behaviour.manipPreference.isDockable;
    }
}