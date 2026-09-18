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
