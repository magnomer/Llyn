# LEngineWorkspaceUpdate.cs

## `public sealed partial class LEngine`

The one pass that fills the derived strings a rebuilt workspace holds blank.
A database migrated from an older schema comes across with its new columns empty.
The reading view prints stored strings only, so those columns must be filled before it reads them.
The pass runs once, right after the migration, and never on an ordinary open.

## `private void LEngineWorkspaceUpdate()`

Walks every entry of the workspace in one session and derives what each stores.
Respellings missing on pronunciation and reflex rows, the epithet, and the regular flag of every form.
One session holds the whole pass, so a walk cut short lands nothing half-done.

## `private void LEngineRespellingUpdate(LEntry entry)`

Derives and stores the respelling of every pronunciation row and reflex row of the entry that carries none.
A row already carrying one is left as the user wrote it.
A row whose language has no respelling groups stays blank, as a save would leave it.
The reflex rows are rewritten as one set only when at least one changed.
So the ids and the order stand.
