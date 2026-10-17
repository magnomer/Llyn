# Llyn.Launcher

The console launcher, published as `Llyn.com` beside `Llyn.exe`.
It stands apart from Host because it must run with no .NET runtime beside it.
A single-file `Llyn.exe` carries the runtime and every assembly inside itself.
So a plain apphost named `Llyn.com` would find no `Llyn.dll` to load.
So this project is compiled ahead of time to native code.
It is one small file that needs nothing beside it.
It names no Llyn project, and no Llyn project names it.

## `Llyn.Launcher.csproj`

`PublishAot` makes the native executable, and the size settings keep it near one megabyte.
Its own name is `Llyn.Launcher.exe`, so it never collides with Host's `Llyn.exe`.
Host's publish renames it to `Llyn.com` as it copies it beside `Llyn.exe`.
It takes the brand icon, so both launchers look the same in a terminal and in Explorer.

## `LConsoleUsher`

`LConsoleUsherRun` starts `Llyn.exe` from its own folder and returns that process's exit code.
`LConsoleUsherShorten` cuts its own name off the command line and keeps the rest as typed.
So every argument reaches `Llyn.exe` unparsed.
It hands its three standard handles to the child, so a redirect or a pipe still reaches the CUI.
It sets `LLYN_LAUNCHER` to `console` in the child's environment.
That mark is how Host knows the console was asked for.
It ignores Ctrl+C itself, so only the CUI answers the key.
`STARTUPINFOW` and `PROCESS_INFORMATION` are byte buffers written at their x64 offsets, so no struct is declared.
`LConsoleUsherStartup` and `LConsoleUsherInformation` are their sizes.
`LConsoleUsherFlags` and `LConsoleUsherStreams` are the offsets of the flags and the three standard handles.
`LConsoleUsherHandles` is `STARTF_USESTDHANDLES`, which makes the child take those handles.
`Main` is the entry point the language requires.
Host already uses top-level statements, and only one file of a program may.
The audit binder compiles every source as one program, so this file cannot use them too.

## Stamp

Commit: `05a1bb950841`
- `LConsoleUsher.cs` `e5fce6367809bbde`
- `Llyn.Launcher.csproj` `5344bd36181b3a62`
