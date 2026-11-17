namespace Lab33.BLL.Exceptions;

// Власні винятки предметної області (студенти). Рівень BLL їх лише
// ГЕНЕРУЄ (throw); перехоплює і показує користувачу рівень Menu (PL).

/// <summary>Базовий виняток предметної області — зручно ловити одним catch.</summary>
public class StudentServiceException : Exception
{
    public StudentServiceException(string message, Exception? inner = null)
        : base(message, inner) { }
}

/// <summary>Некоректні дані студента (порожнє прізвище, курс поза межами тощо).</summary>
public class InvalidStudentDataException : StudentServiceException
{
    public InvalidStudentDataException(string message) : base(message) { }
}

/// <summary>Студента з таким студентським квитком не знайдено.</summary>
public class StudentNotFoundException : StudentServiceException
{
    public StudentNotFoundException(string ticket)
        : base($"Студента зі студентським квитком '{ticket}' не знайдено.") { }
}

/// <summary>Студент з таким студентським квитком уже існує.</summary>
public class DuplicateStudentException : StudentServiceException
{
    public DuplicateStudentException(string ticket)
        : base($"Студент зі студентським квитком '{ticket}' уже існує.") { }
}

/// <summary>Проблема зі сховищем (файл недоступний, пошкоджений формат тощо).</summary>
public class StorageException : StudentServiceException
{
    public StorageException(string message, Exception? inner = null)
        : base(message, inner) { }
}
