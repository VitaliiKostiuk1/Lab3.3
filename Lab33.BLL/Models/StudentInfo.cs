namespace Lab33.BLL.Models;

/// <summary>
/// Додаткове вміння студента (варіант 13): готувати.
/// </summary>
public interface ICook
{
    string Cook();
}

/// <summary>
/// Власна сутність рівня BLL (щоб не залежати від класу Student з DAL і
/// зменшити зв'язки між рівнями). Містить бізнес-поведінку:
/// відмінник / студентка / вміння готувати.
/// </summary>
public class StudentInfo : ICook
{
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public StudentGender Gender { get; set; }
    public double AverageGrade { get; set; }
    public string IdCode { get; set; } = string.Empty;

    public StudentInfo() { }

    // Копіюючий конструктор — для створення Storyteller / Dentist зі студента.
    protected StudentInfo(StudentInfo other)
    {
        Surname = other.Surname;
        Name = other.Name;
        Course = other.Course;
        StudentTicket = other.StudentTicket;
        Gender = other.Gender;
        AverageGrade = other.AverageGrade;
        IdCode = other.IdCode;
    }

    public string Cook() => $"{Surname} {Name} готує смачну вечерю.";

    public override string ToString() =>
        $"{Surname} {Name}, {Course} курс, квиток: {StudentTicket}, " +
        $"стать: {(Gender == StudentGender.Female ? "жіноча" : "чоловіча")}, " +
        $"середній бал: {AverageGrade:0.##}, ІПН: {IdCode}";
}

/// <summary>Додаткова сутність: студент-«сказитель» (Storyteller).</summary>
public class Storyteller : StudentInfo
{
    public Storyteller(StudentInfo student) : base(student) { }

    public string TellStory() => $"{Surname} {Name} розповідає цікаву історію.";
}

/// <summary>Додаткова сутність: студент-стоматолог (Dentist).</summary>
public class Dentist : StudentInfo
{
    public Dentist(StudentInfo student) : base(student) { }

    public string TreatTeeth() => $"{Surname} {Name} лікує зуби пацієнту.";
}
