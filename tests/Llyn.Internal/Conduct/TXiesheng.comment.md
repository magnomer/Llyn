# TXiesheng.cs

## `public sealed class TXiesheng`

Covers the xiesheng panel's gates end to end on a real workspace.
The series are fetched from fake pages, so nothing here reaches the web.

The bundled series pack shows the tab, and with nothing fetched the panel lists nothing under the bare keys.
Opening a fetched series by key lists it chosen with its count, and shows its page and entry.
The page carries the headword and glyph fonts of the series language, and the blank page carries none.
A blank or unknown key opens nothing.
An arrival empties the series search before the driver hears of it.
Choosing the chosen series unchooses it, choosing it again brings it back, and a workspace change clears it.
A glyph opens in the library tab only while a series page shows, in the language of that page.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its series.
A null order keeps the chosen one.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
An entry opened in scribe mode shows in the editor, and closing the panel empties it.

## `internal static CXiesheng TXieshengPrepare(CAtelier atelier)`

Builds the xiesheng panel and restores its vistas, as the window does for the tab.

## `internal static LEngine TXieshengEngineStart(TWorkspace workspace)`

Starts the engine with fake pages for the rime book and the series module of one character.

## `internal static async Task<LEntry> TXieshengStemSave(LEngine engine, string language)`

Stores one entry in the series pack's language and waits for its series fetch to settle.
