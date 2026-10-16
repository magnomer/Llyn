# Llyn.UIDemeanor

Demeanor drives the console surface, as Deportment drives the veneer.
`Kit` holds the loop and the console port, and `Shell` the start of the program.
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

Commit: `ed7d18d90760`
- `Llyn.UIDemeanor.csproj` `eadca67d8ca74413`
