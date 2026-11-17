namespace Lab33.DAL.Providers;

/// <summary>Тип серіалізації на рівні DAL.</summary>
public enum DataProviderType
{
    Binary,
    Xml,
    Json,
    Custom
}

/// <summary>Фабрика: за типом повертає потрібний провайдер даних.</summary>
public static class DataProviderFactory
{
    public static IDataProvider Create(DataProviderType type) => type switch
    {
        DataProviderType.Binary => new BinaryDataProvider(),
        DataProviderType.Xml => new XmlDataProvider(),
        DataProviderType.Json => new JsonDataProvider(),
        DataProviderType.Custom => new CustomDataProvider(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Невідомий тип серіалізації.")
    };
}
