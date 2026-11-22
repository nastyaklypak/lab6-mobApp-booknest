using System.Text.Json.Serialization;

namespace BookNest;

// Книжка (так само зберігається в обраному)
public class Book
{
    public string Key { get; set; } = "";       // наприклад "/works/OL45883W"
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Year { get; set; } = "";
    public int CoverId { get; set; }            // 0 = обкладинки немає
    public int Pages { get; set; }
    public string Subjects { get; set; } = "";

    // Нижче - властивості, які лише показуємо на екрані (у файл не пишуться)
    [JsonIgnore]
    public string Cover => CoverId > 0 ? "https://covers.openlibrary.org/b/id/" + CoverId + "-M.jpg" : "";

    [JsonIgnore]
    public string Star => AppState.IsFavorite(this) ? "★" : "☆";

    [JsonIgnore]
    public string Info => Year == "" ? Author : Author + " · " + Year;
}

// Одна сесія читання. id - це час створення в мілісекундах (щоб був унікальним)
public class ReadingSession
{
    public long Id { get; set; }
    public string Book { get; set; } = "";
    public int Minutes { get; set; }
    public int Pages { get; set; }
    public string Note { get; set; } = "";

    [JsonIgnore]
    public DateTime Date => DateTimeOffset.FromUnixTimeMilliseconds(Id).LocalDateTime;

    [JsonIgnore]
    public string Title => Book;

    [JsonIgnore]
    public string Details => Pages > 0 ? Minutes + " min · " + Pages + " pages" : Minutes + " min";

    [JsonIgnore]
    public string When => Date.ToString("d MMM, HH:mm");
}

// Усе, що зберігаємо у файл
public class SaveData
{
    public List<Book> Favorites { get; set; } = new();
    public List<ReadingSession> Sessions { get; set; } = new();
    public bool DarkTheme { get; set; }
    public int WeeklyGoal { get; set; } = 150;   // хвилин читання на тиждень
}
