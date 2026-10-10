# Llyn.Conduct.csproj
Hash: `246ef06664a7ce7e`

Builds the Conduct assembly against ShellEngine, without a platform-specific target framework.

## `<TargetFramework>net10.0</TargetFramework>`

The target framework has no Windows qualifier, keeping this project's target independent of the Windows UI.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Conduct.

## `<InternalsVisibleTo Include="Llyn.Tests.Interface" />`

The relay layer reaches Conduct internals so the behaviour tests need not.

## `<InternalsVisibleTo Include="Llyn.Tests.Conduct" />`

Some Conduct test bodies still reach internal Conduct members directly.

## `<InternalsVisibleTo Include="Llyn.Tests.Windows" />`

The Windows behaviour relay shows an entry through `LDisplayRule`, the internal rules of a display.

## `<InternalsVisibleTo Include="Llyn" />`

The host, built as `Llyn`, builds `CAtelier` over the ports it makes.
The constructor naming those ports stays internal, so no driver can build one.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

The sole direct project reference is ShellEngine, which supplies engine ports and tenure quills.
Core and Application arrive through ShellEngine, never by a reference of their own.
