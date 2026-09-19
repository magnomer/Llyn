# LPhonology.cs

## `public sealed class LPhonology`

The deportment of the phonology panel: the shared panel state and the pronunciation rows it browses.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.

## `public LEditor LPhonologyEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `private int _lPhonologyCount;`

How many rows the last read returned, so the empty notice is a verdict rather than a veneer count.

## `public IReadOnlyList<LCatalogPronunciation> LPhonologyRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, twinned and marked.

## `public void LPhonologyOrderSet(string? choice)`

Hands a chosen ordering to the vista, which saves and announces it.
A sender without a tag names no ordering and is ignored.

## `public Task LPhonologyPortraitPrint(LPortraitLabel label, LPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.
