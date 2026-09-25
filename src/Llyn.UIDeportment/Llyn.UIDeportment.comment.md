# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0-windows</TargetFramework>`

Deportment drives the veneer's controls, so it builds for Windows.

## `<UseWPF>true</UseWPF>`

Deportment names windows, controls and event arguments directly.

## `<ProjectReference Include="..\Llyn.Conduct\Llyn.Conduct.csproj" />`

Conduct is the only reference, and ShellEngine, Application and Core arrive through it.
The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
A deportment holds the engine through the `L*Port` slices it calls, never through `LEngine` itself.
The ring guard lists the ports and handles it may name, and every other engine type is out of reach.
`Llyn.Core.Windows` is not referenced, since playback stays a veneer control.
