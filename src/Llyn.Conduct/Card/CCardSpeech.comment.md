# CCardSpeech.cs
Hash: `662538c4d0cc794c`

## `public sealed class CCardSpeech`

The draft's parts of speech while the user builds the list.
It is split from `CCard` by role, and its gates keep the `CCard` base, as `CDisplaySound` keeps `CDisplay`.
The chips live in the draft, below Conduct, and the desk's speech quill reads them off it for every edit.
It holds only the raw text the user is typing, which the read overlays on the draft's chips.

## `internal CCardSpeech(CDesk desk)`

Takes the editor's desk.
The unit and freedom rows are worded through static port members, so no port is held.
Only the editor builds one, so the constructor is internal.

## `private LTenure? CCardSpeechTenure`

The held tenure for an edit, or null while none is held or the desk fills its controls.
The edit gates check it before they reach the desk's speech quill.

## `public CCategory CCardSpeechSet(string typed)`

The user typed into the part of speech field.
The text is kept, and the speech quill defers it as a pending part with the draft's chips.
It answers the category menu for the text, which also says whether the menu opens.
While no tenure is held it answers an empty, closed menu.

## `public void CCardSpeechAdd(string name)`

The user committed a name, by Enter or by picking it from the catalog.
The typed text clears, since the field is committed with it.

## `public void CCardSpeechRemove(string name)`

The user erased a chip, and the typed text stays pending.

## `public CMarker CCardSpeechRead()`

The chips and the typed text ready to show, as the speech quill settles them against the draft.
A draft changed from elsewhere shows its own chips and clears the typed text.
It carries the category menu for the settled text, so one read paints the whole field.

## `private static CCategory LCategoryRead(LSpeechOffer offer)`

A plain map of the engine's offer onto the menu, holding no rule.

## `internal static LUnit LUnitRowParse(string key)`

The unit a menu key names, for the editor's unit gate.
A key the menu never offered is a programming error, so it throws.

## `private static IReadOnlyList<(string, bool)> LUnitRowScan(IReadOnlyList<LUnit> units, LUnit taken)`

One pair per offered unit, keyed through the static `LEntryPort.LEngineUnitFormat`, the held one marked.
The read hands it out with the field, since the unit dropper heads the field's row.

## `internal static LFreedom LFreedomRowParse(string key)`

The freedom a menu key names, for the editor's freedom gate.
A key the menu never offered is a programming error, so it throws.

## `private static IReadOnlyList<(string, bool)> LFreedomRowScan(IReadOnlyList<LFreedom> freedoms, LFreedom taken)`

One pair per offered freedom, keyed through the static `LEntryPort.LEngineFreedomFormat`, the held one marked.
The read hands it out with the field, since the freedom dropper follows the unit dropper.
