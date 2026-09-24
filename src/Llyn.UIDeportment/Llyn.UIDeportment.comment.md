# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0-windows</TargetFramework>`

Deportment drives the veneer's controls, so it builds for Windows.

## `<UseWPF>true</UseWPF>`

Deportment names windows, controls and event arguments directly.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
A deportment holds the engine through the `L*Port` slices it calls, never through `LEngine` itself.
The ring guard lists the ports and handles it may name, and every other engine type is out of reach.
`Llyn.Media` is not referenced, since playback stays a veneer control.
