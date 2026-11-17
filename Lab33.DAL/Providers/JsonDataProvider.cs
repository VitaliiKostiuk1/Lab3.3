using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lab33.DAL.Entities;

namespace Lab33.DAL.Providers;

/// <summary>JSON-серіалізація (System.Text.Json).</summary>
public sealed class JsonDataProvider : IDataProvider
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        // Кирилиця у файлі залишається читабельною.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // Gender записується як "Male"/"Female", а не 0/1.
        Converters = { new JsonStringEnumConverter() }
    };

    public List<Student> Read(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<Student>>(json, Options) ?? new List<Student>();
    }

    public void Write(string path, IReadOnlyCollection<Student> students)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(students.ToList(), Options));
    }
}
