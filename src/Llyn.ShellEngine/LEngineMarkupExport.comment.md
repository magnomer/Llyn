# LEngineMarkupExport.cs

## `public sealed partial class LEngine`

The markup half of export.
Stored entries become `LMarkupEntry` records through `LMarkupLoader` and those become one `.llx` file.
Import stands in `LEngineMarkup.cs`, and the two share only the record shapes.

## `public void LEngineMarkupExport(IReadOnlyList<long> ids, string path)`

Writes the entries named by `ids` to `path` as one markup file in the given order.
One session covers every read, so the file describes one moment of the workspace.
A missing id refuses with `LRefusalEntry` before anything is written.
The file is UTF-8 without a byte order mark, as `LMarkup` formats it.

## `private LMarkupEntry LEngineMarkupCreate(long id)`

Loads the markup record for `id` and turns an absent entry into the refusal.
The loader answers null because absence is a fact, and the engine decides it is a refusal.
