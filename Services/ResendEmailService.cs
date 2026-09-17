using BeginnerMvc.Models;
using Resend;
using System.Text.RegularExpressions;

namespace BeginnerMvc.Services;

// Keep email-provider code separate from the controller and page design.
public class ResendEmailService(
    IResend resend,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    ILogger<ResendEmailService> logger) : IEmailService
{
    public async Task SendAsync(ContactFormViewModel contact, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Fail safely when setup is incomplete. Never include the secret in an error.
        var apiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN");
        var sender = configuration["Resend:FromEmail"];
        if (environment.IsDevelopment())
        {
            // Report presence only, so we can check what this running website inherited.
            logger.LogInformation(
                "Contact email configuration: RESEND_APITOKEN exists: {ApiTokenExists}; Resend__FromEmail exists: {SenderEnvironmentExists}; effective Resend:FromEmail exists: {SenderExists}.",
                !string.IsNullOrWhiteSpace(apiToken),
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Resend__FromEmail")),
                !string.IsNullOrWhiteSpace(sender));
        }
        if (string.IsNullOrWhiteSpace(apiToken))
        {
            if (environment.IsDevelopment())
                logger.LogWarning("Contact email stopped before calling Resend: RESEND_APITOKEN is missing.");
            throw new InvalidOperationException("Email sending is not configured.");
        }

        // Set Resend__FromEmail to an approved sender on your verified Resend domain.
        // There is deliberately no default: a sender must not be guessed.
        if (string.IsNullOrWhiteSpace(sender))
        {
            if (environment.IsDevelopment())
                logger.LogWarning("Contact email stopped before calling Resend: Resend:FromEmail is missing. Configure Resend__FromEmail.");
            throw new InvalidOperationException("Email sending is not configured.");
        }

        var message = new EmailMessage
        {
            From = sender,
            // Keep user input out of the sender and recipient fields.
            // Reply-To lets the owner reply directly to the visitor.
            To = new EmailAddressList { "mnisi5435@gmail.com" },
            ReplyTo = new EmailAddressList { contact.Email.Trim() },
            Subject = "Website contact: " + contact.Subject.Replace('\r', ' ').Replace('\n', ' ').Trim(),
            // Plain text displays visitor input as text, without interpreting HTML.
            TextBody = $"Name: {contact.Name}\nEmail: {contact.Email}\nSubject: {contact.Subject}\n\n{contact.Message}"
        };

        ResendResponse<Guid> response;
        try
        {
            response = await resend.EmailSendAsync(message, cancellationToken);
        }
        catch (ResendException error)
        {
            LogProviderError(error, apiToken, sender, contact);
            throw new InvalidOperationException("Email sending failed.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception error)
        {
            // Transport/parsing exceptions can contain request details. Log only the type.
            if (environment.IsDevelopment())
                logger.LogWarning("Resend call failed without an API response. Exception type: {ExceptionType}.", error.GetType().Name);
            throw new InvalidOperationException("Email sending failed.");
        }

        // An API rejection is a failure too, even when no exception was thrown.
        // Log selected, sanitized details only in Development. Visitors still get
        // the controller's friendly message; never pass an exception to the logger.
        if (!response.Success)
        {
            LogProviderError(response.Exception, apiToken, sender, contact);
            throw new InvalidOperationException("Email sending failed.");
        }
        if (response.Content == Guid.Empty)
        {
            if (environment.IsDevelopment())
                logger.LogWarning("Resend returned success without an email ID.");
            throw new InvalidOperationException("Email sending failed.");
        }
    }

    private void LogProviderError(ResendException? error, string apiToken, string sender, ContactFormViewModel contact)
    {
        if (!environment.IsDevelopment()) return;

        // A provider message is untrusted text: redact credentials and submitted
        // values before logging, and remove control characters to prevent forged log lines.
        var safeMessage = error?.Message ?? "No provider error details were returned.";
        foreach (var value in new[] { apiToken, sender, contact.Name, contact.Email, contact.Subject, contact.Message }
                     .Where(value => !string.IsNullOrWhiteSpace(value)).OrderByDescending(value => value.Length))
        {
            safeMessage = safeMessage.Replace(value, "[redacted]", StringComparison.OrdinalIgnoreCase);
        }
        safeMessage = Regex.Replace(safeMessage, @"\bBearer\s+\S+|\bre_[A-Za-z0-9_\-]+", "[redacted]", RegexOptions.IgnoreCase);
        safeMessage = Regex.Replace(safeMessage, @"[^\s<>]+@[^\s<>]+", "[email redacted]");
        safeMessage = new string(safeMessage.Select(character => char.IsControl(character) ? ' ' : character).ToArray());
        if (safeMessage.Length > 1000) safeMessage = safeMessage[..1000] + "...";

        logger.LogWarning("Resend send failed. HTTP status: {StatusCode}; error type: {ErrorType}; message: {ProviderMessage}",
            (int?)error?.StatusCode, error?.ErrorType.ToString() ?? "Unknown", safeMessage);
    }
}
