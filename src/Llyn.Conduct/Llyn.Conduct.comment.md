# Llyn.Conduct.csproj
Hash: `246ef06664a7ce7e`

Builds Conduct, which holds every behaviour of the application behind the screen.

## `<TargetFramework>net10.0</TargetFramework>`

Conduct names no window or control, so it builds for every platform.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Conduct.

## `<InternalsVisibleTo Include="Llyn.Tests.Interface" />`

The relay layer reaches Conduct internals so the behaviour tests need not.

## `<InternalsVisibleTo Include="Llyn.Tests.Conduct" />`

Some Conduct test bodies still reach internal Conduct members directly.

## `<InternalsVisibleTo Include="Llyn.Tests.Windows" />`

The Windows behaviour tests build `CAtelier` for the deportments they drive.

## `<InternalsVisibleTo Include="Llyn" />`

The host, built as `Llyn`, builds `CAtelier` over the ports it makes.
The constructor naming those ports stays internal, so no driver can build one.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

Conduct reads the engine through the `L*Port` slices alone.
Core and Application arrive through ShellEngine, never by a reference of their own.
