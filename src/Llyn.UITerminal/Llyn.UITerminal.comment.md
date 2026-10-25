# Llyn.UITerminal

The console surface, the second UI beside the WPF veneer.
It holds the blueprints of the console screens and the one class that loads them.
Demeanor drives the screens, and this project only says where each part sits.

## `Llyn.UITerminal.csproj`

UITerminal compiles against Demeanor alone, never Conduct or the rings below it.
A name past Demeanor fails the build, not only the border audit.

Demeanor is the only reference, as Deportment is the veneer's only reference.
A UITerminal member only calls a function, and Demeanor drives it.
It targets the portable framework, and a Windows API fails its build through CA1416.
It embeds `ABootstrap.xaml` and the blueprints `AWindow`, `ANotice`, `ASParley`, `ASNavigation`, `ASPathfinder` and `ASDrive` as resources.
The loader finds a blueprint among the embedded resources, so a file left out of the project is never read.

## `ABootstrap`

The shell of the console UI, which the host hands to `BBootstrap`.
Its constructor loads the blueprints and glyph sets of `ABootstrap.xaml` and holds no other logic.
A broken or missing blueprint throws there, so the start fails before any screen.

## `ABootstrap.xaml`

The shell's list of the blueprints to load and the glyph sets they may draw with.
It lists the same six blueprints in that order, and a blueprint left out of the list is not loaded.
Each glyph set holds six frame marks, three rule marks and the marks of the UI's symbols.
The `double` set draws the window, the `single` set uses thin lines, and the `ascii` set needs no box drawing.
A mark with several states lists one cell per state, such as `Grasp` with its three star steps.
`Tree` lists the branch, last-branch and trunk cells that the etymology map draws.
Every set has a mark for every symbol, so a screen may switch sets without a missing glyph.

## Stamp

Commit: `cb8ab7e8feb4`
- `ABootstrap.cs` `ffad35cfe3757da5`
- `ABootstrap.xaml` `5458370522a30897`
- `Llyn.UITerminal.csproj` `86e4ea580199520e`
