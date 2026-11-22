namespace BookNest;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ApplyTheme();
    }

    // Створює вікно програми зі сторінкою AppShell (так у новому MAUI замість MainPage)
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell());

    // Вмикає темну або світлу тему згідно з налаштуванням
    public static void ApplyTheme()
    {
        if (Current != null)
            Current.UserAppTheme = AppState.DarkTheme ? AppTheme.Dark : AppTheme.Light;
    }
}
