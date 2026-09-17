# Contact email setup

The form uses the Resend .NET SDK to email `mnisi5345@gmail.com`.

1. Keep the API key in the Windows environment variable `RESEND_APITOKEN`.
   The running website must inherit it. Never put the key in a settings file.
2. Verify your sending domain in the Resend dashboard, then set the Windows
   environment variable `Resend__FromEmail` to your chosen address on that domain.
   The double underscore maps to the ASP.NET Core setting `Resend:FromEmail`.
   No sender is supplied by default.
3. Restart the website (and the terminal or IDE that launches it) after changing
   environment variables so it receives the updated values.
4. Submit the contact form to test delivery after the sender is configured.

Resend also provides `onboarding@resend.dev` for testing, but it can send only to
the email address associated with your Resend account. Use it only if that
account address is `mnisi5345@gmail.com`.

Sender requirements: https://resend.com/docs/knowledge-base/403-error-resend-dev-domain

The visitor's email is used as Reply-To. A success message means Resend accepted
the email; inbox delivery can still be affected by bounces or spam filtering.
Missing settings, API rejections, and network errors produce a friendly error
while preserving the visitor's entries. Logs omit the key, visitor data, and
provider error details.

Build with `dotnet build BeginnerMvc.csproj`.
