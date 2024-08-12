namespace UI.Interface
{
    public interface IUIPanelInteractable<TInteractable>
    {
        public TInteractable Interactable { get; }

        public IInteractableService<IUIPanelInteractable<TInteractable>,TInteractable> InteractableService { get; set; }
        bool IsInteractable { get; set; }
        public void SetInteractable(bool interactable);
        
        
    }
}