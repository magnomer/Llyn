# PGlossMenu.cs

## `public partial class PEditor`

The Gloss gestures of a card's sentence row, turned into requests against the held draft.

## `internal void PGlossAddHandle(object sender, RoutedEventArgs e)`

Adds a Gloss at the end of the row's list, in English.

## `internal void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Gloss the command carries from the sentence row the command was raised on.

## `private void PGlossChangeHandle(PCard card, PSentence row, PGloss gloss, string field)`

Turns one edited field of a Gloss into its request.
Typed text is deferred under a key that names the Gloss, so the redraw skips it while it is pending.
A chosen language is sent at once.

## `private string PGlossLanguageRead()`

English when it is loaded, else the first loaded language, else empty.
English is the language the user renders into, so it is the right first guess.
