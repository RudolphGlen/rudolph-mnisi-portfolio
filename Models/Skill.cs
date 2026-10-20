using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

public class Skill
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [StringLength(50)]
    public string ProficiencyLevel { get; set; } = string.Empty;
}
