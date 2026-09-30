# Llyn.Conduct.csproj

Builds Conduct, which holds every behaviour of the application behind the screen.

## `<TargetFramework>net10.0</TargetFramework>`

Conduct names no window or control, so it builds for every platform.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Conduct.

## `<InternalsVisibleTo Include="Llyn.Internal" />`

The portable behaviour tests reach Conduct internals directly.

## `<InternalsVisibleTo Include="Llyn.Windows" />`

The Windows behaviour tests build `CAtelier` for the deportments they drive.

## `<InternalsVisibleTo Include="Llyn" />`

The host, built as `Llyn`, builds `CAtelier` over the ports it makes.
The constructor naming those ports stays internal, so no driver can build one.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

Conduct reads the engine through the `L*Port` slices alone.
Core and Application arrive through ShellEngine, never by a reference of their own.
