namespace _Project.Script.UI.Interface
{
    public interface IUIKeyboardSupport
    {
        public IUIKeyboardService.UIKeyboardPreference UIKeyboardPreferences { get; set; }
        public IUIKeyboardService KeyboardService { get; set; }
    }
}