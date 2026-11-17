namespace Lab33.BLL.Models;

/// <summary>
/// Вид серіалізації, який обирає користувач у меню. Власний enum рівня BLL:
/// рівень PL не бачить DAL, тому не може використовувати DataProviderType.
/// </summary>
public enum SerializationType
{
    Binary,
    Xml,
    Json,
    Custom
}

public enum StudentGender
{
    Male,
    Female
}
