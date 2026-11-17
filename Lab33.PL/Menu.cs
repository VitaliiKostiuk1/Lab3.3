using System.Globalization;
using System.Text;
using Lab33.BLL;
using Lab33.BLL.Exceptions;
using Lab33.BLL.Models;

namespace Lab33.PL;

/// <summary>
/// Menu — найвищий рівень (представлення). Єдиний, хто працює з Console:
/// вводить дані від користувача, викликає методи EntityService і виводить
/// результат. Саме тут ПЕРЕХОПЛЮЮТЬСЯ й показуються користувачу винятки,
/// згенеровані нижчими рівнями (а не в момент їх виникнення).
/// </summary>
public static class Menu
{
    // Значення за замовчуванням: JSON-файл поруч із програмою.
    private static SerializationType _type = SerializationType.Json;
    private static string _fileName = "students.json";
    private static EntityService _service = new(_type, _fileName);

    /// <summary>Головний цикл меню. Викликається з Main().</summary>
    public static void MainMenu()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            PrintMenu();
            string choice = ReadLine("Ваш вибір: ");

            try
            {
                switch (choice)
                {
                    case "1": ChooseStorage(); break;
                    case "2": AddStudent(); break;
                    case "3": RemoveStudent(); break;
                    case "4": FindStudent(); break;
                    case "5": ShowAll(); break;
                    case "6": ShowExcellentFemaleFifthCourse(); break;
                    case "7": DemoAdditionalEntities(); break;
                    case "0": return;
                    default: Console.WriteLine("Невідомий пункт меню."); break;
                }
            }
            // Спершу — наші винятки предметної області (зрозумілі повідомлення).
            catch (StudentServiceException ex)
            {
                Console.WriteLine($"\n[Помилка] {ex.Message}");
            }
            // Страховка від будь-яких непередбачених помилок: програма не падає.
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Неочікувана помилка] {ex.Message}");
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("================ СТУДЕНТИ ================");
        Console.WriteLine($"Сховище: {_type}, файл: {_service.StoragePath}");
        Console.WriteLine("1. Обрати вид серіалізації та файл");
        Console.WriteLine("2. Додати студента");
        Console.WriteLine("3. Видалити студента (за студентським квитком)");
        Console.WriteLine("4. Знайти студента (за прізвищем)");
        Console.WriteLine("5. Показати всіх студентів");
        Console.WriteLine($"6. Кількість студенток-відмінниць {EntityService.TargetCourse}-го курсу (з файлу)");
        Console.WriteLine("7. Додаткові сутності: Storyteller, Dentist, вміння «готувати»");
        Console.WriteLine("0. Вихід");
    }

    // ---------------- Пункти меню ----------------

    private static void ChooseStorage()
    {
        Console.WriteLine("Вид серіалізації: 1 - Бінарна, 2 - XML, 3 - JSON, 4 - Користувацька");
        SerializationType type = ReadLine("Вибір: ") switch
        {
            "1" => SerializationType.Binary,
            "2" => SerializationType.Xml,
            "3" => SerializationType.Json,
            "4" => SerializationType.Custom,
            _ => throw new InvalidStudentDataException("Потрібно обрати число від 1 до 4.")
        };

        string fileName = ReadLine("Ім'я файлу (наприклад students.dat): ");
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidStudentDataException("Ім'я файлу не може бути порожнім.");

        // Вид серіалізації та ім'я файлу передаються з PL на нижчі рівні.
        _service.ChangeStorage(type, fileName);
        _type = type;
        _fileName = fileName;
        Console.WriteLine("Сховище змінено.");
    }

    private static void AddStudent()
    {
        var model = new StudentModel
        {
            Surname = ReadLine("Прізвище: "),
            Name = ReadLine("Ім'я: "),
            Course = ReadInt("Курс (1-6): "),
            StudentTicket = ReadLine("Студентський квиток: "),
            Gender = ReadGender(),
            AverageGrade = ReadDouble("Середній бал (0-100): "),
            IdCode = ReadLine("Ідентифікаційний код (10 цифр): ")
        };

        _service.AddStudent(model.ToInfo());
        Console.WriteLine("Студента додано і збережено у файл.");
    }

    private static void RemoveStudent()
    {
        string ticket = ReadLine("Студентський квиток для видалення: ");
        _service.RemoveStudent(ticket);
        Console.WriteLine("Студента видалено.");
    }

    private static void FindStudent()
    {
        string surname = ReadLine("Прізвище: ");
        PrintList(_service.FindBySurname(surname).Select(StudentModel.FromInfo).ToList());
    }

    private static void ShowAll()
    {
        PrintList(_service.GetAll().Select(StudentModel.FromInfo).ToList());
    }

    // Операція варіанта 13: кількість студенток-відмінниць 5-го курсу + їх дані.
    private static void ShowExcellentFemaleFifthCourse()
    {
        var students = _service.GetExcellentFemaleStudentsOfFifthCourse()
                               .Select(StudentModel.FromInfo)
                               .ToList();

        Console.WriteLine($"\nКількість студенток-відмінниць {EntityService.TargetCourse}-го курсу: {students.Count}");
        PrintList(students);
    }

    private static void DemoAdditionalEntities()
    {
        string ticket = ReadLine("Студентський квиток студента: ");
        foreach (string line in _service.DemonstrateAdditionalEntities(ticket))
            Console.WriteLine(line);
    }

    // ---------------- Допоміжні методи введення/виведення ----------------

    private static void PrintList(List<StudentModel> students)
    {
        if (students.Count == 0)
        {
            Console.WriteLine("(список порожній)");
            return;
        }
        foreach (var s in students) Console.WriteLine(s);
    }

    private static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    // Некоректне число → виняток предметної області (його покаже MainMenu).
    private static int ReadInt(string prompt)
    {
        if (!int.TryParse(ReadLine(prompt), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            throw new InvalidStudentDataException("Потрібно ввести ціле число.");
        return value;
    }

    private static double ReadDouble(string prompt)
    {
        // Допускаємо і кому, і крапку як роздільник.
        string text = ReadLine(prompt).Replace(',', '.');
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            throw new InvalidStudentDataException("Потрібно ввести число.");
        return value;
    }

    private static StudentGender ReadGender()
    {
        return ReadLine("Стать (Ч/Ж): ").ToUpperInvariant() switch
        {
            "Ж" or "F" => StudentGender.Female,
            "Ч" or "M" => StudentGender.Male,
            _ => throw new InvalidStudentDataException("Стать: введіть Ч або Ж.")
        };
    }
}
