using Lab33.BLL.Models;

namespace Lab33.PL;

/// <summary>
/// Власна сутність рівня представлення (PL). Окремий клас, щоб зміни
/// в BLL/DAL не змушували переписувати інтерфейс користувача.
/// </summary>
public class StudentModel
{
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public StudentGender Gender { get; set; }
    public double AverageGrade { get; set; }
    public string IdCode { get; set; } = string.Empty;

    // Рядок для виведення в консоль.
    public override string ToString() =>
        $"{Surname} {Name} | {Course} курс | квиток: {StudentTicket} | " +
        $"{(Gender == StudentGender.Female ? "жін." : "чол.")} | " +
        $"бал: {AverageGrade:0.##} | ІПН: {IdCode}";

    // Перетворення PL <-> BLL.
    public StudentInfo ToInfo() => new()
    {
        Surname = Surname,
        Name = Name,
        Course = Course,
        StudentTicket = StudentTicket,
        Gender = Gender,
        AverageGrade = AverageGrade,
        IdCode = IdCode
    };

    public static StudentModel FromInfo(StudentInfo s) => new()
    {
        Surname = s.Surname,
        Name = s.Name,
        Course = s.Course,
        StudentTicket = s.StudentTicket,
        Gender = s.Gender,
        AverageGrade = s.AverageGrade,
        IdCode = s.IdCode
    };
}
