namespace BookNest;

public partial class SearchPage : ContentPage
{
    private List<Book> results = new();
    private bool loading = false;

    public SearchPage()
    {
        InitializeComponent();
    }

    // Показуємо список ще раз, щоб оновились зірочки (наприклад, після повернення зі сторінки Detail)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ShowResults();
    }

    private void ShowResults()
    {
        list.ItemsSource = null;
        list.ItemsSource = results;
    }

    // Кнопка Search (або Enter у рядку пошуку)
    private async void OnSearch(object? sender, EventArgs e)
    {
        string query = queryEntry.Text?.Trim() ?? "";
        if (query == "" || loading) return;

        loading = true;
        spinner.IsVisible = true;
        spinner.IsRunning = true;
        message.IsVisible = false;
        try
        {
            results = await Api.SearchBooks(query);
            ShowResults();
            if (results.Count == 0)
            {
                message.Text = "Nothing found. Try another query.";
                message.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            results = new List<Book>();
            ShowResults();
            message.Text = "Network error: " + ex.Message;
            message.IsVisible = true;
        }
        spinner.IsRunning = false;
        spinner.IsVisible = false;
        loading = false;
    }

    // Натиснули на книжку - відкриваємо сторінку Detail
    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Book book) return;
        list.SelectedItem = null;
        AppState.SelectedBook = book;
        await Shell.Current.GoToAsync("detail");
    }

    // Натиснули на зірочку
    private void OnStar(object? sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Book book)
        {
            AppState.ToggleFavorite(book);
            ShowResults();
        }
    }
}
