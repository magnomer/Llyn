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

## `internal static void PGlossRowApply(FrameworkElement container, object item, string? _)`

Fills the named parts of `Theme.Gloss.Row`, `Theme.Gloss.Field` and `Theme.Gloss.Display` from the row.
A part the template lacks is skipped, so one fill serves all three and the corpus line.
The flag shows the language's flag, and the ring stands in while there is none.
The picker opens under the flag toggle and lists the shared catalog through `PLanguageItem.PLanguageItemApply`.
The picker selects by language name, a path set here so the markup holds none.
The selection handler is off while the fill sets the selection, so only a pick reaches the row.
The cross takes the row as its parameter and its icon.

## `private static void PGlossNameApply(TextBlock label, string language)`

The language name, or the muted language label while none is chosen.

## `private static void PGlossTextApply(FrameworkElement container, LStateValue text)`

Reads the text and its placeholder through the state converter's logic, as the bindings did.
The unknown mark and the hint are read once per fill, so they do not follow a later language switch.

## `private static void PGlossSpeakerHandle(object sender, RoutedEventArgs e)`

A click on the flag toggle opens or shuts the picker on the row.

## `private static void PGlossPopupHandle(object? sender, EventArgs e)`

A picker closed by a click elsewhere marks the row's picker shut.

## `private static void PGlossListHandle(object sender, SelectionChangedEventArgs e)`

A pick in the picker sets the row's language.
