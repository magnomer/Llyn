# TEaselDesk.cs
Hash: `d9a92700b2cd8765`

## `public sealed class TEaselDesk`

Covers the image and video gates over a desk, on the playwright's scenario and on the editor's cards.
A pressed add appends a blank row after the rows held, and the driver hands no card.
A removed row leaves its own list, and the owning card is found below Conduct.
A chosen file lands at once, even under a long delay.
A cancelled file dialog hands null, and the row stays blank.
A chosen file that exists carries its ready address or screen, and a missing one carries none.

## `private const int TEaselDeskHold = 600000;`

A wait no test outlives, so only a request sent at once can show before the fact ends.

## `private static CRepertoire TEaselScenarioPrepare(CAtelier atelier)`

Builds the repertoire and opens a blank scenario on its playwright's desk.

## `private static IReadOnlyList<CImageDraft> TEaselImageRead(CRepertoire repertoire)`

The scenario's image rows as the playwright reads them.

## `private static IReadOnlyList<CVideoDraft> TEaselVideoRead(CRepertoire repertoire)`

The scenario's video rows as the playwright reads them.
