using Lab33.DAL.Entities;

namespace Lab33.DAL.Providers;

/// <summary>
/// Абстракція "провайдера даних": відповідає за роботу з файлом певного
/// формату. Кожен спосіб серіалізації (бінарний, XML, JSON, користувацький)
/// має власну реалізацію, а EntityContext працює лише з цим інтерфейсом
/// і не знає про деталі формату (принцип абстракції / відсутності дублювання).
/// </summary>
public interface IDataProvider
{
    List<Student> Read(string path);
    void Write(string path, IReadOnlyCollection<Student> students);
}
