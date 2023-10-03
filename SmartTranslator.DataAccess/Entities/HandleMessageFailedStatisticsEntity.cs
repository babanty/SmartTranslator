using SmartTranslator.Enums;

namespace SmartTranslator.DataAccess.Entities;

/// <summary> Statistics of failed messages handling </summary>
public record HandleMessageFailedStatisticsEntity
{
    public Guid Id { get; set; }

    public DateTime HappennedAt { get; set; }

    /// <summary> Priority of thrown exception</summary>
    public ExceptionPriority Priority { get; set; }

    /// <summary> Name of thrown exception </summary>
    public string? ExceptionName { get; set; }

    /// <summary> Name of the user who got the exception </summary>
    public string UserName { get; set; } = default!;

    /// <summary> Message in json format </summary>
    public string MessageJson { get; set; } = default!;

    /// <summary> Thrown exception </summary>
    public string Exception { get; set; } = default!;
}
