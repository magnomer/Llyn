# Llyn.Infrastructure

This project holds the adapters behind the engine's ports: archives, loaders, files and fetchers.
It stands apart from Core.Windows because it never references the platform.
The project root holds the project file and the file system usher.

## `Llyn.Infrastructure.csproj`

The `SQLitePCLRaw.bundle_e_sqlite3` reference pins version 2.1.13.
Microsoft.Data.Sqlite 9.0.11 pins SQLitePCLRaw 2.1.10, which carries a high-severity SQLite advisory (GHSA-2m69-gcr7-jv3q).
The reference overrides the transitive bundle with a patched version.

`CA1416` is a warning treated as an error.
A Windows-only call turns the build red, so the platform stays out of Infrastructure.

The favorite, marked favorite, star and expand icons are embedded resources.
They ride inside the assembly, as the themes do.
`LPortraitIcon.LPortraitIconRead` reads them, so an exported page embeds them without a file beside the program.
Each logical name is `LPortraitIcon.LPortraitIconPrefix` plus the file name.

## `LUsherFile`

It answers the `LUsher` port from the file system.
The host wraps one in `LUsherShell` and hands that to the rig factory.
Opening a folder or a link lives in the media ring's `LUsherShell`, since a launch is platform work.
`LUsherOpen` throws, because a file usher answers path facts only.

`LUsherPathExist` is true when a file is present, and false for a null path.

`LUsherPathDelete` removes a cached SVG the renderer could not read, so the next fetch replaces it.
A file already gone or held by another process is left as it is, since the next fetch retries anyway.

`LUsherLockCheck` is true for an input or output failure or a refused access.
It is found on the exception or any inner one.
A reader may wrap the failure, so the whole chain is walked.
A missing file or folder answers false, since there is nothing left to keep.

## Stamp

Commit: `92c4426f8506`
- `LUsherFile.cs` `4c0ba06ff2d16b37`
- `Llyn.Infrastructure.csproj` `5c7b158c85d1ec06`
