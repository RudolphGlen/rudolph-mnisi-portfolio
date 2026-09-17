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

    // These are the skills Rudolph provided; no programming languages or tools
    // are inferred from the technologies used to build this website.
    public string[] TechnicalSkills { get; set; } =
        ["Computer skills", "Analytical skills", "Data management", "Basic math"];
    public string[] ProfessionalSkills { get; set; } =
        ["Teamwork", "Written communication", "Adaptability", "Listening skills", "Self-motivation"];

    // Education cards appear in this order, with the highest qualification first.
    // Use completion dates only; descriptions stay empty because none were provided.
    public EducationItem[] Education { get; set; } =
    [
        new("National Diploma in ICT", "Tshwane University of Technology", "Completed: February 2025", ""),
        new("Matric", "Shanke High School", "Completed: 2017", "")
    ];

    // Show confirmed employment in the Experience section, most recent first.
    // Keep the summary separate from the list of practical experience below.
    public ExperienceItem[] Experience { get; set; } =
    [
        new("Support Technician", "Matupunuka ICT", "March 2023 – Present",
            "Providing day-to-day technical support, troubleshooting technical issues, maintaining IT systems and equipment, and ensuring reliable support for users.",
            ["Provide daily technical support and resolve user issues.",
             "Diagnose and troubleshoot hardware, software, and peripheral problems.",
             "Install, configure, and maintain IT equipment and systems.",
             "Perform routine maintenance and technical checks.",
             "Respond to support requests and ensure issues are resolved efficiently."],
            "Key Responsibilities"),
        new("Internship Programme", "Nkosingiphe Inkazimulo Trading and Projects (Pty) Ltd", "March 2022 – February 2023",
            "Completed a MICT-funded internship programme, gaining practical exposure to web and application development and IT support.",
            ["Contributed to the development of a web application for the company.",
             "Developed a money-transfer application designed to give clients access to and control over their funds through web and mobile platforms.",
             "Assisted with maintaining the company's business processes.",
             "Performed upgrading and installation tasks."])
    ];

    // Project 01 contains confirmed internship work; Projects 02 and 03 remain placeholders.
    // Empty technology lists keep unconfirmed tools from appearing on the cards.
    public ProjectItem[] Projects { get; set; } =
    [
        new("01", "Internship Project", "Money Transfer Application",
            "Contributed to a web and mobile money-transfer application during my internship, providing clients with access to and control over their funds.", [], "portfolio", "Internship contribution"),
        new("02", "PROJECT PLACEHOLDER", "Project details to be added",
            "Placeholder: add another confirmed project and describe the work you completed.", [], "dashboard"),
        new("03", "PROJECT PLACEHOLDER", "Project details to be added",
            "Placeholder: add a further project when its details are ready to share.", [], "inventory")
    ];

    // PLACEHOLDER: No certificates, issuers, or award dates have been provided.
    public CertificationItem[] Certifications { get; set; } =
    [
        new("Certification details to be added", "Issuing organization to be added", "Date to be added")
    ];
}

// Each small type groups the fields needed by one card or timeline entry.
// Add items to the lists above; the view creates matching entries automatically.
public record EducationItem(string Qualification, string Institution, string Dates, string Description);
public record ExperienceItem(string Title, string Company, string Dates, string Description, string[] Highlights, string HighlightsHeading = "Key Experience");
// The default note labels placeholders; confirmed projects supply their own note.
public record ProjectItem(string Number, string Category, string Title, string Description, string[] Technologies, string Visual, string Note = "Placeholder · No project claimed");
public record CertificationItem(string Title, string Issuer, string Year);
