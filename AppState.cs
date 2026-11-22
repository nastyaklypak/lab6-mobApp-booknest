namespace BookNest;

// Тут зберігаються всі дані застосунку та дії над ними.
// Сторінки лише показують ці дані й викликають методи AppState.
public static class AppState
{
    private static readonly SaveData saved = Storage.Load();

    // Книжка, яку відкрили на сторінці Detail
    public static Book? SelectedBook;

    public static List<Book> Favorites => saved.Favorites;
    public static List<ReadingSession> Sessions => saved.Sessions;

    public static bool DarkTheme
    {
        get => saved.DarkTheme;
        set { saved.DarkTheme = value; Save(); }
    }

    public static int WeeklyGoal
    {
        get => saved.WeeklyGoal;
        set { saved.WeeklyGoal = Math.Clamp(value, 30, 1200); Save(); }
    }

    private static void Save() => Storage.Save(saved);

    // ---------- обрані книжки ----------
    public static bool IsFavorite(Book book) => saved.Favorites.Any(b => b.Key == book.Key);

    public static void ToggleFavorite(Book book)
    {
        if (IsFavorite(book))
            saved.Favorites.RemoveAll(b => b.Key == book.Key);
        else
            saved.Favorites.Add(book);
        Save();
    }

    public static void ClearFavorites()
    {
        saved.Favorites.Clear();
        Save();
    }

    // ---------- сесії читання ----------
    public static void AddSession(string book, int minutes, int pages, string note)
    {
        var session = new ReadingSession
        {
            Id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Book = book.Trim(),
            Minutes = minutes,
            Pages = pages,
            Note = note.Trim()
        };
        saved.Sessions.Insert(0, session);   // нова - на початок списку
        Save();
    }

    public static void DeleteSession(ReadingSession session)
    {
        saved.Sessions.RemoveAll(s => s.Id == session.Id);
        Save();
    }

    public static void ClearSessions()
    {
        saved.Sessions.Clear();
        Save();
    }

    // ---------- статистика ----------
    // Скільки хвилин читання було в певний день
    public static int MinutesOnDay(DateTime day)
        => saved.Sessions.Where(s => s.Date.Date == day.Date).Sum(s => s.Minutes);

    // Хвилини за останні 7 днів (разом із сьогодні)
    public static int MinutesThisWeek()
    {
        int total = 0;
        for (int i = 0; i < 7; i++) total += MinutesOnDay(DateTime.Today.AddDays(-i));
        return total;
    }

    public static int TotalMinutes() => saved.Sessions.Sum(s => s.Minutes);

    public static int TotalPages() => saved.Sessions.Sum(s => s.Pages);

    // Книжка, на яку витрачено найбільше часу
    public static string FavouriteBook()
    {
        var top = saved.Sessions
            .GroupBy(s => s.Book)
            .OrderByDescending(g => g.Sum(s => s.Minutes))
            .FirstOrDefault();
        return top == null ? "-" : top.Key;
    }
}
