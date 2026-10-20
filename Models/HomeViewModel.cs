namespace BeginnerMvc.Models;

// This model holds Rudolph's public professional information in one place.
// Only add confirmed facts. Search for PLACEHOLDER to find details still needed.
// Never put identity numbers, private documents, or a home address in this model.
public class HomeViewModel
{
    // Keep visitor input separate from the portfolio owner's public information.
    public ContactFormViewModel Contact { get; set; } = new();

    public string Name { get; set; } = "Rudolph Mnisi";
    public string Initials { get; set; } = "RM";
    public string ProfessionalTitle { get; set; } = "IT, Software Development & Technical Support";
    public string Introduction { get; set; } = "I bring software development and technical support experience, supported by analytical skills, teamwork, and a self-motivated approach to IT.";
    // These two paragraphs appear only in About Me; edit them here to update that section.
    public string Summary { get; set; } = "I am an ICT graduate based in Bushbuckridge, Mpumalanga, South Africa, with an interest in software development and technical support. I enjoy working with technology, solving problems, and continuing to develop my technical skills.";
    public string AboutMore { get; set; } = "I also gained practical industry experience through a two-year internship at InvestHood, where I was exposed to real-world IT and software development work.";

    // These public contact details appear in Get in Touch.
    // The call link uses the international number without display spaces.
    public string Email { get; set; } = "mnisi5345@gmail.com";
    public string Phone { get; set; } = "+27 71 203 4198";
    public string PhoneLink { get; set; } = "+27712034198";
    // A separate contact location keeps this update from changing the Home section.
    public string ContactLocation { get; set; } = "Bushbuckridge, Mpumalanga, South Africa";
    public string Location { get; set; } = "Witbank, Mpumalanga, South Africa";
    public string Availability { get; set; } = "Professional portfolio";

    public string ProfileImageUrl { get; set; } = "/images/profile.jpg";
    // This is still a clearly labeled placeholder PDF, not Rudolph's real CV.
    public string CvUrl { get; set; } = "/documents/Rudolph-Glen-Mnisi-CV.pdf.pdf";

    // Portfolio content is loaded from the database by HomeController.
    public List<Project> Projects { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
    public List<Experience> Experiences { get; set; } = [];
    public List<Certification> Certifications { get; set; } = [];

    // Education cards appear in this order, with the highest qualification first.
    // Use completion dates only; descriptions stay empty because none were provided.
    public EducationItem[] Education { get; set; } =
    [
        new("National Diploma in ICT", "Tshwane University of Technology", "Completed: February 2025", ""),
        new("Matric", "Shanke High School", "Completed: 2017", "")
    ];

}

// Each small type groups the fields needed by one card or timeline entry.
// Add items to the lists above; the view creates matching entries automatically.
public record EducationItem(string Qualification, string Institution, string Dates, string Description);
