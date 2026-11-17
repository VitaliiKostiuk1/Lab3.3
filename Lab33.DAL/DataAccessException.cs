namespace Lab33.DAL;

/// <summary>
/// Виняток рівня DAL: будь-яка проблема з файлом або форматом даних.
/// DAL лише ЗГЕНЕРОВУЄ його (загортає системний виняток, зберігаючи його
/// в InnerException) і НЕ обробляє — обробка відбувається на вищому рівні.
/// </summary>
public class DataAccessException : Exception
{
    public DataAccessException(string message, Exception? inner = null)
        : base(message, inner) { }
}
