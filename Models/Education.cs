using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

// Stores one education card displayed on the public portfolio.
public class Education
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Qualification { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Institution { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Dates { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; } = string.Empty;

    // Lower numbers appear first so the newest or most relevant entry can lead.
    [Range(0, 10000)]
    public int DisplayOrder { get; set; }
}
