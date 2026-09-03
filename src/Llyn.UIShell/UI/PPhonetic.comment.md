# PPhonetic.cs

## `public partial class PEditor : LReceiver`

Pronunciation lookup as the editor shows it.
Opening the menu starts a search for the headword.
Candidates stream in from the engine and fill the list.
Picking one writes it into the pronunciation field.
This is the shell side of `LReceiver`.
The engine calls back on a worker thread.
So every arrival is marshalled onto the dispatcher here.

## Inline notes

### `await _lEngine.LEnginePronunciationFind(word, _pLanguageChoice, this, _pPhoneticCancellation.Token);`

The panel asks and then listens.
The search is over when the receiver is told it is, never when this call returns.
The two are not the same moment.
The engine reports the end through LReceiverLookupFinish.
Reading completion off the awaited task gave the menu a second opinion.
The search is not one the menu runs.

### `catch (Exception)`

Superseded by a newer lookup, or the window closed.
Ignore it.

### `_pPhoneticSearching = false;`

A lookup that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private void PPhoneticUpdate()`

What the menu shows, from the two things it knows.
Those are whether the search is still running, and what has arrived so far.
They are independent, because a source that has already answered does not end the search.
That is why the running line follows the search alone.
Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (candidates)`

The notice is the one line the menu says while it has no rows to show.
It says what it is doing, or that there was nothing to find.
With rows on screen it says nothing.

### `void LReceiver.LReceiverCandidateAdd(LCandidate candidate)`

Each source's arrival is surfaced through LReceiverCandidateAdd.
The running line is already shown for the whole search.
So no per-source update is needed here.
