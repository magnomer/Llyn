# LPhonology.cs

## `public sealed class LPhonology`

The deportment of the phonology panel: the shared panel state and the pronunciation rows it browses.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.
The panel's loads and clears go straight to the editor's lectern, so the veneer relays no draft.

## `public LEditor LPhonologyEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `private int _lPhonologyCount;`

How many rows the last read returned, so the empty notice is a verdict rather than a veneer count.

## `public IReadOnlyList<LCatalogPronunciation> LPhonologyRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, twinned and marked.

## `public void LPhonologyOrderSet(LCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves and announces it.
The veneer hands the enum its row carries, so no ordering is spelled or parsed.
A sender that is no order row hands null, which keeps the ordering it has.

## `public Task LPhonologyPortraitPrint(LPortraitLabel label, LPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.
