using Lab33.BLL.Models;
using Lab33.DAL.Entities;

namespace Lab33.BLL;

/// <summary>
/// Перетворення між сутністю DAL (Student) і сутністю BLL (StudentInfo).
/// internal — назовні (до PL) DAL-типи взагалі не "протікають".
/// </summary>
internal static class StudentMapper
{
    public static Student ToEntity(StudentInfo s) => new()
    {
        Surname = s.Surname,
        Name = s.Name,
        Course = s.Course,
        StudentTicket = s.StudentTicket,
        Gender = s.Gender == StudentGender.Female ? Gender.Female : Gender.Male,
        AverageGrade = s.AverageGrade,
        IdCode = s.IdCode
    };

    public static StudentInfo ToInfo(Student s) => new()
    {
        Surname = s.Surname,
        Name = s.Name,
        Course = s.Course,
        StudentTicket = s.StudentTicket,
        Gender = s.Gender == Gender.Female ? StudentGender.Female : StudentGender.Male,
        AverageGrade = s.AverageGrade,
        IdCode = s.IdCode
    };
}
