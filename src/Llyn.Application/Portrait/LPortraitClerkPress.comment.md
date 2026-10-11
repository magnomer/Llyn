# LPortraitClerkPress.cs
Hash: `fdf397463713192a`

## `public sealed class LPortraitClerkPress`

A composed page sent out, either saved in a format or printed through the press.
It composes no page itself, so the entry page and the catalog pages share it.

## `public LPortraitClerkPress(LRig rig)`

Reads the portrait port, the press and the printer queue out of `rig`.

## `public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)`

A PDF goes through the press as rendered sheet HTML.
Every other format is saved through the portrait port.

## `public Task<LGalleyClerk> LPortraitClerkOpen(LPortraitPage page)`

A catalog page rendered as sheet HTML and loaded into a kept press board.
The board stays open for the whole print session, so each setting change redraws without reloading the sheet.
The answer is the session clerk that holds the board, so the session never names the board.
The caller owns the clerk and disposes it when the session ends.

## `public async Task LPortraitClerkExport(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup, string path, LPortraitMedium format)`

An entry's export, which reads the Livery page beside the portrait page.
HTML is saved through the portrait port's entry sheet, and PDF prints that same sheet through the press.
Markup, Markdown and Word keep the portrait page alone, through the other overload.

## `public Task<LGalleyClerk> LPortraitClerkOpen(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup)`

An entry's sheet, the same one its print and HTML export write, loaded into a kept press board.

## `private async Task<LGalleyClerk> LPortraitClerkOpen(string html)`

Opens the board on `html`, wraps it in a session clerk and lists no printers.
A broken spooler then cannot stop a print window from opening.
The await keeps the caller's context, so the board's browser stays on the thread that opened it.

## `public async Task<IReadOnlyList<LPressPrinter>> LPortraitPrinterRead()`

Lists the installed printers through `LRigQueue`, each with its sheets made ready.
The print session calls it when its window shows, apart from opening the board.
A listing failure travels to the caller unchanged, so the window can report it.

## `private static LPressPrinter LPortraitPrinterRead(LPressPrinter printer)`

The printer with its sheets turned upright, repeats dropped and standard sheets named.
Upright sheets let a sheet a driver lists wide match its standard name.
A sheet keeps the driver's own size and only borrows the standard name.
The driver's size is what the printer actually holds, so a custom print lands on a real form.
The twin only maps the driver's values, so this rule lives here.

## `public static IReadOnlyList<(LPortraitMedium, string, bool)> LPortraitMediumRead()`

Every format an entry can be exported to, in the order a chooser offers them.
Each row carries the file suffix the format is written under, and whether it is the default.
HTML is the default.
