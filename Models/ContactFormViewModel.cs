using System.ComponentModel.DataAnnotations;

namespace BeginnerMvc.Models;

// MVC checks these rules on the server, even when browser validation is bypassed.
// Required also rejects values made entirely of spaces. Length limits bound input size.
public class ContactFormViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, ErrorMessage = "Your name must be 100 characters or fewer.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(254, ErrorMessage = "Your email address must be 254 characters or fewer.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please enter a subject.")]
    [StringLength(150, ErrorMessage = "The subject must be 150 characters or fewer.")]
    public string Subject { get; set; } = "";

    [Required(ErrorMessage = "Please enter a message.")]
    [StringLength(3000, ErrorMessage = "The message must be 3,000 characters or fewer.")]
    public string Message { get; set; } = "";
}
