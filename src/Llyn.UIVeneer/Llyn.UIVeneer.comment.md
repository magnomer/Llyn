# Llyn.UIVeneer.csproj

## `<ProjectReference Include="..\Llyn.UIDeportment\Llyn.UIDeportment.csproj" />`

The veneer is a library that names only the deportment.
Every view, template and theme dictionary has returned from the purge, disentangled.
The veneer holds pages and resources, and a page's only C# is its `x:Class` shell.
`Llyn.Host` builds the engine and runs the application, so the veneer names no lower project.

## `<ItemGroup>`

Every markup file builds as a page by the default glob, `PBootstrap.xaml` included.
No file is named `App.xaml`, so the library carries no generated entry point.
A page the deportment pulls by contract ID carries an `x:Class` shell.
A page the deportment still loads by pack URI names `Llyn.UIVeneer` explicitly, since the application assembly is the host.

## `<Resource Include="..\..\assets\icons\**\*.svg" Link="icons\%(RecursiveDir)%(Filename)%(Extension)" />`

The multicolor 3D interface icons are embedded resources.
`QIcon` converts each SVG into one cached drawing that preserves its fills, gradients, strokes, and opacity.
The brand icon and image are embedded beside them for the windows' title bars.
