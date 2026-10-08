# LEntryPort.cs
Hash: `c26f5d2b58d9b842`

## `public interface LEntryPort`

The slice of the engine a deportment sees when it loads one stored entry.
It loads the entry, reads its stamps, its Source tally and its written reflexes.
It also opens the entry a glyph cell names, words a lexical unit and places a card in its list.
`LEntryFacade` implements it itself, so no outlet forwards between Conduct and the engine.
Member names keep the engine's `LEngine*` form.

## `LEntryDraft? LEngineEntryLoad(long id);`

The stored entry as a draft, or null when it no longer stands.
Its recordings come resolved to absolute paths.

## `(bool, string, string) LEngineStampRead(long entryId);`

Whether an entry is stored, with its worded creation and update times, empty once it is gone.
The engine parses and words the stored stamps, so Conduct only copies them.

## `string LEngineTallyRead(long? reference);`

The usage tally line of one Source, worded as none when no Source is given.

## `IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);`

The written reflex rows of the draft, which a reading view lists.

## `long LEngineGlyphResolve(string character, string language);`

The id of the entry a glyph cell opens, made when none exists yet.

## `string LEngineUnitFormat(LUnit unit);`

The localization key that names a lexical unit, empty when none is chosen.

## `(LOwner, int)? LEngineCardFind(LEntryDraft draft, long id);`

Which card list of `draft` holds the card `id`, and at which place, or null when neither does.
It reads only the draft the caller holds.
