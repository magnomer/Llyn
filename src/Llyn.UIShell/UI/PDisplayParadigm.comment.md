# PDisplayParadigm.cs

## `public partial class PDisplay`

The paradigm box of the reading view: the inflected forms of the shown headword.
The box draws the rows the engine already filtered, so the view never judges regularity.

## `private void PDisplayParadigmShow(long id)`

Reads the shown entry's rows and the morphology switch, and a failed read empties the box.
With the switch on it asks the engine to fetch, which declines by itself when nothing is missing or a fetch already runs.
It then asks whether a fetch is running, so an empty row can say it is being looked up rather than lost.
Consecutive slots of one part sharing one stored form fold into one row.
The part heads a row only when more than one part inflects, and only on its first row.
The box takes the entry's headword font, read by the language the rows carry.
The fetch announces itself as an inflection bulletin, and this view re-reads the box alone.

## `private static bool PDisplayParadigmMatch(LParadigmSlot one, LParadigmSlot other)`

Reports whether two slots belong to one part and are both specified with the same stored text.
Such slots fold into one row.
