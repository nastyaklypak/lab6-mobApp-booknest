namespace BookNest;

public partial class ReadingPage : ContentPage
{
    public ReadingPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }

    // Оновлює статистику, графік і історію
    private void Refresh()
    {
        int goal = AppState.WeeklyGoal;
        int week = AppState.MinutesThisWeek();
        int total = AppState.TotalMinutes();

        totalLabel.Text = "Total: " + total / 60 + " h " + total % 60 + " min, " + AppState.TotalPages() + " pages";
        favouriteLabel.Text = "Most read book: " + AppState.FavouriteBook();
        goalLabel.Text = "This week: " + week + " / " + goal + " min";
        goalBar.Progress = Math.Min(1.0, (double)week / goal);

        DrawChart();

        BindableLayout.SetItemsSource(historyList, null);
        BindableLayout.SetItemsSource(historyList, AppState.Sessions.ToList());
        emptyLabel.IsVisible = AppState.Sessions.Count == 0;
    }

    // Малює 7 стовпчиків: для кожного дня - число, стовпчик (BoxView) і назва дня
    private void DrawChart()
    {
        chart.Children.Clear();
        chart.ColumnDefinitions.Clear();

        var days = new List<DateTime>();
        for (int i = 6; i >= 0; i--) days.Add(DateTime.Today.AddDays(-i));
        int max = Math.Max(30, days.Max(d => AppState.MinutesOnDay(d)));   // найвищий стовпчик

        for (int i = 0; i < days.Count; i++)
        {
            chart.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            int minutes = AppState.MinutesOnDay(days[i]);
            bool isToday = days[i] == DateTime.Today;

            var bar = new BoxView
            {
                HeightRequest = Math.Max(2, 90.0 * minutes / max),
                CornerRadius = 4,
                Color = isToday ? Color.FromArgb("#4F46E5") : Color.FromArgb("#A5B4FC")
            };
            var column = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.End,
                Spacing = 2,
                Children =
                {
                    new Label { Text = minutes > 0 ? minutes.ToString() : "", FontSize = 11, HorizontalTextAlignment = TextAlignment.Center },
                    bar,
                    new Label { Text = days[i].ToString("ddd"), FontSize = 11, TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Center }
                }
            };
            chart.Add(column, i, 0);
        }
    }

    // Кнопка Add session
    private void OnAdd(object? sender, EventArgs e)
    {
        string book = bookEntry.Text?.Trim() ?? "";
        if (book == "")
        {
            ShowError("Enter the book title.");
            return;
        }
        if (!int.TryParse(minutesEntry.Text, out int minutes) || minutes <= 0)
        {
            ShowError("Minutes must be a positive number.");
            return;
        }
        int pages = 0;
        if (!string.IsNullOrWhiteSpace(pagesEntry.Text) && (!int.TryParse(pagesEntry.Text, out pages) || pages < 0))
        {
            ShowError("Pages must be a number (or leave it empty).");
            return;
        }

        AppState.AddSession(book, minutes, pages, noteEntry.Text ?? "");
        errorLabel.IsVisible = false;
        minutesEntry.Text = "";
        pagesEntry.Text = "";
        noteEntry.Text = "";
        Refresh();
    }

    private void ShowError(string text)
    {
        errorLabel.Text = text;
        errorLabel.IsVisible = true;
    }

    private async void OnDelete(object? sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ReadingSession session)
        {
            bool ok = await DisplayAlert("Delete session", "Delete this session?", "Delete", "Cancel");
            if (!ok) return;
            AppState.DeleteSession(session);
            Refresh();
        }
    }
}
