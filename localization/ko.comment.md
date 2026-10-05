# ko.json
Hash: `d0b79f4c7c09ae29`

The Korean interface catalog, with the same shape and rules as `en.json`.
Each failure text sits beside its area's other texts at the same place as in `en.json`.
A failure area with no other texts, such as `Sentence` or `Language`, sits where `en.json` puts it.

## Adding a language

A new catalog is listed from the embedded resource names, so no code names a language.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.
