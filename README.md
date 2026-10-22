# MyFirstWebsite — your resume and portfolio

This is a .NET 9 ASP.NET Core MVC website. It includes Home, About, Skills, Education, Work Experience, Projects, Certifications, and Contact sections. The navy, white, and teal design adapts to desktop, tablet, and mobile.

## Run it

The app uses SQLite for Identity and portfolio content. On first start it applies EF Core migrations and creates the initial Admin account. Configure the account once with .NET User Secrets before starting the app. From the project folder, run:

```powershell
dotnet user-secrets set "AdminSettings:Email" "you@example.com"
dotnet user-secrets set "AdminSettings:Password" "Choose-A-Strong-Password"
dotnet build
dotnet run --launch-profile http
```

Replace the sample email and password with your own. User Secrets are stored outside the repository and are for local development. For a hosted deployment, provide `AdminSettings__Email` and `AdminSettings__Password` through the hosting provider's secret configuration. The password is used only to create the initial account; changing that setting later does not reset an existing account's password.

The build command checks the code. The run command starts the web server. Leave that terminal open, then visit **http://localhost:5176** in a browser. Sign in at **/Admin/Login**. Press **Ctrl+C** in the server's terminal to stop it. After five failed sign-in attempts, the admin account is locked for five minutes.

If it is already running, use the existing server or stop it before starting another copy. If another program uses that address, run `dotnet run --launch-profile http --urls http://localhost:5177` and visit http://localhost:5177.

You can use `dotnet watch --launch-profile http` while editing to automatically apply changes or restart the website. If changes do not appear, refresh your browser.

Optional HTTPS: run `dotnet dev-certs https --trust` and accept the Windows certificate prompt, then run `dotnet run --launch-profile https` and visit https://localhost:7174. Local development permits HTTP; production redirects HTTP to HTTPS.

## Update your professional details

**Start with Models/HomeViewModel.cs.** It contains Rudolph Mnisi's supplied professional information: IT, software development, technical support, InvesthooIT experience (2022–2023), the supplied skills, and Witbank, Mpumalanga, South Africa. Search for **PLACEHOLDER** to find missing details. Education, project, certification, email, and phone details have not been provided; the website does not invent them.

- Update Name, Initials, ProfessionalTitle, Introduction, Summary, and AboutMore only with confirmed information.
- Add your public professional Email, Phone, and PhoneLink (digits with country code) when ready. Empty values display placeholders without clickable contact links. Keep Location limited to Witbank, Mpumalanga, South Africa.
- The skill lists contain only your supplied skills. Add other skills only when confirmed.
- Edit the Education, Experience, Projects, and Certifications lists. Copy an existing item to add another entry; keep commas between entries.
- Keep text inside quotation marks. To include a quotation mark inside text, write \".
- Remove the relevant placeholder notices in Views/Home/Index.cshtml only after providing real details. Keep the contact form's demo notice until real delivery is implemented.

### Add a photo

Put your photo at **wwwroot/images/profile.jpg**. In Models/HomeViewModel.cs, change:

```csharp
public string ProfileImageUrl { get; set; } = "/images/profile.jpg";
```

Until then, the website displays your initials in a professional photo area. The image description automatically includes your name. Use a clear portrait with enough space around your face.

### Add your CV

Replace **wwwroot/documents/my-cv.pdf** with a public professional version of your CV, keeping the filename. Exclude identity, document and exam numbers, full home addresses, private records, and identity-document copies. Both Download CV links will then download your file. The existing file is a valid PDF clearly labeled as a placeholder.

### Contact form and email setup

The contact form validates submissions on the server and sends them through Resend. To enable delivery in a local PowerShell session, set the API token and a sender address verified with Resend before running the app:

```powershell
$env:RESEND_APITOKEN = "your-resend-api-token"
$env:Resend__FromEmail = "portfolio@your-verified-domain.example"
dotnet run --launch-profile http
```

Use your own verified sender address; the example domain will not send. Configure the same names as environment variables in your hosting provider for deployment. The API token is read from `RESEND_APITOKEN`, and the sender from `Resend__FromEmail`. The destination inbox is currently set in `Services/ResendEmailService.cs`. The form sends messages directly, so add rate limiting or equivalent abuse protection before exposing it publicly.

## What the important files do

MVC means Model, View, Controller: information, page design, and the code connecting them.

| File | What it does |
| --- | --- |
| Models/HomeViewModel.cs | Stores confirmed professional information and clearly marked placeholders and defines the shape of each card or timeline item. Start editing here. |
| Controllers/HomeController.cs | Loads portfolio records from SQLite and handles contact form submissions. |
| Controllers/AdminController.cs | Protects the admin area and manages portfolio records. |
| Data/DbSeeder.cs | Creates the initial admin account and role from configuration. |
| Views/Home/Index.cshtml | Builds all eight sections using the model. Loops repeat cards for each item. |
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
