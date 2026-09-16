# LEngineAnatomy.cs

## `public sealed partial class LEngine`

The anatomy side of the engine: the onset, vowel, coda and tone every reflex row carries, in IPA and respelling.
The rules come from the pack of the entry's own language, so only a Classical Chinese entry has any.
It runs on every seam that writes a reflex row, so a view only reads what is stored.

## `public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)`

The tone correspondence rows the pack named by `language` declares, empty for a blank name or a pack without them.
The anchor dropdown reads them to mark the placements a reflex reading's contour may descend from.

## `private LReflexDraft LEngineAnatomyResolve(string language, LReflexDraft row)`

The row with its anatomy cut afresh under the rules of the pack named by `language`, the entry's language.
A blank language, a pack without rules or a reflex language no rule names leaves the anatomy empty.

## `private IReadOnlyList<LReflexDraft> LEngineAnatomyScan(string language, IReadOnlyList<LReflexDraft> rows)`

Every row resolved in turn, order and ids kept.

## `private static IReadOnlyList<LReflex> LEngineAnatomyClear(IReadOnlyList<LReflex> rows)`

The rows with their anatomy blanked, so the reflex sync can compare two lists on what the user can see.

## `private LEntryDraft LEngineAnatomyRebuild(LEntryDraft content)`

The draft with every reflex row resolved under its current language, run when the language changes.
