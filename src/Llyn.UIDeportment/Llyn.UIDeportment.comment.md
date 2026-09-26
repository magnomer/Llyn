# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

Deportment drives the veneer's controls, so it builds for Windows.
It hosts the WebView2 player of `PScreen`, so it takes the Windows version WebView2 needs.

## `<UseWPF>true</UseWPF>`

Deportment names windows, controls and event arguments directly.
It holds no markup, since every page and dictionary lives in the veneer.

## `<ProjectReference Include="..\Llyn.Conduct\Llyn.Conduct.csproj" />`

Conduct is the only reference, and ShellEngine, Application and Core arrive through it.
The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
A deportment holds the engine through the `L*Port` slices it calls, never through `LEngine` itself.
The ring guard lists the ports and handles it may name, and every other engine type is out of reach.
`Llyn.Core.Windows` is not referenced, since playback runs through WPF and WebView2 controls held here.

## `<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.4191.47" />`

`PScreen` builds its WebView2 player in code, so the package stays in Deportment by the first test of the purge.
The veneer holds only the empty slot the player is put into.

## `<PackageReference Include="SharpVectors.Wpf" Version="1.8.4.2" />`

Renders flag and interface SVGs into WPF drawings.
WPF's bitmap decoders can't read SVG on their own.
The icons it renders are embedded in the veneer, so `PIcon` reads them by pack URI.
