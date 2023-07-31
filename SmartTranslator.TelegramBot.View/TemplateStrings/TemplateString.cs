namespace SmartTranslator.TelegramBot.View.TemplateStrings;

public record TemplateString
{
    public TemplateString(Guid id, string name, string templateString, string descrition, TemplateLanguage language, EnvironmentType environment)
    {
        Id = id;
        Name = name;
        String = templateString;
        Description = descrition;
        Language = language;
        Environment = environment;
    }

    public Guid Id { get; set; } = default!;

    /// <summary> Имя шаблона (основной способ получить по имени) </summary>
    public string Name { get; set; } = default!;

    /// <summary> Собственно сама строка-шаблон </summary>
    public string String { get; set; } = default!;

    /// <summary> Неизменяемое (только через БД) описание шаблона, где необходимо указать какие ключи он включает </summary>
    public string Description { get; set; } = default!;

    /// <summary> Комментарий (изменяемый в отличие от Decription) о конкретно текущем содержимом </summary>
    public string? Comment { get; set; } = default!;

    /// <summary> Поле в которое можно положить дополнительные сведения для фильтрации </summary>
    public string? Tag { get; set; } = default!;

    /// <summary> Язык шаблона </summary>
    public TemplateLanguage Language { get; set; }

    /// <summary> Тип окружения (например prod) </summary>
    public EnvironmentType Environment { get; set; }

    /// <summary> Получить строку с уже заполненными ключами </summary>
    /// <param name="keyAndNewValues"> Ключ - новое значение </param>
    public string Format(IEnumerable<KeyAndNewValue> keyAndNewValues)
    {
        if (string.IsNullOrEmpty(String))
        {
            return string.Empty;
        }

        return BulkReplace(String, keyAndNewValues);
    }

    private static string BulkReplace(string str, IEnumerable<KeyAndNewValue> keyAndNewValues)
    {
        var result = str;

        foreach (var keyAndNewValue in keyAndNewValues)
        {
            result = result.Replace(keyAndNewValue.Key, keyAndNewValue.NewValue);
        }

        return result;
    }

#pragma warning disable CS8618, SA1201, SA1502
    [Obsolete("mapping, serialization, etc")]
    public TemplateString() { }
#pragma warning restore CS8618, SA1201, SA1502
}


/// <summary> Ключ в строке и новое значение </summary>
public record KeyAndNewValue(string Key, string NewValue);
