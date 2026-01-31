# StreamSuites Creator Desktop

StreamSuites Creator Desktop is a lightweight Windows shell that hosts the StreamSuites™ Creator web experience in a dedicated desktop window.

## Architecture summary
- .NET 8 WPF application
- Microsoft WebView2 for rendering the Creator web app
- Thin shell only with no business logic or native feature layering
- Persistent WebView2 user data stored under `%LOCALAPPDATA%\StreamSuites\CreatorDesktop\UserData`

## Security model overview
- Authentication and authorization are handled by the web application at `https://creator.streamsuites.app`.
- The desktop app does not implement any local authentication, session logic, or API integrations.
- All sensitive operations and data handling are governed by the web app and its backend services.

## Source of truth
The StreamSuites Creator web dashboard is the single source of truth for data, permissions, and workflows. The desktop app only hosts that web experience.

## Explicit non-goals
- No admin access or admin-only shortcuts
- No desktop-only authentication or identity model
- No offline or cached business logic
- No backend API assumptions or integrations
