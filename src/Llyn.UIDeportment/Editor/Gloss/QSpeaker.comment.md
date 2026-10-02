# QSpeaker.cs

## `internal sealed class QSpeaker`

The editor's language pill and its menu of loaded languages.
The pill shows the draft's language, and a pick hands the language to the editor's gate.
It takes the plain scope, since it calls no member of the editor.

## `internal QSpeaker(FrameworkElement surface, ObservableCollection<PLanguageItem> language, QLink link)`

Hands the language list its rows and fill, and ties the pill to its menu.
The menu opens and closes with the pill, where a binding followed its check.
The rows are the editor's one language list, which every card's Gloss picker also shows.
The Translation driver is held for its flags, which wait on the same catalog load.

## `private ToggleButton QSpeakerSwitch`

The language pill is drawn as the reading view draws it, so one entry reads the same in both.
The toggle keeps its arrow and its menu, because here the language is chosen rather than reported.

## `internal void QSpeakerIntroduce(CEditor editor, CAtelier atelier)`

Holds the Conduct editor and the window's atelier, whose catalog draws the flags.
Subscribes the draft's language Refine and the language menu's fill for each opened workspace.

## `private void QSpeakerRefine(CEntryDraft _)`

The draft's language name and its flag.

## `private async void QSpeakerLanguageRefine()`

Fills the language menu from the one catalog load, which flags the languages and answers them.
Then it repaints the pill's flag and the card links' flags, since their images are now in.
It answers each opened workspace, since the languages are the workspace's.

## `private void QSpeakerObserve(object sender, RoutedEventArgs e)`

Hands the picked language row's name to the editor's language gate, then shuts the pill.

## `private void QSpeakerChoiceRefine()`

Unchecks the pill, so its menu closes once a language is picked.

## `private async void QSpeakerFlagRefine()`

Waits for the flag images, then paints the pill's flag for the draft's language.

## `private void QSpeakerEnsignRefine()`

Paints the speaker's flag, or its globe, from the drawings already in the store.
The language menu calls it after its own load, so it asks for no second fill.

## `private void QSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one language row through the shared language fill, then shows the circle for a flagless language.
It subscribes the row's click, which picks the speaker.
