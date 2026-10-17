# Llyn.UIDemeanor

Demeanor drives the console surface, as Deportment drives the veneer.
`Kit` holds four folders: `Asset`, `Blueprint`, `Canvas` and `Console`.
`Asset` holds the theme, the words and the part contract, and `Blueprint` the loader.
`Canvas` holds the grid and cell widths, and `Console` the port and the loop.
`Shell` holds the start of the program, its window and its observer.
`Invocation` holds strict mode: the parsed command line and the envoy that answers without a screen.
`Llyn.Host` references Demeanor and starts it when the process runs through `Llyn.com`.

## `Llyn.UIDemeanor.csproj`

The console needs no Windows code, so Demeanor is portable and has no twin.
A Windows API fails its build through CA1416.

Demeanor compiles against Conduct alone, never the rings Conduct references.
A name from ShellEngine, Application or Core fails the build, not only the border audit.

Conduct is the only reference, and the engine is reached only through Conduct's gates.
Demeanor calls the same gate Deportment calls for the same user action.

`Llyn.Tests.Console` sees Demeanor's internals, so the strict types stay internal and still meet their tests.

## Stamp

Commit: `92c4426f8506`
- `Llyn.UIDemeanor.csproj` `eadca67d8ca74413`
