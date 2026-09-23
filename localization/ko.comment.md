# ko.json

The Korean interface catalog, with the same shape and rules as `en.json`.

## Adding a language

A new catalog is listed from the embedded resource names, so no code names a language.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.
