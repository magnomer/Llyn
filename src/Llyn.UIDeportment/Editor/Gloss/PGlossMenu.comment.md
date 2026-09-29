# PGlossMenu.cs

## `public partial class PEditor`

The Gloss gestures of a card's sentence row, each handed raw to one sentence gate.
Also the editor's language pill and its menu of loaded languages.

## `private void PGlossAddObserve(object sender, RoutedEventArgs e)`

Hands the pressed sentence row to the gloss add gate.
The engine chooses the new Gloss's language and its place.

## `private void PGlossRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the Gloss the command carries, and the row it was raised on, to the gloss remove gate.

## `private void PGlossTextObserve(PGloss gloss, string text)`

Hands the text typed into a Gloss field to the sentence's gloss gate, with the row it stands in.

## `private (PCard, PSentence)? PSentenceGlossFind(PGloss gloss)`

The card and sentence row a Gloss row sits under, which every gate about the Gloss names.

## `private void PSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one language row through the shared language fill, then shows the circle for a flagless language.
It subscribes the row's click, which picks the speaker.

## `private void PSpeakerAttach()`

Hands the language list its rows and fill, and ties the pill to its menu.
The menu opens and closes with the pill, where a binding followed its check.

## `private ToggleButton PSpeaker`

The language pill is drawn as the reading view draws it, so one entry reads the same in both.
The toggle keeps its arrow and its menu, because here the language is chosen rather than reported.

## `internal async void PLanguageRefine()`

Fills the language menu from the one catalog load, which flags the languages and answers them.
Then it repaints the pill's flag and the card links' flags, since their images are now in.
It answers each opened workspace, since the languages are the workspace's.
It stands here beside the row fill, since the editor file reached its line limit.

## `private void PSpeakerObserve(object sender, RoutedEventArgs e)`

Hands the picked language row's name to the editor's language gate, then shuts the pill.

## `private void PSpeakerChoiceRefine()`

Unchecks the pill, so its menu closes once a language is picked.

## `private async void PSpeakerFlagRefine()`

Waits for the flag images, then paints the pill's flag for the draft's language.
