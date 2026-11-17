using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Serialization;

namespace Lab33.Part1;

/// <summary>
/// Узагальнені (generic) допоміжні методи серіалізації/десеріалізації.
/// Завдяки T одні й ті самі методи працюють і з масивом Product[],
/// і з колекцією List&lt;Product&gt; (завдання 4).
/// </summary>
public static class Serializers
{
    // ---------- 1. Бінарна серіалізація ----------
    // Записує повний граф об'єктів у компактному бінарному форматі .NET.
    // Для користувацької (ISerializable) серіалізації використовується
    // той самий форматер, але клас сам вирішує, що писати (див. ProductCustom).
    public static void SaveBinary<T>(string path, T data)
    {
        using var fs = new FileStream(path, FileMode.Create);
        new BinaryFormatter().Serialize(fs, data!);
    }

    public static T LoadBinary<T>(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        return (T)new BinaryFormatter().Deserialize(fs);
    }

    // ---------- 2. XML-серіалізація ----------
    // XmlSerializer серіалізує лише ПУБЛІЧНІ поля/властивості і вимагає
    // публічний конструктор без параметрів.
    public static void SaveXml<T>(string path, T data)
    {
        using var fs = new FileStream(path, FileMode.Create);
        new XmlSerializer(typeof(T)).Serialize(fs, data);
    }

    public static T LoadXml<T>(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        return (T)new XmlSerializer(typeof(T)).Deserialize(fs)!;
    }

    // ---------- 3. JSON-серіалізація ----------
    // UnsafeRelaxedJsonEscaping — щоб кирилиця записувалась читабельно,
    // а не як М...
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void SaveJson<T>(string path, T data)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(data, JsonOptions));
    }

    public static T LoadJson<T>(string path)
    {
        return JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonOptions)!;
    }
}
