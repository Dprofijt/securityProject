# Copilot Instructions for this Repository - .NET 10 and C#

This repository contains a C# application (`SecurityCVEDashboard`) and should follow clean, maintainable, testable patterns.

## Target stack
- Language: C# (latest stable supported by .NET 10)
- Runtime: .NET 10
- UI: Blazor (Razor components)
- Primary goals: readability, security, testability, and low coupling

## High-level project structure
When adding new code, prefer this logical structure under `SecurityCVEDashboard/`:

- `Domain/`
  - Entities, value objects, domain services, domain exceptions
  - No framework dependencies
- `Application/`
  - Use-cases, DTOs, interfaces (ports), validation, orchestration logic
  - Depends on `Domain`, not on infrastructure details
- `Infrastructure/`
  - External integrations (HTTP clients, persistence, file/system adapters, auth providers)
  - Implementations of application interfaces
- `Components/`
  - UI components and pages only (presentation concerns)
  - Keep business logic out of Razor code-behind where possible
- `Common/` or `Shared/` (optional)
  - Cross-cutting concerns: result types, constants, utilities, abstractions used broadly

If introducing these folders incrementally, do not perform large-scale moves unless explicitly requested.

## SOLID rules (must follow)

### 1) Single Responsibility Principle (SRP)
- One class should have one reason to change.
- Keep UI rendering, business rules, and data access in separate classes.
- Avoid "god classes" and overgrown component code-behind files.

### 2) Open/Closed Principle (OCP)
- Prefer extension via interfaces and composition over modifying stable core behavior.
- Introduce strategy/policy abstractions for behavior variants.

### 3) Liskov Substitution Principle (LSP)
- Derived implementations must preserve contract behavior.
- Avoid interface implementations that throw `NotSupportedException` for normal flows.

### 4) Interface Segregation Principle (ISP)
- Use small, purpose-specific interfaces.
- Avoid broad interfaces forcing implementations to depend on unused members.

### 5) Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions.
- Register implementations in DI (`Program.cs`) and inject interfaces into consumers.
- Never let domain logic depend directly on infrastructure APIs.

## Dependency direction and boundaries
- `Components` -> `Application` (allowed)
- `Application` -> `Domain` (allowed)
- `Infrastructure` -> `Application` + external libs (allowed)
- `Domain` -> no dependency on `Application`, `Infrastructure`, or UI frameworks
- `Components` should not call infrastructure classes directly.

## C# coding conventions
- Use file-scoped namespaces.
- Prefer `sealed` for classes not intended for inheritance.
- Keep methods short; extract named private methods when a method grows beyond a few logical steps.
- Use meaningful names; avoid abbreviations except common standards.
- Use `async`/`await` end-to-end for I/O.
- Accept `CancellationToken` in async methods where applicable.
- Validate inputs early and fail fast with clear exceptions/messages.
- Avoid static mutable state.
- Avoid workarounds and fallbacks that obscure intent; prefer explicit handling of edge cases.

## Error handling and resilience
- Do not swallow exceptions.
- Convert technical failures into user-safe messages at UI boundaries.
- Log with enough context for diagnosis, without leaking secrets.
- For outbound calls, apply timeout/retry policies when appropriate.

## Security baseline
- Treat all external input as untrusted.
- Validate and sanitize user input.
- Never hardcode secrets, tokens, or connection strings.
- Use configuration + secret providers for sensitive settings.
- Follow least-privilege for integrations and credentials.

## Blazor-specific guidance
- Keep `.razor` pages focused on rendering and interaction wiring.
- Move business logic to application services.
- Keep component state minimal and explicit.
- Reuse components; avoid duplicated markup/logic.

## Testing expectations
When adding/changing behavior:
- Add or update tests for the changed behavior (happy path + edge case).
- Unit test domain and application logic first.
- Keep UI tests focused on critical user flows.
- Mock external boundaries, not domain behavior.

## Pull request quality gates
Before finalizing changes, ensure:
- Build passes.
- Existing tests pass.
- New behavior has test coverage where feasible.
- No obvious layering or SOLID violations introduced.
- No secrets added to source control.

## Copilot behavior for this repo
When generating code in this repository:
1. Respect the layering and dependency direction above.
2. Propose minimal, focused changes instead of broad refactors.
3. Prefer interface-first design at boundaries.
4. Include tests when public behavior changes.
5. Explain trade-offs briefly when multiple valid approaches exist.

## Example (good direction)
- A Razor page calls `IGetCveOverviewUseCase` from `Application`.
- The use-case depends on `ICveRepository` (interface in `Application`).
- `Infrastructure` provides `CveRepository` implementation using HTTP/DB.
- `Program.cs` wires interface to implementation through DI.

This keeps concerns separated and aligns with SOLID and clean architecture principles.
