using Lab33.BLL.Exceptions;
using Lab33.BLL.Models;
using Lab33.DAL;
using Lab33.DAL.Entities;
using Lab33.DAL.Providers;

namespace Lab33.BLL;

/// <summary>
/// EntityService — проміжний рівень (бізнес-логіка).
/// Містить методи додавання, видалення, пошуку та виконання операції
/// варіанта 13. НЕ працює з файлами/потоками напряму — лише через EntityContext.
/// Не використовує Console: введення/виведення — справа рівня Menu.
/// </summary>
public class EntityService
{
    /// <summary>Курс, для якого рахуємо відмінниць (за варіантом — 5-й).</summary>
    public const int TargetCourse = 5;

    /// <summary>
    /// Поріг "відмінника". Припущено 100-бальну шкалу (90–100 = відмінно).
    /// Якщо у вашій таблиці 5-бальна шкала — змініть на 4.5 і MaxGrade на 5.
    /// </summary>
    public const double ExcellentGrade = 90.0;
    public const double MaxGrade = 100.0;

    private EntityContext _context;

    public EntityService(SerializationType type, string fileName)
    {
        _context = CreateContext(type, fileName);
    }

    /// <summary>Шлях до поточного файлу (для показу в меню).</summary>
    public string StoragePath => _context.FilePath;

    /// <summary>
    /// Змінити вид серіалізації та/або ім'я файлу. Значення приходять
    /// з рівня представлення і передаються вниз у EntityContext.
    /// </summary>
    public void ChangeStorage(SerializationType type, string fileName)
    {
        _context = CreateContext(type, fileName);
    }

    // ---------------- Базові операції ----------------

    public void AddStudent(StudentInfo student)
    {
        Validate(student);

        List<Student> all = Load();
        if (all.Any(s => s.StudentTicket == student.StudentTicket))
            throw new DuplicateStudentException(student.StudentTicket);

        all.Add(StudentMapper.ToEntity(student));
        Save(all);
    }

    public void RemoveStudent(string studentTicket)
    {
        List<Student> all = Load();
        int removed = all.RemoveAll(s => s.StudentTicket == studentTicket);
        if (removed == 0)
            throw new StudentNotFoundException(studentTicket);

        Save(all);
    }

    public List<StudentInfo> FindBySurname(string surname)
    {
        return Load()
            .Where(s => s.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase))
            .Select(StudentMapper.ToInfo)
            .ToList();
    }

    public List<StudentInfo> GetAll()
    {
        return Load().Select(StudentMapper.ToInfo).ToList();
    }

    // ---------------- Операція варіанта 13 ----------------

    /// <summary>
    /// Студентки-відмінниці 5-го курсу. Дані ЧИТАЮТЬСЯ З ФАЙЛУ (через
    /// EntityContext). Кількість = Count результату.
    /// </summary>
    public List<StudentInfo> GetExcellentFemaleStudentsOfFifthCourse()
    {
        return Load()
            .Where(s => s.Gender == Gender.Female
                        && s.Course == TargetCourse
                        && s.AverageGrade >= ExcellentGrade)
            .Select(StudentMapper.ToInfo)
            .ToList();
    }

    // ---------------- Додаткові сутності та вміння ----------------

    /// <summary>
    /// Демонстрація додаткових сутностей (Storyteller, Dentist) і вміння
    /// «готувати» для студента із заданим квитком. Повертає рядки для виведення.
    /// </summary>
    public List<string> DemonstrateAdditionalEntities(string studentTicket)
    {
        StudentInfo? student = Load()
            .Where(s => s.StudentTicket == studentTicket)
            .Select(StudentMapper.ToInfo)
            .FirstOrDefault();

        if (student is null)
            throw new StudentNotFoundException(studentTicket);

        var storyteller = new Storyteller(student);
        var dentist = new Dentist(student);

        return new List<string>
        {
            storyteller.TellStory(),
            dentist.TreatTeeth(),
            ((ICook)student).Cook()
        };
    }

    // ---------------- Внутрішні методи ----------------

    private static EntityContext CreateContext(SerializationType type, string fileName)
    {
        // Відображення enum BLL -> enum DAL.
        DataProviderType providerType = type switch
        {
            SerializationType.Binary => DataProviderType.Binary,
            SerializationType.Xml => DataProviderType.Xml,
            SerializationType.Json => DataProviderType.Json,
            SerializationType.Custom => DataProviderType.Custom,
            _ => throw new InvalidStudentDataException("Невідомий вид серіалізації.")
        };

        try
        {
            return new EntityContext(providerType, fileName);
        }
        catch (DataAccessException ex)
        {
            throw new StorageException(ex.Message, ex);
        }
    }

    // Читання: якщо файлу ще немає — створюємо порожній.
    // DataAccessException (DAL) перетворюємо на StorageException (BLL) і
    // передаємо вгору — остаточна обробка відбувається в Menu.
    private List<Student> Load()
    {
        try
        {
            if (!_context.FileExists) _context.CreateFile();
            return _context.Open();
        }
        catch (DataAccessException ex)
        {
            throw new StorageException(ex.Message, ex);
        }
    }

    private void Save(List<Student> students)
    {
        try
        {
            _context.Update(students);
        }
        catch (DataAccessException ex)
        {
            throw new StorageException(ex.Message, ex);
        }
    }

    // Перевірка бізнес-правил для студента.
    private static void Validate(StudentInfo s)
    {
        if (string.IsNullOrWhiteSpace(s.Surname))
            throw new InvalidStudentDataException("Прізвище не може бути порожнім.");
        if (string.IsNullOrWhiteSpace(s.Name))
            throw new InvalidStudentDataException("Ім'я не може бути порожнім.");
        if (s.Course < 1 || s.Course > 6)
            throw new InvalidStudentDataException("Курс має бути від 1 до 6.");
        if (string.IsNullOrWhiteSpace(s.StudentTicket))
            throw new InvalidStudentDataException("Номер студентського квитка не може бути порожнім.");
        if (s.AverageGrade < 0 || s.AverageGrade > MaxGrade)
            throw new InvalidStudentDataException($"Середній бал має бути від 0 до {MaxGrade}.");
        if (s.IdCode.Length != 10 || !s.IdCode.All(char.IsDigit))
            throw new InvalidStudentDataException("Ідентифікаційний код має складатися з 10 цифр.");
    }
}
