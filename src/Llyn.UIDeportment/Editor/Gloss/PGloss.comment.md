# PGloss.cs

## `internal sealed class PGloss`

One Gloss row as the UI shows it: the language it is written in and its flag.
The text is the engine's value, state and all, and the converter reads its mark.
The card editor and the corpus panel both draw it, through the templates in `PThemeGloss.xaml`.
The row raises a notice for a language pick and lets its owner hand it to the gate.
What is typed into its text field leaves through the owner's own handler.
So the row holds no copy of it.

## `internal PGloss(ObservableCollection<PLanguageItem> catalog, CGlossDraft draft)`

Takes the language catalog the picker offers and the draft row to show.

## `public ObservableCollection<PLanguageItem> PGlossLanguageCatalog { get; }`

The loaded language packs the picker lists, shared with every other row.

## `public long PGlossId`

The draft id of the row, negative for one the store has not written.

## `public string PGlossLanguage`

The chosen language, as the draft holds it.
Only the draft sets it, so a pick shows once the gate's change comes back.

## `public string? PGlossHint`

The key Conduct chose for the label while no language is chosen, or null.

## `public ImageSource? PGlossFlag`

The flag of the chosen language, or null when none is chosen.

## `public CStateValue PGlossText`

The rendering as the draft holds it, set only from the draft.

## `public bool PGlossLanguageVisible`

Whether the language picker is open.

## `internal event Action<PGloss, string>? PGlossPicked;`

A language picked in the row's picker, raw, for the owner to hand to its gate.
The row's own language waits for the draft.

## `internal void PGlossShow(CGlossDraft draft)`

Redraws the row from the draft, leaving a field already reading what the engine holds alone.

## `internal static void PGlossRowApply(FrameworkElement container, object item, string? _)`

Fills the named parts of `Theme.Gloss.Row`, `Theme.Gloss.Field` and `Theme.Gloss.Display` from the row.
A part the template lacks is skipped, so one fill serves all three and the corpus line.
The flag shows the language's flag, and the ring stands in while there is none.
The picker opens under the flag toggle and lists the shared catalog through `PLanguageItem.PLanguageItemApply`.
The picker selects by language name, a path set here so the markup holds none.
The selection handlers are off while the fill sets the selection, so only a pick raises the notice.
The cross takes the row as its parameter and its icon.

## `private static void PGlossNameApply(TextBlock label, PGloss gloss)`

The language name, or the muted hint Conduct chose while none is chosen.

## `private static void PGlossTextApply(FrameworkElement container, CStateValue text)`

Reads the text and its placeholder through the state converter's logic, as the bindings did.
The unknown mark and the hint are read once per fill, so they do not follow a later language switch.

## `private static void PGlossSpeakerRefine(object sender, RoutedEventArgs e)`

A click on the flag toggle opens or shuts the picker on the row.

## `private static void PGlossPopupRefine(object? sender, EventArgs e)`

A picker closed by a click elsewhere marks the row's picker shut.

## `private static void PGlossPickRaise(object sender, SelectionChangedEventArgs e)`

A pick in the picker raises the row's notice with the raw language.
A null selection, which the list shows while the catalog reloads, raises nothing.
The editor hears the notice through the sentence row, and the corpus hears the list itself.

## `private static void PGlossListRefine(object sender, SelectionChangedEventArgs e)`

A pick shuts the picker.
The shut waits for the dispatcher, since it refills the row and would reset the selection.
So every handler of the pick reads the picked language first.
