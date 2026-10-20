using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(3000)]
    public string Description { get; set; } = string.Empty;

    // Store technology names as a comma-separated string for this initial model.
    [Required]
    [StringLength(1000)]
    public string Technologies { get; set; } = string.Empty;

    [Url]
    [StringLength(2048)]
    public string? ProjectUrl { get; set; }

    [Url]
    [StringLength(2048)]
    public string? GithubUrl { get; set; }
}
