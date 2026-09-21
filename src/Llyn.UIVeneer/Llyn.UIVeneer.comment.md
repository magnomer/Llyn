# Llyn.UIVeneer.csproj

## `<PackageReference Include="SharpVectors.Wpf" Version="1.8.4.2" />`

Renders flag and interface SVGs into WPF drawings.
WPF's bitmap decoders can't read SVG on their own.

## `<Content Include="..\..\languages\**\*.json" Link="languages\%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />`

Language packs ship as drop-in files copied next to the executable, not embedded.
So a language can be added or edited without a recompile.
LLanguageLoader reads them from languages/<Lang>/source.json under the application base directory.
Flags are not shipped: a pack declares an ISO country code and the engine downloads the flag on demand.

## `<Resource Include="..\..\assets\icons\**\*.svg" Link="icons\%(RecursiveDir)%(Filename)%(Extension)" />`

The multicolor 3D interface icons are embedded resources.
PIcon converts each SVG into one cached drawing that preserves its fills, gradients, strokes, and opacity.
