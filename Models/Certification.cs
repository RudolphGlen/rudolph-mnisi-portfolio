using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

public class Certification
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Issuer { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime DateEarned { get; set; }

    [Url]
    [StringLength(2048)]
    public string? CertificateUrl { get; set; }
}
