namespace BeginnerMvc.Models;

// This model holds Rudolph's public professional information in one place.
// Only add confirmed facts. Search for PLACEHOLDER to find details still needed.
// Never put identity numbers, private documents, or a home address in this model.
public class HomeViewModel
{
    // Keep visitor input separate from the portfolio owner's public information.
    public ContactFormViewModel Contact { get; set; } = new();
    public SiteProfile Profile { get; set; } = new();

    // The profile and all portfolio sections are loaded from the database by HomeController.
    public List<Project> Projects { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
    public List<Experience> Experiences { get; set; } = [];
    public List<Certification> Certifications { get; set; } = [];

    public List<Education> Education { get; set; } = [];

}
