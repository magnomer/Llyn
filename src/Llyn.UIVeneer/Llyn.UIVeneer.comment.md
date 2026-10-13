# Llyn.UIVeneer

The veneer is a library of pages and resources that names only the deportment.
A page's only C# is its `x:Class` shell.
`Llyn.Host` builds the engine and runs the application, so the veneer names no lower project.

## `DisableTransitiveProjectReferences`

The veneer compiles against the deportment alone, never the rings below it.
A page that names Conduct or the engine fails the build, not only the border audit.

## `ItemGroup`

Every markup file builds as a page by the default glob, `PBootstrap.xaml` included.
No file is named `App.xaml`, so the library carries no generated entry point.
A page the deportment pulls by contract ID carries an `x:Class` shell.
A page the deportment still loads by pack URI names `Llyn.UIVeneer` explicitly, since the application assembly is the host.

## `Resource`

The multicolor 3D interface icons are embedded resources.
`QIcon` converts each SVG into one cached drawing that preserves its fills, gradients, strokes, and opacity.
The brand icon and image are embedded beside them for the windows' title bars.

## `PBootstrap`

It is the application the host creates and runs, with the resources every window shares.
The deportment reaches it through `Application.Current` and never names it.
The file is not named `App.xaml`, so no generated entry point competes with the host's.
Its C# shell only calls `InitializeComponent()`, which merges the shared resources.
The base theme dictionary is merged once for every window.

## Inline notes

### PBootstrap.xaml: `PIconRoot`

It is the folder the icon assets live in, under its contract ID.
The deportment joins an icon name to it, so it never names the veneer itself.

### PBootstrap.xaml: `x:Shared="False"`

`PWindow`, `PSCoinage`, `PSLeave`, `PSCustoms` and `PSPress` are each registered under a contract ID.
Every pull builds a fresh instance, since a window cannot be shown twice.

## Stamp

Commit: `c2f335c331fe`
- `Llyn.UIVeneer.csproj` `b305211001a11aef`
- `PBootstrap.xaml` `c242f215cd17bba8`
- `PBootstrap.xaml.cs` `383f02603bb13df7`
