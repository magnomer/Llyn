# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0</TargetFramework>`

Deportment holds panel state and no control, so it builds without WPF and runs under the test project.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
A deportment holds the engine through the `L*Port` slices it calls, never through `LEngine` itself.
The ring guard lists the ports and handles it may name, and every other engine type is out of reach.
`Llyn.Media` is not referenced, since playback stays a veneer control.
