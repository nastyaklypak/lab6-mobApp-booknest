using System.Text.Json;

namespace BookNest;

// Мережа: безкоштовний сайт Open Library (ключ не потрібен).
public static class Api
{
    private static readonly HttpClient http = CreateClient();

    private const string Fields = "key,title,author_name,first_publish_year,cover_i,number_of_pages_median,subject";

    // private static HttpClient CreateClient()
    // {
    //     var client = new HttpClient();
    //     client.Timeout = TimeSpan.FromSeconds(15);
    //     // Сайт просить вказувати, хто робить запити
    //     client.DefaultRequestHeaders.UserAgent.ParseAdd("BookNest/1.0 (student project)");
    //     return client;
    // }
        private static HttpClient CreateClient()
    {
#if IOS || MACCATALYST
        // системний мережевий клієнт Apple: довіряє тим самим сертифікатам, що й Safari
        var client = new HttpClient(new NSUrlSessionHandler());
#else
        var client = new HttpClient();
#endif
        client.Timeout = TimeSpan.FromSeconds(15);
        // Сайт просить вказувати, хто робить запити
        client.DefaultRequestHeaders.UserAgent.ParseAdd("BookNest/1.0 (student project)");
        return client;
    }

    // Пошук книжок за назвою або автором
    public static async Task<List<Book>> SearchBooks(string query)
    {
        string url = "https://openlibrary.org/search.json?limit=25&fields=" + Fields
                     + "&q=" + Uri.EscapeDataString(query);
        return ParseBooks(await http.GetStringAsync(url));
    }

    // Інші книжки цього автора
    public static async Task<List<Book>> BooksByAuthor(string author)
    {
        string url = "https://openlibrary.org/search.json?limit=10&fields=" + Fields
                     + "&author=" + Uri.EscapeDataString(author);
        return ParseBooks(await http.GetStringAsync(url));
    }

    // Опис книжки. Повертає "" якщо опису немає
    public static async Task<string> LoadDescription(string key)
    {
        string json = await http.GetStringAsync("https://openlibrary.org" + key + ".json");
        using JsonDocument doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("description", out JsonElement desc)) return "";

        // Опис буває просто рядком, а буває об'єктом {"type": "...", "value": "..."}
        if (desc.ValueKind == JsonValueKind.String) return desc.GetString() ?? "";
        if (desc.ValueKind == JsonValueKind.Object && desc.TryGetProperty("value", out JsonElement value))
            return value.GetString() ?? "";
        return "";
    }

    // Перетворює JSON-відповідь пошуку на список книжок
    public static List<Book> ParseBooks(string json)
    {
        var books = new List<Book>();
        using JsonDocument doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("docs", out JsonElement docs)) return books;

        foreach (JsonElement d in docs.EnumerateArray())
        {
            string key = Text(d, "key");
            string title = Text(d, "title");
            if (key == "" || title == "") continue;   // без ключа книжку не зберегти

            var book = new Book();
            book.Key = key;
            book.Title = title;
            book.Author = FirstItems(d, "author_name", 2);
            if (book.Author == "") book.Author = "Unknown author";
            book.Year = Text(d, "first_publish_year");
            book.CoverId = Number(d, "cover_i");
            book.Pages = Number(d, "number_of_pages_median");
            book.Subjects = FirstItems(d, "subject", 5);
            books.Add(book);
        }
        return books;
    }

    // Читає поле як текст (число теж перетворює на текст). Якщо поля немає - ""
    private static string Text(JsonElement e, string name)
    {
        if (!e.TryGetProperty(name, out JsonElement v)) return "";
        if (v.ValueKind == JsonValueKind.String) return v.GetString() ?? "";
        if (v.ValueKind == JsonValueKind.Number) return v.GetRawText();
        return "";
    }

    private static int Number(JsonElement e, string name)
    {
        if (e.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out int n))
            return n;
        return 0;
    }

    // Бере перші кілька елементів масиву й з'єднує комами: "Frank Herbert, Brian"
    private static string FirstItems(JsonElement e, string name, int count)
    {
        if (!e.TryGetProperty(name, out JsonElement array) || array.ValueKind != JsonValueKind.Array) return "";
        var items = new List<string>();
        foreach (JsonElement item in array.EnumerateArray())
        {
            string s = item.GetString() ?? "";
            if (s != "" && !items.Contains(s)) items.Add(s);
            if (items.Count == count) break;
        }
        return string.Join(", ", items);
    }
}
