# QTranscriptSpeaker.cs
Hash: `a66bc4dc56d5712b`

## `internal sealed class QTranscriptSpeaker`

The speaker chip of the Corpus panel's edit area, with the dropper that picks the held Example's language.
It fills the language menu that every Gloss row of the panel lists.
It subscribes what it paints itself, so the owner `QCorpus` only builds, introduces and closes it.

## `internal QTranscriptSpeaker(UserControl scope)`

Takes the Corpus page, and finds the speaker parts in it by contract ID.
It ties the dropper to its popup, sets the chip's icon, and attaches the row fill of the language list.

## `internal ObservableCollection<PLanguageItem> QTranscriptSpeakerLanguage`

The workspace languages the speaker menu lists.
The owner hands the same list to `QExcerpt` and `QTranscriptGloss`, so every Gloss row offers the same languages.

## `internal void QTranscriptSpeakerIntroduce(CCorpus corpus, CAtelier atelier, CEnvoy envoy)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
It takes the atelier and the envoy rather than the window, since the workspace reload needs only those two.
It subscribes the transcript notice and the draft notice, which both paint the chip.
It subscribes the workspace notice, which reloads the menu, and binds the menu to its list.

## `internal void QTranscriptSpeakerClose()`

Closes the language popup, so it never outlives the window.
The owner's exit Refine calls it.

## `private async void QSpeakerWorkspaceRefine()`

Answers `CCorpusWorkspaceChanged`, raised after the corpus closed its Example on a workspace notice.
The flags and the language menu belong to the old folder, so both are loaded again.
One ensign read loads the flags and answers the languages the menu lists.
The envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `internal void QSpeakerRefine(IReadOnlyList<string> languages)`

Fills the speaker menu with the given workspace languages.
The chip is not redrawn, since a fresh workspace holds no draft.
No default is chosen, because an Example's language is optional and the store accepts none.
The owner's vista Refine calls it with the languages its first load answered.

## `private void QSpeakerObserve(object sender, RoutedEventArgs e)`

Hands the picked language to `CAnthologySpeakerSet`, then closes the dropper.
The chip changes when the draft bulletin returns.

## `private void QSpeakerDropperRefine()`

Closes the speaker dropper after a pick.

## `private void QSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one speaker choice through the language item fill, then subscribes its click.

## `private void QSpeakerShow(CExample example)`

Paints the speaker chip's name and flag from the language the held Example carries, an empty one included.
It runs on every transcript and draft notice, since painting the same language changes nothing.
