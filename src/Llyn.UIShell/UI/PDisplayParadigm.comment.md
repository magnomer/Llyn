# PDisplayParadigm.cs

## `public partial class PDisplay`

The paradigm box of the reading view: the inflected forms of the shown headword.
The box draws the rows the engine already filtered, so the view never judges regularity.

## `private void PDisplayParadigmShow(long id)`

Reads the shown entry's rows and the morphology switch, and a failed read empties the box.
With the switch on it asks the engine to fetch, which declines by itself when nothing is missing or a fetch already runs.
It then asks whether a fetch is running, so an empty row can say it is being looked up rather than lost.
The rows are folded and flagged by the item itself, so the editor's box reads the same way.
The box takes the entry's headword font, read by the language the rows carry.
The fetch announces itself as an inflection bulletin, and this view re-reads the box alone.
