# CCardSpeech.cs
Hash: `8928e1c415977bae`

## `public sealed class CCardSpeech`

The draft's parts of speech while the user builds the list.
It is split from `CCard` by role, and its gates keep the `CCard` base, as `CDisplaySound` keeps `CDisplay`.
The chips live in the draft, below Conduct, and the tenure reads them off it for every edit.
It holds only the raw text the user is typing, which the read overlays on the draft's chips.

## `private LTenure? CCardSpeechTenure`

The held tenure for an edit, or null while none is held or the desk fills its controls.

## `public CCategory CCardSpeechSet(string typed)`

The user typed into the part of speech field.
The text is kept, and the tenure defers it as a pending part with the draft's chips.
It answers the category menu for the text, which also says whether the menu opens.
While no tenure is held it answers an empty, closed menu.

## `public void CCardSpeechAdd(string name)`

The user committed a name, by Enter or by picking it from the catalog.
The typed text clears, since the field is committed with it.

## `public void CCardSpeechRemove(string name)`

The user erased a chip, and the typed text stays pending.

## `public CMarker CCardSpeechRead()`

The chips and the typed text ready to show, as the tenure settles them against the draft.
A draft changed from elsewhere shows its own chips and clears the typed text.
It carries the category menu for the settled text, so one read paints the whole field.

## `private static CCategory LCategoryRead(LSpeechOffer offer)`

A plain map of the engine's offer onto the menu, holding no rule.
