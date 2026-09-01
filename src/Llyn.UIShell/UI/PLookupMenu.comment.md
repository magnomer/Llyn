# PLookupMenu.cs

## `public partial class PEditor : LReceiver`

Pronunciation lookup as the editor shows it: opening the menu starts a search for the headword, candidates stream in from the engine and fill the list, and picking one writes it into the pronunciation field. This is the shell side of `LReceiver` — the engine calls back on a worker thread, so every arrival is marshalled onto the dispatcher here.

## Inline notes

### `await _lEngine.LEnginePronunciationFind(word, _pLangcodeChoice, this, _pLookupCancellation.Token);`

The panel asks and then listens: the search is over when the receiver is told it is, never when this call returns. The two are not the same moment — the engine reports the end through LReceiverLookupFinish, and reading completion off the awaited task as well gave the menu a second opinion about a search it does not run.

### `catch (Exception)`

Superseded by a newer lookup or the window closed; ignore.

### `_pLookupSearching = false;`

A lookup that could not be started reports no end of its own, so the menu is taken out of its searching state here rather than left running under a search that never began.

### `private void PLookupMenuUpdate()`

What the menu shows, from the two things it knows: whether the search is still running, and what has arrived so far. They are independent — a source that has already answered does not end the search — which is why the running line follows the search alone. Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (candidates)`

The notice is the one line the menu says while it has no rows to show: what it is doing, or that there was nothing to find. With rows on screen it says nothing.

### `void LReceiver.LReceiverCandidateAdd(LCandidate candidate)`

Each source's arrival is surfaced through LReceiverCandidateAdd; the running line is already shown for the whole search, so no per-source update is needed here.
