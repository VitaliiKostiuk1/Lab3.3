namespace Lab33.DAL.Entities;

public enum Gender
{
    Male = 0,
    Female = 1
}

/// <summary>
/// Entity рівня DAL (варіант 13, табл. 2): студент.
/// Поля: прізвище, ім'я, курс, студентський квиток, стать,
/// середній бал, ідентифікаційний код.
///
/// Це "сутність для зберігання": простий об'єкт даних без бізнес-логіки.
/// [Serializable] — для бінарної серіалізації; публічний конструктор без
/// параметрів і публічні властивості — для XML та JSON.
/// </summary>
[Serializable]
public class Student
{
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public double AverageGrade { get; set; }
    public string IdCode { get; set; } = string.Empty;
}
