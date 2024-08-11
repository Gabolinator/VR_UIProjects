namespace UI.Interface
{
    public class UIKeyboardService : IUIKeyboardService
    {
        public IUIKeyboardService.UIKeyboardPreference Preferences { get; set; }

        public UIKeyboardService()
        {
        }

        public UIKeyboardService(IUIKeyboardService.UIKeyboardPreference prefs)
        {
            Preferences = prefs;
        }

        public void Initialize(IUIKeyboardService.UIKeyboardPreference prefs)
        {
            Preferences = prefs;
        }
    }
}