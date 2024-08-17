using _Project.Script.UI.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Script.UI.Services
{
    public class UIBehaviourService : IUIBehaviourService
    {
        
        //todo implement
        public GameObject SnapVolume { get => Behaviour.manipPreference.snapVolume; set =>  Behaviour.manipPreference.snapVolume = value; }
        public UIBehaviourPreference Behaviour { get; set; }
        public UniTask<bool> Initialize(UIBehaviourPreference behaviour)
        {
            Debug.Log($"Initializing behaviour service");
            Behaviour = behaviour;
            return new UniTask<bool>(true);
        }

        public UniTask<bool> HandleBehaviour(UIBehaviourMovementPreference behaviour)
        {
            Debug.LogWarning("Handle behaviour not yet implemented ");
            return new UniTask<bool>(true);
        }

        public UniTask<bool> StopBehaviour()
        {
            Debug.LogWarning("stop behaviour not yet implemented ");
            return new UniTask<bool>(true);
        }

        public void DockIn(IUIContainer container)
        {
            if (Behaviour == null) Behaviour = new UIBehaviourPreference();
            if (Behaviour.manipPreference == null) Behaviour.manipPreference = new UIBehaviourManipPreference();
          if(!Behaviour.manipPreference.isDockable) return;
          Debug.LogWarning("Dockin to container");
          Behaviour.manipPreference.DockedIn = container;
        }

        public void UnDock()
        {
            if(!Behaviour.manipPreference.isUndockable) return;
            Behaviour.manipPreference.DockedIn = null;
        }
    }
}