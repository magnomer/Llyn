# QLecternHeader.cs
Hash: `129c98ee22f19ead`

## `public sealed class QLecternHeader`

The reading view's header section, drawing the headword, the language pill, its flag and the heart.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and subscribes its own events, so nothing is handed in late.

## `public QLecternHeader(FrameworkElement surface, CDisplay display, CAtelier atelier, CEnvoy envoy)`

Pulls the headword, the language pill, the flag, the globe and the heart from `surface`.
The envoy is the window's, which the catalog's flag load reports its failure through.
The header redraws on both the area's open and close, since a closed area shows the blank entry.
The text, the font, the flag and the heart each subscribe apart, so each makes one Conduct read.
A favorite notice arrives on the engine's thread, so it is marshalled onto `surface` through `QObserver`.

## `private void QLecternFavoriteObserve(object sender, RoutedEventArgs e)`

Hands the heart's new state to the gate, then draws the heart from the stored value it answers.
A refused mark thus springs the heart back, and the display reports the failure.

## `private void QLecternHeaderRefine()`

Draws the shown headword and language.

## `private void QLecternFontRefine()`

The headword takes the pack's font, so the two views never differ in family or size.

## `private async void QLecternFlagRefine()`

The flag comes from `QEnsignImage`, which every tab holding a display reads too.
The first call may await a fetch, so a later entry may be shown before it arrives.
The flag is then drawn for the language the area shows now, not the one opened.
The globe stands in until then, and wherever no flag is known.
A failed flag load is reported by the catalog, so the globe stays and the view goes on.

## `private void QLecternFavoriteRefine()`

Draws the heart from the stored mark of the chosen entry.

## `private void QLecternFavoriteRefine(bool marked)`

Sets the heart to the ready `marked` value.
