# PGloss.cs

## `internal sealed class PGloss`

One Gloss row as the UI shows it: the language it is written in and its flag.
The text comes with its unknown mark.
The card editor and the corpus panel both draw it, through the templates in `PThemeGloss.xaml`.
The row raises a property change for every edit and lets its owner decide what to send.
It resolves no state of its own: the text and the mark travel to the engine as written.

## `internal PGloss(ObservableCollection<PLanguageItem> catalog, LGlossDraft draft)`

Takes the language catalog the picker offers and the draft row to show.

## `public ObservableCollection<PLanguageItem> PGlossLanguageCatalog { get; }`

The loaded language packs the picker lists, shared with every other row.

## `public long PGlossId`

The draft id of the row, negative for one the store has not written.

## `public string PGlossLanguage`

The chosen language.
Setting it closes the picker and refreshes the flag.
A null is ignored, because the picker's list clears its selection to null while the catalog reloads.

## `public ImageSource? PGlossFlag`

The flag of the chosen language, or null when none is chosen.

## `public string PGlossText`

The rendering as typed.
Typing clears the unknown mark, because what stood there as unknown stops being that the moment it is written.

## `public bool PGlossUnknown`

Whether the text stands as not known.

## `public bool PGlossLanguageVisible`

Whether the language picker is open.

## `internal LStateWritten PGlossTextRead()`

The text and its mark as written, for a request to carry.

## `internal void PGlossShow(LGlossDraft draft, Func<string, bool> pending)`

Redraws the row from the draft, skipping a field whose edit is still pending.
