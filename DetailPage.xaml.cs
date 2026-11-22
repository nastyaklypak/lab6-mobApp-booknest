namespace BookNest;

public partial class DetailPage : ContentPage
{
    private Book? book;

    public DetailPage()
    {
        InitializeComponent();
    }

    // Сторінка з'являється - показуємо вибрану книжку й завантажуємо додаткові дані
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        book = AppState.SelectedBook;
        if (book == null) return;

        coverImage.Source = book.Cover;
        titleLabel.Text = book.Title;
        authorLabel.Text = book.Author;
        infoLabel.Text = book.Year == "" ? "" : "First published: " + book.Year;
        if (book.Pages > 0) infoLabel.Text += "\n" + book.Pages + " pages";
        subjectsLabel.Text = book.Subjects == "" ? "" : "Topics: " + book.Subjects;
        UpdateStar();

        // 1) опис
        descriptionLabel.Text = "Loading...";
        try
        {
            string description = await Api.LoadDescription(book.Key);
            descriptionLabel.Text = description == "" ? "No description." : description;
        }
        catch (Exception ex)
        {
            descriptionLabel.Text = "Could not load the description: " + ex.Message;
        }

        // 2) інші книжки автора
        othersMessage.Text = "Loading...";
        othersMessage.IsVisible = true;
        BindableLayout.SetItemsSource(othersList, null);
        try
        {
            string firstAuthor = book.Author.Split(',')[0].Trim();
            var others = (await Api.BooksByAuthor(firstAuthor))
                .Where(b => b.Key != book.Key)
                .ToList();
            BindableLayout.SetItemsSource(othersList, others);
            othersMessage.IsVisible = others.Count == 0;
            othersMessage.Text = "No other books found.";
        }
        catch (Exception ex)
        {
            othersMessage.Text = "Could not load books: " + ex.Message;
        }
    }

    private void UpdateStar()
    {
        if (book == null) return;
        starButton.Text = AppState.IsFavorite(book) ? "★ In favorites (tap to remove)" : "☆ Add to favorites";
    }

    private void OnStar(object? sender, EventArgs e)
    {
        if (book == null) return;
        AppState.ToggleFavorite(book);
        UpdateStar();
    }
}
