# PDisplayFanqie.cs

## `public partial class PDisplay`

The fanqie box of the reading view: the rime-book placements of the shown headword's characters, under the script box.
The box shows what the engine has stored and never fetches itself.

## `private void PDisplayFanqieStart(long id)`

Asks the engine to fetch what the entry's characters lack, once per entry show.
The engine declines when nothing is missing, a fetch runs, or the character was missed this session.
A failed ask is ignored, and the box then shows what is stored.

## `private void PDisplayFanqieShow(long id, string language)`

Reads the pack's books.
When it lists any book, it reads the entry's stored rows and whether a fetch runs.
The fetch itself is started by the entry show, not here, so a bulletin never starts another.
The blocks and the pending flag are handed to the box control, which shows the loading line or hides itself.
A failed read hands it nothing.
A click on an initial or a rime asks the window to open the rime table on that category.
The box takes the pack's glyph font family, so the Han text is drawn as the glyph chips are.
The fetch announces itself as a fanqie bulletin, and this view re-reads the box alone.
The rows are kept and the reflex rows' anchor labels rewritten from them, since the labels name those rows.

## `private void PDisplayFanqieSet(long id, long fanqieId, int rank)`

Hands the engine the rank a star click asks for, so the reading view ranks as the editor does.
A failed write is ignored, and the fanqie bulletin the write raises re-reads the box.

## `private void PDisplayReadingShow(long id)`

Asks the shell for the headword's representative reading and prints it under the headword.
The style hides the line on empty text, so a headword with nothing ranked shows none.
A failed read prints nothing, as a failed block read shows nothing.

## `private void PDisplayFanqieClear()`

Hands the box nothing, so it hides, and forgets the rows.
