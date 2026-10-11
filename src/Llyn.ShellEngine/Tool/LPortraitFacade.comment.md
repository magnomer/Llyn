# LPortraitFacade.cs
Hash: `77018d523cca1d13`

## `internal sealed class LPortraitFacade`

The engine's facade for portrait, covering the entry page and the kind pages, their print and the entry's export.
The entry page is composed by the portrait clerk, the kind pages by the example, reference and situation clerks.

## `public LPortraitFacade(LEngineHearth hearth, LLiveryFacade livery, LSettingsFacade settings)`

Stores the hearth, its gate and the two sibling facades that export and print read.
`LEngine` builds it after `LEngineLivery`, since the entry export reads the Livery page.
The settings facade answers the localized text the entry sheet words its labels with.
It also formats the file name a print session offers for saving.
The gate, the staff and the shared state are read through the hearth.

## `internal LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)`

The page of one entry, composed by the portrait clerk under the gate.

## `internal LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)`

The likeness of one example, reference or situation, composed by the clerk of the realm `owner` names.
Each realm is read under the engine's lock, so screen and page show one thing.
A missing row is an error, because a caller asked to portray one that no longer stands.

## `internal Task LEnginePortraitExport(long entryId, string path, LPortraitMedium format, LPortraitLabel label)`

Markup is written through the markup clerk under the gate.
Every other format composes the portrait page and hands it to the press clerk.
HTML and PDF also hand the Livery page, read with `pending` true.
So the file shows a fill in flight as view mode does.
A missing Livery page throws, as a missing kind page does.
Markdown and Word keep the portrait page alone.
The press clerk prints a PDF through the press and saves each other format itself.

## `public Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label)`

The export of the entry a vista has chosen, nothing when the vista holds no entry.

## `internal async Task<LGalleyRelay> LEnginePressOpen(long entryId, LPortraitLabel label)`

Opens a print session on one entry's sheet, the same sheet its print and HTML export write.
It reads the Livery page with `pending` true, and a missing one throws.
The PDF name is formatted before the board opens, so a failure there leaves no board behind.
No `ConfigureAwait(false)` is used, because the board's browser is bound to the opening thread.
The press clerk answers a session clerk around the board, so the facade names no Core contract.
The relay owns that session clerk, and the galley it answers reads it.
The galley keeps the press clerk to list printers when its window shows.
It also keeps the print setting clerk, which runs every setting rule and the store.

## `public async Task<LGalleyRelay?> LEnginePressOpen(LVista? vista, LPortraitLabel label)`

The print session of the entry a vista has chosen, null when the vista holds no entry.

## `public async Task<LGalleyRelay?> LEnginePressOpen(LVista? vista, LPortraitLegend legend)`

The print session of the example, situation or reference a catalog vista has chosen.
It answers null when the vista holds no choice.
Any other subject is a caller mistake and throws.

## `internal async Task<LGalleyRelay> LEnginePressOpen(long id, LOwner owner, LPortraitLegend legend)`

Opens a print session on one kind page, offered for saving under the page title.
The relay and its galley keep the same clerks as the entry session does.

## `private LEngineStaff LPortraitFacadeStaff`

The engine's held staff, read afresh at each use.
