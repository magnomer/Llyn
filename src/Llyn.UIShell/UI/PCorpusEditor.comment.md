# PCorpusEditor.cs

## `public partial class PCorpus`

Editing behavior of the Corpus panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
Editing an Example means editing that Example.
A sentence meant for one card alone is a new Example on that card.
What the controls hold is pushed to a draft the engine keeps.
A crash costs the last keystrokes rather than the sentence.
The panel keeps no copy of a stored Example, and asks the engine what counts as a change.

## Inline notes

### `private bool _pTranscriptLoading;`

Set while fields are being filled from the held sentence.
Filling a field raises the same change the user typing raises, and only the second may clear an unreadable mark.
It also holds the push back, so a fill never writes what it has just read.

## `private void PSpeakerLoad()`

Reads the workspace languages the picker offers.
No default is chosen, because an Example's language is optional and the store accepts none.
Choosing one here would put a language on every sentence the user never named.

## `private void PCitationFind()`

Reads the whole shelf of Sources the citation may point at.
Nothing here creates, changes or deletes a Source.

## `private string PCitationNameRead(LStateValue source)`

The name a cited Source is shown under, falling back to its id when it names itself nowhere.
The catalog, the search and the display all read a Source through this, so all three agree.

## `private void PCitationClearHandle(object sender, RoutedEventArgs e)`

Drops the citation.
The Source is left standing, because clearing a pointer is not deleting what it pointed at.

## `private void PTranscriptTranslationHandle(object sender, TextChangedEventArgs e)`

Clears the unreadable mark on the translation as soon as the user types over it.
Typing is a recording, so what stood there as unreadable stops being that the moment it is written.

## `private void PTranscriptApply(LExample? example)`

Fills every field from the held sentence, or empties them when no draft stands.
The language is taken as the sentence carries it, an empty one included.
Keeping the last shown language would write it onto a sentence imported without one.
The delete control and the usage count follow the stored id the draft names, not the sentence being written.
A sentence the engine has not stored yet can be deleted from nothing.
The control stays off until it names one.

## `private LExample PTranscriptRead(LExample held)`

The held sentence with the control values written over it.
Identity travels on the record given, so the panel never mints or carries an id of its own.

## `private void PCorpusFreshHandle(object sender, RoutedEventArgs e)`

Opens the editor on a sentence nothing has stored, by starting a draft naming no Example.

## `private void PTranscriptDiscardHandle(object sender, RoutedEventArgs e)`

Throws the held sentence away and starts again from the Example it opened on.
Discarding is a new draft rather than a refill, so what was typed leaves the workspace with it.

## `private void PTranscriptStoreHandle(object sender, RoutedEventArgs e)`

Commits the held sentence, creating an Example the draft names none of and rewriting the one it names.
Anything still waiting to be pushed is pushed first, so the commit carries the last keystroke.
Saving changes neither the identifier, nor what quotes the Example, nor the order it takes for any quoter.

## `private void PTranscriptRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the Example the held draft was opened on.
A draft naming none stands over nothing stored, so there is nothing to delete.
An Example nothing quotes is simply deleted.
One something quotes is named to the user first, and only then detached and deleted in one operation.
