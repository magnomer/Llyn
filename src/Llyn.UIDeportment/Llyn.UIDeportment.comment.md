# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0</TargetFramework>`

Deportment holds panel state and no control, so it builds without WPF and runs under the test project.

## `<ProjectReference Include="..\Llyn.ShellEngine\Llyn.ShellEngine.csproj" />`

The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
`Llyn.Media` is not referenced, since playback stays a veneer control.
