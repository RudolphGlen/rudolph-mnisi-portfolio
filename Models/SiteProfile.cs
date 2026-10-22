using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

// Stores the public text and contact information shown across the portfolio.
public class SiteProfile
{
    public const int DefaultId = 1;

    [Key]
    public int Id { get; set; } = DefaultId;

    [Required, StringLength(120)]
    public string Name { get; set; } = "Rudolph Mnisi";

    [Required, StringLength(10)]
    public string Initials { get; set; } = "RM";

    [Required, StringLength(160)]
    public string ProfessionalTitle { get; set; } = "IT, Software Development & Technical Support";

    [Required, StringLength(1000)]
    public string Introduction { get; set; } = "I bring software development and technical support experience, supported by analytical skills, teamwork, and a self-motivated approach to IT.";

    [Required, StringLength(3000)]
    public string Summary { get; set; } = "I am an ICT graduate based in Bushbuckridge, Mpumalanga, South Africa, with an interest in software development and technical support. I enjoy working with technology, solving problems, and continuing to develop my technical skills.";

    [Required, StringLength(3000)]
    public string AboutMore { get; set; } = "I also gained practical industry experience through a two-year internship at InvestHood, where I was exposed to real-world IT and software development work.";

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; } = "mnisi5345@gmail.com";

    [StringLength(40)]
    public string? Phone { get; set; } = "+27 71 203 4198";

    [StringLength(40)]
    public string? PhoneLink { get; set; } = "+27712034198";

    [StringLength(160)]
    public string? ContactLocation { get; set; } = "Bushbuckridge, Mpumalanga, South Africa";

    [StringLength(160)]
    public string? Location { get; set; } = "Witbank, Mpumalanga, South Africa";

    [StringLength(100)]
    public string? Availability { get; set; } = "Professional portfolio";

    [StringLength(2048)]
    public string? ProfileImageUrl { get; set; } = "/images/profile.jpg";

    [StringLength(2048)]
    public string? CvUrl { get; set; } = "/documents/Rudolph-Glen-Mnisi-CV.pdf.pdf";
}
