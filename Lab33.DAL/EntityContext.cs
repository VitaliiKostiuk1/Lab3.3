using System.Runtime.Serialization;
using System.Text.Json;
using Lab33.DAL.Entities;
using Lab33.DAL.Providers;

namespace Lab33.DAL;

/// <summary>
/// EntityContext — найнижчий рівень (доступ до даних).
/// Надає узагальнений набір методів для створення, відкриття, оновлення та
/// видалення файлу БД. ЗНАЄ про файли, але НЕ знає, у якому саме форматі
/// вони зберігаються — це приховано за IDataProvider.
/// </summary>
public class EntityContext
{
    private readonly IDataProvider _provider;

    public string FilePath { get; }

    // Тип серіалізації і ім'я файлу приходять з рівня представлення
    // через BLL (EntityService).
    public EntityContext(DataProviderType type, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DataAccessException("Ім'я файлу не може бути порожнім.");

        _provider = DataProviderFactory.Create(type);
        FilePath = filePath;
    }

    public bool FileExists => File.Exists(FilePath);

    /// <summary>Створити порожній файл бази (порожній список студентів).</summary>
    public void CreateFile() => Update(new List<Student>());

    /// <summary>Відкрити файл і прочитати всіх студентів.</summary>
    public List<Student> Open()
    {
        if (!FileExists)
            throw new DataAccessException($"Файл '{FilePath}' не знайдено.");

        return Wrap(() => _provider.Read(FilePath), "Не вдалося прочитати файл");
    }

    /// <summary>Записати (оновити) файл повним списком студентів.</summary>
    public void Update(IReadOnlyCollection<Student> students)
    {
        Wrap<object?>(() =>
        {
            string? dir = Path.GetDirectoryName(Path.GetFullPath(FilePath));
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            _provider.Write(FilePath, students);
            return null;
        }, "Не вдалося записати файл");
    }

    /// <summary>Видалити файл бази.</summary>
    public void DeleteFile()
    {
        Wrap<object?>(() =>
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
            return null;
        }, "Не вдалося видалити файл");
    }

    // Єдине місце, де системні винятки (IO, формат, серіалізація) перетворюються
    // на DataAccessException. Тут виняток НЕ "ковтається" — він передається
    // вгору з InnerException, а обробляє його вищий рівень (Menu).
    private T Wrap<T>(Func<T> action, string message)
    {
        try
        {
            return action();
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or InvalidOperationException
               or SerializationException or JsonException or FormatException
               or InvalidCastException or ArgumentException or OverflowException)
        {
            throw new DataAccessException($"{message} '{FilePath}': {ex.Message}", ex);
        }
    }
}
