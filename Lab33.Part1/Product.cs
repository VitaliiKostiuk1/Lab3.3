namespace Lab33.Part1;

/// <summary>
/// Клас «Товар» (варіант 13, табл. 1).
/// Дані: назва, дата виготовлення, термін придатності.
/// Методи: визначення придатності до споживання, визначення кінцевого строку
/// реалізації, виведення інформації про товар.
///
/// [Serializable] — позначає, що екземпляри класу можна серіалізувати
/// бінарним форматером (BinaryFormatter). Для XML та JSON атрибут не потрібен,
/// але ці серіалізатори вимагають ПУБЛІЧНИЙ конструктор без параметрів
/// та публічні властивості з set.
/// </summary>
[Serializable]
public class Product
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Дата виготовлення.</summary>
    public DateTime ManufactureDate { get; set; }

    /// <summary>Термін придатності, у днях.</summary>
    public int ShelfLifeDays { get; set; }

    // Обов'язковий для XmlSerializer / System.Text.Json конструктор без параметрів.
    public Product() { }

    public Product(string name, DateTime manufactureDate, int shelfLifeDays)
    {
        Name = name;
        ManufactureDate = manufactureDate;
        ShelfLifeDays = shelfLifeDays;
    }

    /// <summary>Кінцевий строк реалізації = дата виготовлення + термін придатності.</summary>
    public DateTime GetSellByDate() => ManufactureDate.Date.AddDays(ShelfLifeDays);

    /// <summary>Чи придатний товар до споживання на вказану дату.</summary>
    public bool IsFitForConsumption(DateTime onDate) => onDate.Date <= GetSellByDate();

    /// <summary>Інформація про товар (рядок для виведення).</summary>
    public string GetInfo(DateTime onDate) =>
        $"Товар: {Name}; виготовлено: {ManufactureDate:dd.MM.yyyy}; " +
        $"термін придатності: {ShelfLifeDays} дн.; " +
        $"кінцевий строк реалізації: {GetSellByDate():dd.MM.yyyy}; " +
        $"придатний до споживання: {(IsFitForConsumption(onDate) ? "так" : "ні")}";

    public void PrintInfo(DateTime onDate) => Console.WriteLine(GetInfo(onDate));

    // Потрібно для порівняння масиву до серіалізації та після десеріалізації.
    public override bool Equals(object? obj) =>
        obj is Product p &&
        Name == p.Name &&
        ManufactureDate == p.ManufactureDate &&
        ShelfLifeDays == p.ShelfLifeDays;

    public override int GetHashCode() => HashCode.Combine(Name, ManufactureDate, ShelfLifeDays);
}
