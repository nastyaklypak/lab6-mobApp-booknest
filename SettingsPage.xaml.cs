namespace BookNest;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        themeSwitch.IsToggled = AppState.DarkTheme;
        goalStepper.Value = AppState.WeeklyGoal;
        goalLabel.Text = "Weekly goal: " + AppState.WeeklyGoal + " min";
    }

    private void OnThemeToggled(object? sender, ToggledEventArgs e)
    {
        AppState.DarkTheme = e.Value;
        App.ApplyTheme();
    }

    private void OnGoalChanged(object? sender, ValueChangedEventArgs e)
    {
        AppState.WeeklyGoal = (int)e.NewValue;
        goalLabel.Text = "Weekly goal: " + AppState.WeeklyGoal + " min";
    }

    private async void OnClearSessions(object? sender, EventArgs e)
    {
        if (await DisplayAlert("Clear diary", "Delete all reading sessions?", "Delete", "Cancel"))
            AppState.ClearSessions();
    }

    private async void OnClearFavorites(object? sender, EventArgs e)
    {
        if (await DisplayAlert("Clear favorites", "Remove all favorite books?", "Remove", "Cancel"))
            AppState.ClearFavorites();
    }
}
