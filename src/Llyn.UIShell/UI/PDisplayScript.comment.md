# PDisplayScript.cs

## `public partial class PDisplay`

The script box of the reading view: the glyph pictures of the shown headword's characters, above the first meaning.
The box draws what the engine has stored and never fetches itself.

## `private void PDisplayScriptShow(long id, string language)`

Reads the pack's styles and, when it lists any, the entry's stored pictures.
The read itself starts a fetch for what is missing.
The engine declines when nothing is missing or a fetch runs.
A failed read or an empty result hides the box.
The box takes the pack's glyph font, so a character heading a row is drawn as the glyph chips are.
The fetch announces itself as a script bulletin, and this view re-reads the box alone.

## `private void PDisplayScriptClear()`

Drops the rows and hides the box.
