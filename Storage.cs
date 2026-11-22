using System.Text.Json;

namespace BookNest;

// Збереження й читання файлу booknest.json
public static class Storage
{
    private static readonly string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booknest.json");

    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(path)) return new SaveData();
            return JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path)) ?? new SaveData();
        }
        catch (Exception)
        {
            return new SaveData();   // файл пошкоджений - починаємо з порожніх даних
        }
    }

    public static void Save(SaveData data)
    {
        try
        {
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }
        catch (Exception)
        {
            // не вдалося записати - не заважаємо роботі застосунку
        }
    }
}
