using BeginnerMvc.Models;

namespace BeginnerMvc.Services;

// An interface is a contract: the controller can request a send without knowing
// which provider will handle it.
public interface IEmailService
{
    Task SendAsync(ContactFormViewModel contact, CancellationToken cancellationToken = default);
}
