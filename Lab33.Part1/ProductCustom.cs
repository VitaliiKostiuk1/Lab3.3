using System.Runtime.Serialization;

namespace Lab33.Part1;

/// <summary>
/// КОРИСТУВАЦЬКА серіалізація: клас сам керує тим, що і як записується
/// в потік, реалізуючи інтерфейс ISerializable.
///
/// Потрібні дві речі:
///  1) метод GetObjectData — записує дані в SerializationInfo;
///  2) приватний конструктор (SerializationInfo, StreamingContext) —
///     відновлює об'єкт при десеріалізації.
/// </summary>
[Serializable]
public sealed class ProductCustom : Product, ISerializable
{
    public ProductCustom() { }

    public ProductCustom(string name, DateTime manufactureDate, int shelfLifeDays)
        : base(name, manufactureDate, shelfLifeDays) { }

    // Конструктор десеріалізації. Імена ключів мають збігатися з GetObjectData.
    private ProductCustom(SerializationInfo info, StreamingContext context)
        : base(
            info.GetString("name") ?? string.Empty,
            info.GetDateTime("manufacture"),
            info.GetInt32("shelfLifeDays"))
    { }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("name", Name);
        info.AddValue("manufacture", ManufactureDate);
        info.AddValue("shelfLifeDays", ShelfLifeDays);
    }
}
