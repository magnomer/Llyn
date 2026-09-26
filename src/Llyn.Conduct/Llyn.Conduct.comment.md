# Llyn.Conduct.csproj

Builds the view logic that holds each panel's state, decisions and engine reads.

## `<TargetFramework>net10.0</TargetFramework>`

Conduct names no window or control, so it builds for every platform.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Conduct.

## `<InternalsVisibleTo Include="Llyn.Internal" />`

The portable behaviour tests reach Conduct internals directly.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

Conduct reads the engine through the `L*Port` slices alone.
Core and Application arrive through ShellEngine, never by a reference of their own.
