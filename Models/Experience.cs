using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

public class Experience
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required]
    [StringLength(3000)]
    public string Description { get; set; } = string.Empty;

    public bool IsCurrentRole { get; set; }
}
