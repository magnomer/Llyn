# PExampleEditor.cs

## `public partial class PExample`

Editing behavior of the Examples panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
Editing an Example means editing that Example, and a sentence meant for one card alone is a new Example on that card.

## Inline notes

### `private LExample? _pEditorExample;`

The Example the editor stands on as the store knows it.
It stands empty while the editor is open over an Example nothing has stored.
Comparing the written fields against it is what says whether anything is unsaved.

### `private bool _pEditorLoading;`

Set while fields are being filled from a stored Example.
Filling a field raises the same change the user typing raises, and only the second may clear an unreadable mark.

## `public ObservableCollection<string> PExampleLanguage { get; }`

The workspace languages, offered to every rendition row through the panel itself.
A row inside a template cannot reach the panel's private fields, so this stands public for the binding.

## `private void PLangcodeLoad()`

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

## `private void PRenditionMoveHandle(object sender, RoutedEventArgs e)`

Moves one rendition within the order.
Reordering changes the position and nothing else, so an id anything holds stays valid.

## `private IReadOnlyList<LRendition> PRenditionRead()`

The renditions as written, in list order, skipping a row with no text or no language.
A half-written row is not a rendition, and the store would not know what to make of one.

## `private void PEditorApply(LExample? example)`

Fills every field from the stored Example, or empties them for one being written for the first time.
It is also the discard: what the store holds is written back over what the user typed.

## `private bool PEditorChangeCheck()`

Whether the editor stands over modifications nothing has saved yet.
The record cannot be compared whole, because two rendition lists holding equal rows are not the same object.
So the fields are compared one by one and the renditions in order.

## `private void PStoreHandle(object sender, RoutedEventArgs e)`

Saves the written Example, creating one that has no id yet and rewriting one that has.
Saving changes neither the identifier, nor what quotes the Example, nor the order it takes for any quoter.

## `private void PRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the shown Example.
An Example nothing quotes is simply deleted.
One something quotes is named to the user first, and only then detached and deleted in one operation.
