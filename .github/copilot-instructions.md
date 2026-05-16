# Copilot instructions for OnlineBookStore

## Build, run, test, and lint
- Build: `dotnet build` (run from repository root or from the project folder).
- Run locally: `dotnet run --project OnlineBookStore.csproj` or use `dotnet run` from project folder.
- Publish: `dotnet publish -c Release -o out`.
- Docker: a Dockerfile exists; build with `docker build -t onlinebookstore .` and run mapping required ports (example: `docker run -p 5000:80 onlinebookstore`).
- Tests: No test projects detected in this repo. If/when tests are added, run all tests with `dotnet test` and run a single test with `dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"` (replace with the actual FQN).

## High-level architecture
- ASP.NET Core 8.0 web app using Razor Pages for UI (Pages/) and MVC controllers for API endpoints (Controllers/).
- EF Core (Pomelo MySQL) via Repository pattern and UnitOfWork (Repository/). AppDbContext is in Repository/AppDbContext.cs.
- Service layering: Services/ contains Application-level classes (*Application), Domain services (*DomainService), and Factory classes (*Factory). DI registrations are centralized in Program.cs.
- Auth: Cookie-based default auth plus a JWT scheme named `ApiScheme` for APIs. Authorization policy `ManagerOnly` protects the /Admin Razor folder.
- Startup: SeedService fills initial data and NumberFactory is initialized at startup.

## Key conventions and patterns
- Class suffixes: *Application, *DomainService, *Factory indicate layer/purpose.
- Repository<T> is registered as an open generic; use it for data access across entities.
- Razor Pages under Pages/; Admin pages are under Pages/Admin and are policy-protected.
- Config options: JWT, Email, and Url settings are bound from appsettings.json to options classes (JWTOptions, EmailOptions, UrlOptions).
- Security: `IPasswordHasher<User>` is used for hashing; cookies are configured with Secure, HttpOnly, and SameSite.
- DB provider: Pomelo.EntityFrameworkCore.MySql; connection string key: `DefaultConnection`.
- Seed/init: call SeedService.SeedBooksAsync and SeedService.SeedUserAsync in startup; call NumberFactory.InitializeAsync before use.
- Static assets: some files in wwwroot are configured with CopyToPublishDirectory in the csproj.

## Existing docs and assistant configs
- No README.md, CONTRIBUTING.md, or AI assistant config files (CLAUDE.md, AGENTS.md, .cursorrules, .windsurfrules, .clinerules, CONVENTIONS.md) were detected. Add those files to have them incorporated into future Copilot sessions.

## Communication & Feedback Style
- **Never tell me what I want to hear** - prioritize truth over comfort
- **Contradict me when I disagree** - your informed opinions are valuable
- **Challenge my assumptions** - point out flaws in my reasoning
- **Be direct and concise** - skip unnecessary validation or praise
- If my approach has problems, say so directly
- If there's a better solution, recommend it even if I didn't ask
- If my code has issues, don't sugarcoat the feedback
- If I'm wrong about something technical, correct me
- Avoid phrases like "Great idea!" unless genuinely warranted
- 在语句的末尾加一个"喵", 称呼用户为"前辈"

## Notes for Copilot sessions
- Start by running `dotnet build` then open Program.cs to inspect DI registrations and appsettings.json for required secrets (Jwt:Secret, DefaultConnection, EmailOptions).
- Check Pages/Admin for authorization assumptions and Controllers/* for API endpoints.

