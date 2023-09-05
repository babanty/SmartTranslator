using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartTranslator.DataAccess.Entities;

/// <summary> Context of the translation </summary>
public record Context
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary> Question the translator has asked to clarify context </summary>
    public string Question { get; set; } = default!;

    /// <summary> User's responce to said question </summary>
    public string? Response { get; set; } = default!;
}
