# LEngineMarkup.cs

## `public sealed partial class LEngine`

The seam a markup reader attaches to, and nothing more.
The workspace holds no reader, so every import is refused.
The shell keeps its import action, so the boundary the reader lands on stays visible.

## `public IReadOnlyList<LEntry> LEngineMarkupImport(string path)`

Refuses every file, whatever it says.

The path is still held to the same check a reader would hold it to.
A caller that passes nothing is wrong about its own argument rather than early for the feature.
Everything else raises a `NotSupportedException`, which the shell shows as a failed import.

**Parameters**

- `path` — The file the reader would open.

**Returns** — Nothing, because the call never returns.
