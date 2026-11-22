namespace BookNest;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage()
    {
        InitializeComponent();
    }

    // Щоразу, коли сторінка з'являється, показуємо актуальний список
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ShowList();
    }

    private void ShowList()
    {
        list.ItemsSource = null;
        list.ItemsSource = AppState.Favorites;
        emptyLabel.IsVisible = AppState.Favorites.Count == 0;
    }

    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Book book) return;
        list.SelectedItem = null;
        AppState.SelectedBook = book;
        await Shell.Current.GoToAsync("detail");
    }

    private void OnRemove(object? sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Book book)
        {
            AppState.ToggleFavorite(book);
            ShowList();
        }
    }
}
