# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Run from the repo root unless noted. Target framework is .NET 8 (SDK 10 is installed locally and builds it fine).

- Restore/build the solution: `dotnet build Portfolio.sln` (currently 0 errors, ~16 existing nullable warnings in `Core/Entities`)
- Run the API: `dotnet run --project Portfolio` (from `Portfolio/` also works). Swagger is served at `/swagger` in Development only.
- Apply migrations manually: `dotnet ef database update --project Infrastructure --startup-project Portfolio`
- Add a migration: `dotnet ef migrations add <Name> --project Infrastructure --startup-project Portfolio`
- API smoke testing: the Bruno collection lives in `Portfolio.WebApi/` (`.bru` files, one folder per controller). `Portfolio/Portfolio.http` also exists.
- Bruno upkeep: when a route or a request DTO changes, update the matching `.bru` file in `Portfolio.WebApi/` in the same change.

Tests live in `Tests/Portfolio.Tests.csproj` (xUnit), which is part of `Portfolio.sln`. Run them with `dotnet test Portfolio.sln`. **Docker Desktop must be running**: the integration tests start a throwaway SQL Server 2022 container through Testcontainers (`Tests/Common/PortfolioApiFactory.cs`) and never touch `ProjectDb`. To run only the unit tests (no Docker needed), use `dotnet test Portfolio.sln --filter "FullyQualifiedName~UnitTests"`.

- `Tests/UnitTests/`: no database. Services are tested with NSubstitute mocks, plus `ExceptionHandlingMiddleware`.
- `Tests/IntegrationTests/`: every class is marked `[Collection(ApiCollection.Name)]` and shares one `PortfolioApiFactory` (the real `Program.cs` against the container, with `ValidateOnBuild`/`ValidateScopes` on). Covers repository ownership, DI, auth and the resume API.
- `Tests/Common/`: the factory, HTTP client helpers (use `https://localhost`, because the auth cookie is Secure), `TestHosts.WithSettings` (a separate host with extra configuration, and `X-Test-Client-IP` to choose the caller's IP) and PDF text/link extraction.

When you add a repository method that looks items up by id, add a matching test to `RepositoryOwnershipTests` that checks another user's id returns nothing.

Database: the connection string is `DefaultConnection` in `Portfolio/appsettings.json` (LocalDB `MSSQLLocalDb`, database `ProjectDb`). `ApplicationDbContext.OnConfiguring` has a fallback connection string, but `Program.cs` always configures the context, so the fallback is only used by design-time tools that don't pass the startup project.

## Architecture

Layered, "onion" style, with dependencies pointing inward toward `Core`:

- `Core` (`Portfolio.Core`): entities, DTOs, exceptions, and the repository and service **interfaces** (`Contracts/`). No implementations.
- `Application` (`Portfolio.Application`): service implementations (`Services/`) and the QuestPDF document generators (`Documents/`). References `Core` only, never `Infrastructure`.
- `Infrastructure` (`Portfolio.Infrastructure`): `ApplicationDbContext` (ASP.NET Identity + EF Core SQL Server), repository implementations, the `Migrations/` folder, and `DbInitialiser`.
- `Portfolio` (`Portfolio.WebApi`): the startup project. Controllers, `Program.cs` (DI registration, Identity, CORS, middleware), and `ExceptionHandlingMiddleware`.

Things that are not obvious from any single file:

**Request flow.** Controller → service interface → service in `Application` → repository interface → repository in `Infrastructure`. Each layer registers its own types: repositories, `ApplicationDbContext` and `DbInitialiser` in `Infrastructure/DependencyInjection.cs` (`AddInfrastructure`), and services in `Application/DependencyInjection.cs` (`AddApplication`). `Program.cs` calls both. Everything is scoped. `IResumeGenerationService` is the exception: it is registered as a singleton because it is stateless.

**Users and auth.** `ApplicationUser` is an Identity user with a `Guid` key, and `IdentityRole<Guid>` is used for roles. Auth is cookie-based (not JWT): `ConfigureApplicationCookie` sets `Secure`, takes `SameSite` from `Auth:CookieSameSite` (`Lax` by default, `None` in `appsettings.Development.json` because the dev frontend is cross-site), and turns login/access-denied redirects into 401/403 responses for the API. The security stamp is checked on every request (`ValidationInterval = 0`), so logout (which rotates the stamp, ending every session for the user) and password changes take effect immediately. Logout requires a JSON body (`{}`) as CSRF protection. Controllers read the current user via the `ClaimsPrincipal.GetUserId()` extension and then scope every query by that id. Keep that pattern when adding endpoints; ownership checks are done in the repositories (e.g. `SavedResumeRepository.GetByIdAsync(id, userId)`), not in controllers.

**Resume generation pipeline.** This is the core feature and spans several files:
1. `ResumeDataService` assembles a `ResumeDTO` by fetching the user's title, summary, skills, education, experience, certifications and links by the ids sent in a `ResumeRequest`.
2. `ResumeGenerationService` (`/api/Resume/create-pdf` is anonymous and takes a full `ResumeDTO` body; `/api/Resume/get-resume` is authorised and takes ids) turns that DTO into a PDF.
3. PDFs are QuestPDF documents. `BaseResumePdfGenerator` holds shared layout (page setup, header, social links); `ResumePdfGenerator` (classic) and `SimplifiedResumePdfGenerator` override the body.

**Saved resumes and template types.** A saved resume stores the `ResumeDTO` as serialised JSON in `SavedResume.Data`, plus a `TemplateType` string. The allowed template names are the constants in `Application/Documents/TemplateTypes.cs` (`classic`, `simplified`). Adding a template means updating three places that must agree: the constants, the validation in `SavedResumeService.SaveResumeAsync`, and the `switch` in `SavedResumeService.GetBuilderForTemplate`. Because saved data is a JSON snapshot, changing the DTO shape affects how old saved resumes deserialise.

**Rate limiting.** `Portfolio/RateLimiting/` registers the built-in .NET limiter; limits are in the `RateLimiting` section of `appsettings.json`. `[EnableRateLimiting("auth")]` (per IP) is on login/register/change-password and `[EnableRateLimiting("pdf")]` (per user) on authenticated PDF downloads. The anonymous `create-pdf` demo is marked `[DemoQuota]` (per-IP burst and daily quota) and every PDF action is marked `[PdfGeneration]` (one shared concurrency cap); both are enforced by the global limiter. A new PDF endpoint needs `[PdfGeneration]`. `PortfolioApiFactory` raises every limit, because all test clients share one in-memory IP; tests that check limits start their own host with `TestHosts.WithSettings`.

**Request limits.** `ResumeDTO` has `[MaxLength]` caps (`ResumeLimits` in `Core/Constants`), `create-pdf` also checks the Demo page's item counts (`DemoResumeValidator`), and the PDF/save endpoints have `[RequestSizeLimit]`. A user can keep 50 saved resumes. Links in generated PDFs go through `SafeLink`, so only http(s) links are clickable.

**Seeding.** `DbInitialiser.InitialiseAsync` runs on every startup from `Program.cs` (before the app serves requests). It applies pending migrations and, only in Development or when `Database:SeedOnStartup` is true, seeds Bogus data into empty tables. The `Randomizer.Seed` is fixed so the data is deterministic. Note that `_context.Database.Migrate()` is not awaited; it is synchronous and runs before the seed.

**Errors.** Domain exceptions (`Core/Exceptions/CustomExceptions.cs`, e.g. `NotFoundException`, `TemplateTypeNotImplementedException`) are mapped to HTTP responses by `ExceptionHandlingMiddleware`. Throw these from services rather than returning error codes from controllers.

**QuestPDF licence.** `Program.cs` sets `QuestPDF.Settings.License = LicenseType.Community` at startup. Keep that line if PDF generation is touched.

## Reference

- `README.md` has the setup steps, the database ERD, and the onion-architecture diagram (`POE_OArch_Diagram.png`). The ERD is out of date in places: it calls the table "Contact", but the code uses `ProfessionalLink`. Check `Core/Entities` before relying on it.
- The companion frontend is a Vite/React app on `http://localhost:5173`. CORS allows that origin and `http://localhost:3000`.
