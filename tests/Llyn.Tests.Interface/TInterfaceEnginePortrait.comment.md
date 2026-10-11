# TInterfaceEnginePortrait.cs
Hash: `0b846a638698dde8`

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

## `internal static Task<LGalleyRelay?> TEnginePressOpen(this LEngine engine, long entryId, LPortraitLabel label)`

Relays the entry print session to `LEnginePortrait.LEnginePressOpen`, which loads the sheet into the rig's press.
The caller disposes the answered relay.
The engine never answers null here.
The relay widens the type, so a fact holds it like the relay a vista print answers.

## `internal static Task<LGalleyRelay?> TEnginePressOpen(this LEngine engine, long id, LOwner owner, LPortraitLegend legend)`

Relays the page print session to `LEnginePortrait.LEnginePressOpen`, which loads the page into the rig's press.
The caller disposes the answered relay.
The engine never answers null here, and the relay widens the type the same way.

## `internal static LLiveryPage? TLiveryRead(this LEngine engine, long entryId)`

Relays `LEngineLivery.LEngineLiveryRead`, the Joplin page read of one entry.
It passes `pending` false, as the Joplin push does.

## `internal static LLiveryLanguage TLiveryRead(this LEngine engine, string language)`

Relays the reconstruction read of `language` to `LEngineLivery.LEngineLiveryRead`.
Its localizer answers each key unchanged, so a fact can assert on keys instead of shipped text.
