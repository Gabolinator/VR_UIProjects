namespace UI.Interface
{
    public interface IUIPanelInteractable<TInteractable>
    {
        public IInteractableService<TInteractable> InteractableService { get; set; }
        bool IsInteractable { get; set; }
        public void SetInteractable(bool interactable);
    }
}