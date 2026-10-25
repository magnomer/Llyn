# Llyn.UIDemeanor

Demeanor drives the console surface, as Deportment drives the veneer.
`Kit` holds nine folders: `Asset`, `Blueprint`, `Canvas`, `Console`, `Field`, `Keymap`, `Pager`, `Parley` and `Table`.
`Asset` holds the theme, the words and the part contract, and `Blueprint` the loader.
`Canvas` holds the grid and cell widths, and `Console` the port and the loop.
`Parley` holds the sublevel box, `Field` the typed line and `Table` the rows and drops.
`Keymap` holds the key table and `Pager` the paged text with links.
`Panel` holds the bodies that carry a panel's content, so far in `Dialog`.
`Shell` holds the start of the program, its window and its observer.
`Invocation` holds strict mode: the parsed command line and the envoy that answers without a screen.
`Llyn.UIDemeanor.Capsule` is the sibling project that stores that state.
`Llyn.Host` references Demeanor and starts it when the process runs through `Llyn.com`.

## `Llyn.UIDemeanor.csproj`

The console needs no Windows code, so Demeanor is portable and has no twin.
A Windows API fails its build through CA1416.

Demeanor compiles against Conduct and its own Capsule, never the rings Conduct references.
A name from ShellEngine, Application or Core fails the build, not only the border audit.

Capsule keeps the console-only state that outlives a run.
It names no Llyn project, so it opens no path to the engine.
Conduct is the only reference that reaches the engine, and only through Conduct's gates.
Demeanor calls the same gate Deportment calls for the same user action.

`Llyn.Tests.Console` sees Demeanor's internals, so the strict types stay internal and still meet their tests.

## Stamp

Commit: `8c403d1df768`
- `Llyn.UIDemeanor.csproj` `fad815941f44246a`
