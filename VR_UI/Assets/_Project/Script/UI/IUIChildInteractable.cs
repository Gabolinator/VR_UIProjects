using UI.Interface;

namespace UI
{
    public interface IUIChildInteractable<TInteractable>
    {
        public IUIPanel SelectedUIPanel { get; }
        public int SelectedIndex { get; }
        
        public IInteractableService<TInteractable> InteractableService { get; set; }
        bool IsInteractable { get; set; }
        bool AllowChildInteractability { get; set; }
        public void SetInteractable(bool interactable);
        
    }
}