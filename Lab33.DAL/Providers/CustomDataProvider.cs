using System.Globalization;
using System.Text;
using Lab33.DAL.Entities;

namespace Lab33.DAL.Providers;

/// <summary>
/// КОРИСТУВАЦЬКА серіалізація: власний текстовий формат.
///
/// Формат файлу:
///   рядок 1: заголовок "STUDENTS-V1" (щоб відрізнити наш файл від чужого)
///   далі: один студент на рядок, 7 полів через роздільник '|':
///   Surname|Name|Course|StudentTicket|Gender|AverageGrade|IdCode
///
/// Символи '|' та '\' всередині тексту екрануються префіксом '\',
/// тому будь-яке прізвище можна записати і прочитати без втрат.
/// </summary>
public sealed class CustomDataProvider : IDataProvider
{
    private const string Header = "STUDENTS-V1";
    private const char Separator = '|';
    private const char Escape = '\\';
    private const int FieldCount = 7;

    public void Write(string path, IReadOnlyCollection<Student> students)
    {
        var lines = new List<string> { Header };
        foreach (var s in students)
        {
            lines.Add(string.Join(Separator, new[]
            {
                Esc(s.Surname),
                Esc(s.Name),
                s.Course.ToString(CultureInfo.InvariantCulture),
                Esc(s.StudentTicket),
                s.Gender.ToString(),
                // "R" — round-trip: число читається назад без втрати точності;
                // InvariantCulture — щоб роздільником завжди була крапка.
                s.AverageGrade.ToString("R", CultureInfo.InvariantCulture),
                Esc(s.IdCode)
            }));
        }
        File.WriteAllLines(path, lines, Encoding.UTF8);
    }

    public List<Student> Read(string path)
    {
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        if (lines.Length == 0 || lines[0] != Header)
            throw new FormatException("Файл не є файлом користувацького формату STUDENTS-V1.");

        var result = new List<Student>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            List<string> f = Split(lines[i]);
            if (f.Count != FieldCount)
                throw new FormatException($"Рядок {i + 1}: очікувалось {FieldCount} полів, знайдено {f.Count}.");

            result.Add(new Student
            {
                Surname = f[0],
                Name = f[1],
                Course = int.Parse(f[2], CultureInfo.InvariantCulture),
                StudentTicket = f[3],
                Gender = Enum.Parse<Gender>(f[4]),
                AverageGrade = double.Parse(f[5], CultureInfo.InvariantCulture),
                IdCode = f[6]
            });
        }
        return result;
    }

    // Екранування спеціальних символів і прибирання переносів рядка.
    private static string Esc(string value) =>
        value.Replace("\\", "\\\\")
             .Replace("|", "\\|")
             .Replace("\r", "")
             .Replace("\n", " ");

    // Розбиття рядка по '|' з урахуванням екранування.
    private static List<string> Split(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == Escape && i + 1 < line.Length)
            {
                current.Append(line[++i]);   // екранований символ беремо як є
            }
            else if (c == Separator)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        fields.Add(current.ToString());
        return fields;
    }
}
