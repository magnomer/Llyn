# TEntrySave.cs

## `public sealed class TEntrySave`

Covers the one seam that writes an entry.
The whole input form goes in as a single engine call, and every row it produces reads back.
A blank headword is refused before anything is opened.
A failure part-way through leaves no entry row behind.
The save is one unit of work or none.

## Inline notes

### `IReadOnlyList<LMeaning> meanings = new LMeaningArchive(workspace.TWorkspaceDatabase).LMeaningRead(entry.LEntryId);`

The meanings land in card order, each at the position its card held.

### `LCollocation collocation = Assert.Single(`

The collocation carries both of its fields: the expression and the meaning that explains it.

### `LRevision? revision = new LRevisionArchive(workspace.TWorkspaceDatabase).LRevisionLatestRead();`

The save is recorded as history and the workspace row is moved onto it.

### `Assert.Equal(`

Each field became a row of its own that the card now references, not a column on the card.

### `Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));`

The meaning card's Synonym text is deliberately dropped.
The relation it would become targets an Entry or a Meaning by id.
The field holds free text, so nothing can resolve it yet.

### `LEntry second = engine.LEngineEntrySave(draft);`

Saving the same text again matches nothing.
Every non-empty field creates a new row.
So the second entry references rows of its own rather than the first entry's.

### `LCardDraft blank = new(`

What the form always hands over.
That is one seeded Meaning card and one seeded Collocation card, both untouched.
It refuses to remove a list's last card.

### `LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);`

An entry with no cards loads as cleanly as it saved.

### `LEntry entry = engine.LEngineEntrySave(new LEntryDraft(`

A card of whitespace counts as blank, on the same terms a card field does.

### `LRefusal refusal = Assert.Throws<LRefusal>(() => engine.LEngineEntrySave(`

The refusal names its reason with a localization key.
So the shell can present it in the interface language.
The engine carries no sentence of its own.

### `workspace.TWorkspaceScriptRun("DROP TABLE revision_change; DROP TABLE revision;");`

The revision is written after the entry and everything under it.
So removing the table it needs fails the save at its last step.
The entry, its meaning, and its note are already written inside the session.
The engine is built first, or creating it would restore the table.
