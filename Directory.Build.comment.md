# Directory.Build.props

Imported by every project in the repository, so its properties reach each assembly.

## Version properties

`version.json` is the single source of the version.
`current-version` is read at compile time and stamped into every assembly's version metadata.
Runtime code reads the version back through reflection on the entry assembly.
A missing or unreadable file stamps `0.0.0` instead of failing the build.

## `AllowedReferenceRelatedFileExtensions`

NuGet packages ship IntelliSense `.xml` files that single-file publish would leave beside the executable.
Only `.pdb` is copied, so the install folder stays clean.
