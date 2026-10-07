# TInterfaceEnginePortrait.cs
Hash: `b9302bb369c7ae87`

## `internal static class TInterfaceEnginePortrait`

The relays for the engine's likeness of an entry or a page, and for its Joplin reads.
Each relay is transparent and carries no test logic of its own.
So a fact reaches the internal facades without the shell's ports.

## `internal static Task TEnginePortraitExport(this LEngine engine, long entryId, string path, LPortraitMedium format, LPortraitLabel label)`

Relays the entry export to `LEnginePortrait.LEnginePortraitExport`.

## `internal static LPortraitPage TEnginePortraitRead(this LEngine engine, long entryId, LPortraitLabel label)`

Relays the entry read to `LEnginePortrait.LEnginePortraitRead`.

## `internal static LPortraitPage TEnginePortraitRead(this LEngine engine, long id, LOwner owner, LPortraitLegend legend)`

Relays the page read of the owner's row `id` to `LEnginePortrait.LEnginePortraitRead`.

## `internal static Task TEnginePortraitPrint(this LEngine engine, long entryId, LPortraitLabel label, LPressTicket ticket)`

Relays the entry print with its press hand-in to `LEnginePortrait.LEnginePortraitPrint`.

## `internal static Task TEnginePortraitPrint(this LEngine engine, long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)`

Relays the page print with its press hand-in to `LEnginePortrait.LEnginePortraitPrint`.

## `internal static LLiveryPage? TLiveryRead(this LEngine engine, long entryId)`

Relays `LEngineLivery.LEngineLiveryRead`, the Joplin page read of one entry.

## `internal static LLiveryLanguage TLiveryRead(this LEngine engine, string language)`

Relays the reconstruction read of `language` to `LEngineLivery.LEngineLiveryRead`.
Its localizer answers each key unchanged, so a fact can assert on keys instead of shipped text.
