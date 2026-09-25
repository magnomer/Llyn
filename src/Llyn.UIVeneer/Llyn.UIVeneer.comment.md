# Llyn.UIVeneer.csproj

## `<ProjectReference Include="..\Llyn.UIDeportment\Llyn.UIDeportment.csproj" />`

The veneer is a library that names only the deportment.
`Llyn.Host` builds the engine and runs the application, so the veneer names no lower project.

## `<Page Include="App.xaml" />`

`App.xaml` builds as a page, so the library carries no generated entry point.
The host creates the application and calls its `InitializeComponent` itself.
Every pack URI names `Llyn.UIVeneer` explicitly, since the application assembly is now the host.

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
