# TSchemaMigrationReflex.cs
Hash: `afa08e2720b02717`

## `public sealed class TSchemaMigrationReflex`

Covers how the rebuild carries the reflex rows an older build wrote.
A build older than the note drops every reflex row and keeps its entries.
A build older than the region keeps each row, its old note becoming the romanization.
A version-71 reflex carries its old note into romanization and its old remark into the new note.
Its new meaning is empty and its ownership flag is clear.
It builds each workspace through `TWorkspace.TWorkspacePrepare`.
