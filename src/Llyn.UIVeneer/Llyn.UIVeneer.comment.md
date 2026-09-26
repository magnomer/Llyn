# Llyn.UIVeneer.csproj

## `<ProjectReference Include="..\Llyn.UIDeportment\Llyn.UIDeportment.csproj" />`

The veneer is a library that names only the deportment.
Every view, template and theme dictionary has returned from the purge, disentangled.
The veneer holds no C# file, only pages and resources.
`Llyn.Host` builds the engine and runs the application, so the veneer names no lower project.

## `<Page Include="App.xaml" />`

`App.xaml` builds as a page, so the library carries no generated entry point.
Every other markup file builds as a page by the default glob.
None carries `x:Class`, since the deportment loads each one by pack URI.
Every pack URI to a returned file names `Llyn.UIVeneer` explicitly, since the application assembly is the host.

## `<Resource Include="..\..\assets\icons\**\*.svg" Link="icons\%(RecursiveDir)%(Filename)%(Extension)" />`

The multicolor 3D interface icons are embedded resources.
`PIcon` converts each SVG into one cached drawing that preserves its fills, gradients, strokes, and opacity.
The brand icon and image are embedded beside them for the windows' title bars.
