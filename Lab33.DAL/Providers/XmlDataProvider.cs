using System.Xml.Serialization;
using Lab33.DAL.Entities;

namespace Lab33.DAL.Providers;

/// <summary>XML-серіалізація (XmlSerializer).</summary>
public sealed class XmlDataProvider : IDataProvider
{
    private static readonly XmlSerializer Serializer = new(typeof(List<Student>));

    public List<Student> Read(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return (List<Student>?)Serializer.Deserialize(fs) ?? new List<Student>();
    }

    public void Write(string path, IReadOnlyCollection<Student> students)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        Serializer.Serialize(fs, students.ToList());
    }
}
