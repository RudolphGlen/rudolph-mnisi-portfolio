# MyFirstWebsite — your resume and portfolio

This is a .NET 9 ASP.NET Core MVC website. It includes Home, About, Skills, Education, Work Experience, Projects, Certifications, and Contact sections. The navy, white, and teal design adapts to desktop, tablet, and mobile.

## Run it

The app uses SQLite for Identity and portfolio content. On first start it applies EF Core migrations, creates the initial Admin account, and adds the current profile and education entries if they are not already in the database. Configure the account once with .NET User Secrets before starting the app. From the project folder, run:

```powershell
dotnet user-secrets set "AdminSettings:Email" "you@example.com"
dotnet user-secrets set "AdminSettings:Password" "Choose-A-Strong-Password"
dotnet build
dotnet run --launch-profile http
```

Replace the sample email and password with your own. User Secrets are stored outside the repository and are for local development. Hosting setup is covered separately in a lesson project. The password is used only to create the initial account; changing that setting later does not reset an existing account's password.

The build command checks the code. The run command starts the web server. Leave that terminal open, then visit **http://localhost:5176** in a browser. Sign in at **/Admin/Login**. Press **Ctrl+C** in the server's terminal to stop it. After five failed sign-in attempts, the admin account is locked for five minutes.

If it is already running, use the existing server or stop it before starting another copy. If another program uses that address, run `dotnet run --launch-profile http --urls http://localhost:5177` and visit http://localhost:5177.

You can use `dotnet watch --launch-profile http` while editing to automatically apply changes or restart the website. If changes do not appear, refresh your browser.

Optional HTTPS: run `dotnet dev-certs https --trust` and accept the Windows certificate prompt, then run `dotnet run --launch-profile https` and visit https://localhost:7174. Local development permits HTTP; production redirects HTTP to HTTPS.

## Update your professional details

Sign in at **/Admin/Login** and use the dashboard to edit public content. **Profile** manages the Home introduction, About paragraphs, public contact details, locations, availability label, profile image path, and CV link. **Education** lets you add, edit, delete, and reorder education cards. The Projects, Skills, Experience, and Certifications pages manage those sections. Only publish details you have confirmed and want to make public.

### Add a photo

Put your photo at **wwwroot/images/profile.jpg**, then enter `/images/profile.jpg` in Admin Dashboard → Profile → Profile image path. Leave the field blank to show your initials instead.

The image description uses your current profile name. Use a clear portrait with enough space around your face.

### Add your CV

Add a public professional PDF under **wwwroot/documents/**, then enter its path (for example, `/documents/my-cv.pdf`) in Admin Dashboard → Profile → CV file path. Exclude identity, document and exam numbers, full home addresses, private records, and identity-document copies. Both Download CV links use this profile setting.

### Contact form and email setup

The contact form validates submissions on the server and sends them through Resend. It currently allows five submissions per detected client IP in each 15-minute window; further requests receive HTTP 429 with a retry message. A hidden honeypot field also silently discards basic bot submissions that fill it. This is a lightweight filter, not a substitute for CAPTCHA or a dedicated spam service if abuse continues. The rate limit is held in application memory and resets when the app restarts; it is intended for this local, single-instance project.

To enable delivery in a local PowerShell session, set the API token and a sender address verified with Resend before running the app:

```powershell
$env:RESEND_APITOKEN = "your-resend-api-token"
$env:Resend__FromEmail = "portfolio@your-verified-domain.example"
dotnet run --launch-profile http
```

Use your own verified sender address; the example domain will not send. The API token is read from `RESEND_APITOKEN`, and the sender from `Resend__FromEmail`. The destination inbox is currently set in `Services/ResendEmailService.cs`.

## What the important files do

MVC means Model, View, Controller: information, page design, and the code connecting them.

| File | What it does |
| --- | --- |
| Models/HomeViewModel.cs | Combines database content for the public home page. |
| Models/SiteProfile.cs | Defines the editable Home, About, and Contact profile data. |
| Models/Education.cs | Defines each editable education card and its display order. |
| Controllers/HomeController.cs | Loads portfolio records from SQLite and handles contact form submissions. |
| Controllers/AdminController.cs | Protects the admin area and manages profile and portfolio records. |
| Data/DbSeeder.cs | Creates the initial admin account and role, and seeds starting profile and education data. |
| Views/Home/Index.cshtml | Builds all eight sections using the model. Loops repeat cards for each item. |
| Views/Admin/Profile.cshtml | Edits the public profile, About text, contact details, and image/CV paths. |
| Views/Admin/Education.cshtml | Adds, edits, deletes, and reorders education records. |
| Views/Shared/_Layout.cshtml | Shares the navigation, page title, stylesheet, and footer across pages. |
| wwwroot/css/site.css | Controls all colors, spacing, cards, responsive layouts, focus indicators, and reduced-motion behavior. |
| wwwroot/js/site.js | Opens the mobile menu and highlights sections. |
| wwwroot/documents/my-cv.pdf | Downloadable sample PDF; replace with your real CV. |
| Views/Home/Privacy.cshtml | Explains privacy and technical logging. Update when adding new services. |
| Views/Shared/Error.cshtml | Displays a friendly error message and tracking ID. |
| Views/_ViewImports.cshtml | Makes model names and MVC link helpers available to views. |
| Views/_ViewStart.cshtml | Applies the shared layout automatically. |
| Program.cs | Starts the server, serves public files, and connects addresses to controller actions. |
| BeginnerMvc.csproj | The existing project file, still targeting .NET 9. The visible site name is MyFirstWebsite; keeping the original internal project name avoids unnecessary changes. |
| appsettings.json | General application settings and logging levels. |
| appsettings.Development.json | Non-secret settings for local development. |
| Properties/launchSettings.json | Local HTTP and HTTPS addresses. |
| global.json | Selects a compatible .NET 9 SDK. |

The old layout stylesheet is kept as a short explanatory comment; all styling now lives in wwwroot/css/site.css. Bootstrap and jQuery files from the starter are still available, but this design does not need to load them. No extra runtime packages are required.

Comments explain the important parts of the C# code, Razor views, CSS, and JavaScript. JSON configuration files are explained here rather than adding comments to their contents.

## Accessibility

The site includes a skip link, semantic headings and sections, visible keyboard focus, labeled form fields, mobile menu state announcements, and reduced-motion support. Navigation remains visible when JavaScript is disabled. The contact form submits through the server and does not depend on JavaScript.

To check your edits: build the site, inspect it at narrow and wide browser sizes, try navigating with Tab and Enter, download the CV, and try an invalid and a valid contact form submission after configuring Resend.


## Public information only

Do not add identity numbers, document numbers, exam numbers, full street/home addresses, identity-document copies, or private records anywhere in this project. Files inside wwwroot are publicly downloadable, including PDFs and photos. Use only confirmed professional content and clearly marked placeholders. No private source documents are needed for these edits.
