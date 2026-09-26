# LPortraitFacade.cs

## `internal sealed class LPortraitFacade`

The engine's facade for portrait: the entry page and the kind pages, their export and their print.
The entry page is composed by the portrait clerk, the kind pages by the example, reference and situation clerks.

## `public LPortraitFacade(LEngine engine)`

The facade bound to its engine and the engine's gate.

## `internal LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)`

The page of one entry, composed by the portrait clerk under the gate.

## `internal LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)`

The likeness of one example, source or situation, composed by the clerk of the realm `owner` names.
Each realm is read under the engine's lock, so screen and page show one thing.
A missing row is an error, because a caller asked to portray one that no longer stands.
## `internal Task LEnginePortraitExport(long entryId, string path, LPortraitMedium format, LPortraitLabel label)`

Markup is written through the markup clerk under the gate.
Every other format composes the page and hands it to the portrait clerk, which prints a PDF through the press.

## `internal Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)`

The entry page handed to the press with the ticket.

## `public Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label)`

The export of the entry a vista has chosen, nothing when the vista holds no entry.

## `public Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket)`

The print of the entry a vista has chosen, nothing when the vista holds no entry.

## `public Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket)`

The print of the example, situation or reference a catalog vista has chosen.
Any other subject is a caller mistake and throws.

## `internal Task LEnginePortraitPrint(long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)`

One kind page handed to the press with the ticket.

