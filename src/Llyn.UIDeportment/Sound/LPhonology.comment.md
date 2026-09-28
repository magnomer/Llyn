# LPhonology.cs

## `public sealed class LPhonology`

The deportment of the phonology panel: the shared panel state and the pronunciation rows it browses.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.
The panel's loads and clears go straight to the lectern its view hands in, so the veneer relays no draft.
Its constructor and vista restore are internal, so only the forge builds it and only the window starts its vista.

## `public LEditor LPhonologyEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `private int _lPhonologyCount;`

How many rows the last read returned, so the empty notice is a verdict rather than a veneer count.

## `public IReadOnlyList<CCatalogPronunciation> LPhonologyRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, twinned and marked.
They are handed on as Conduct shapes, so the driver names no Core row.

## `internal static IReadOnlyList<CCatalogPronunciation> LPhonologyPronunciationRead(`

Maps each engine row to its shape: the entry as a `CVistaRow` and the sound beside it.
A missing epithet becomes an empty one, as `CPanel.CPanelRowRead` reads it.

## `public void LPhonologyOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves and announces it.
The veneer hands the enum its row carries, so no ordering is spelled or parsed.
A sender that is no order row hands null, which keeps the ordering it has.

## `public Task LPhonologyPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.
The label and ticket shapes are mapped back to engine types through `CPortrait`.

## `public Task LPhonologyPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the panel's vista to a file, with the medium and label mapped through `CPortrait`.
