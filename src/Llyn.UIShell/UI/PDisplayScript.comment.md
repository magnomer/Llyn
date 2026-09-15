# PDisplayScript.cs

## `public partial class PDisplay`

The script box of the reading view: the glyph pictures of the shown headword's characters, above the first meaning.
The box draws what the engine has stored and never fetches itself.

## `private void PDisplayScriptStart(long id)`

Asks the engine to fetch what the entry's characters lack, once per entry show.
The engine declines when nothing is missing, a fetch runs, or the character was missed this session.
A failed ask is ignored, and the box then shows what is stored.

## `private void PDisplayScriptShow(long id, string language)`

Reads the pack's styles and, when it lists any, the entry's stored pictures and whether a fetch runs.
The fetch itself is started by the entry show, not here, so a bulletin never starts another.
The rows and the pending flag are handed to the box control, which shows the loading line or hides itself.
A failed read hands it nothing.
The box takes the pack's glyph font, so a character heading a row is drawn as the glyph chips are.
The fetch announces itself as a script bulletin, and this view re-reads the box alone.

## `private void PDisplayScriptClear()`

Hands the box nothing, so it hides.
