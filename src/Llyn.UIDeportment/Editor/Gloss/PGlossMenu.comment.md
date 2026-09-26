# PGlossMenu.cs

## `public partial class PEditor`

The Gloss gestures of a card's sentence row, turned into requests against the held draft.

## `private const string PWindowLanguage = "English";`

The language the program's own text is in, and so the first guess for a new Gloss.

## `internal void PGlossAddHandle(object sender, RoutedEventArgs e)`

Adds a Gloss at the end of the row's list, in English.

## `internal void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Gloss the command carries from the sentence row the command was raised on.

## `private void PGlossChangeHandle(PCard card, PSentence row, PGloss gloss, string field)`

Sends a chosen language at once, since the Gloss row notices its picker.
Typed text never comes this way, because the row holds no copy of it.

## `private void PGlossChangeHandle(PGloss gloss, LStateWritten written)`

Turns the text typed into a Gloss field into its request.
It is deferred through the tenure, keyed by the request's own Gloss, so a later edit replaces the earlier.

## `private (PCard, PSentence)? PSentenceGlossFind(PGloss gloss)`

The card and sentence row a Gloss row sits under, which every request about the Gloss names.

## `private string PGlossLanguageRead()`

English when it is loaded, else the first loaded language, else empty.
English is the language the user renders into, so it is the right first guess.

## `private readonly PLanguageTemplate _pLanguageTemplate`

The language row dictionary, held so its fill can subscribe the row's click.

## `private void PSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one language row through the shared language fill, then shows the circle for a flagless language.
It subscribes the row's click, which picks the speaker.

## `private void PSpeakerAttach()`

Hands the language list its rows and fill, and ties the pill to its menu.
The menu opens and closes with the pill, where a binding followed its check.

## `private ToggleButton PSpeaker`

The language pill is drawn as the reading view draws it, so one entry reads the same in both.
The toggle keeps its arrow and its menu, because here the language is chosen rather than reported.

## `internal async void PSpeakerLoad()`

Loads the flags and the language rows, then shows the editor's language.
It stands here beside the row fill, since the editor file reached its line limit.

## `internal void PSpeakerHandle(object sender, RoutedEventArgs e)`

A language row was picked, so the editor takes that language and the menu closes.
