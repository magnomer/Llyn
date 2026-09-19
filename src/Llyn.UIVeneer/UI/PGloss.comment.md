# PGloss.cs

## `internal sealed class PGloss`

One Gloss row as the UI shows it: the language it is written in and its flag.
The text is the engine's value, state and all, and the converter reads its mark.
The card editor and the corpus panel both draw it, through the templates in `PThemeGloss.xaml`.
The row raises a property change for a language pick and lets its owner decide what to send.
What is typed into its text field leaves through the owner's own handler.
So the row holds no copy of it.

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

## `public LStateValue PGlossText`

The rendering as the draft holds it, set only from the draft.

## `public bool PGlossLanguageVisible`

Whether the language picker is open.

## `internal void PGlossShow(LGlossDraft draft)`

Redraws the row from the draft, leaving a field already reading what the engine holds alone.
