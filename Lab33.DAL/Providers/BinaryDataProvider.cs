using System.Runtime.Serialization.Formatters.Binary;
using Lab33.DAL.Entities;

namespace Lab33.DAL.Providers;

/// <summary>Бінарна серіалізація (BinaryFormatter).</summary>
public sealed class BinaryDataProvider : IDataProvider
{
    public List<Student> Read(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return (List<Student>)new BinaryFormatter().Deserialize(fs);
    }

    public void Write(string path, IReadOnlyCollection<Student> students)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        // Серіалізуємо саме List<Student> — тип має збігатися при читанні.
        new BinaryFormatter().Serialize(fs, students.ToList());
    }
}
