# PExampleEditor.cs

## `public partial class PExample`

Editing behavior of the Examples panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
Editing an Example means editing that Example, and a sentence meant for one card alone is a new Example on that card.

## Inline notes

### `private LExample? _pTranscriptExample;`

The Example the editor stands on as the store knows it.
It stands empty while the editor is open over an Example nothing has stored.
Comparing the written fields against it is what says whether anything is unsaved.

### `private bool _pTranscriptLoading;`

Set while fields are being filled from a stored Example.
Filling a field raises the same change the user typing raises, and only the second may clear an unreadable mark.

## `private void PLanguageLoad()`

Reads the workspace languages and fixes a default for an Example being written for the first time.
The `example` row requires a language, so the field is never left empty on the way to the store.

## `private void PCitationFind()`

Reads the whole shelf of Sources the citation may point at.
Nothing here creates, changes or deletes a Source.

## `private string PCitationNameRead(LStateValue source)`

The name a cited Source is shown under, falling back to its id when it names itself nowhere.
The catalog, the search and the display all read a Source through this, so all three agree.

## `private void PCitationFreshHandle(object sender, RoutedEventArgs e)`

Creates a Source under the typed name and cites it at once.
The name becomes the Source title, exactly as the card row creates one.
The full Source structure is written in the sources panel, not here.

## `private void PCitationClearHandle(object sender, RoutedEventArgs e)`

Drops the citation.
The Source is left standing, because clearing a pointer is not deleting what it pointed at.

## `private void PTranscriptTranslationHandle(object sender, TextChangedEventArgs e)`

Clears the unreadable mark on the translation as soon as the user types over it.
Typing is a recording, so what stood there as unreadable stops being that the moment it is written.

## `private void PTranscriptApply(LExample? example)`

Fills every field from the stored Example, or empties them for one being written for the first time.
It is also the discard: what the store holds is written back over what the user typed.

## `private bool PTranscriptChangeCheck()`

Whether the editor stands over modifications nothing has saved yet.
The fields are compared one by one, so an Example read back from the store is judged against what stands in the editor.

## `private void PTranscriptStoreHandle(object sender, RoutedEventArgs e)`

Saves the written Example, creating one that has no id yet and rewriting one that has.
Saving changes neither the identifier, nor what quotes the Example, nor the order it takes for any quoter.

## `private void PTranscriptRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the shown Example.
An Example nothing quotes is simply deleted.
One something quotes is named to the user first, and only then detached and deleted in one operation.
