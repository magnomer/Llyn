# PNotationItem.cs

## `internal sealed class PNotationItem : INotifyPropertyChanged`

One row of the pronunciation menu, standing for one source.
The row is created the moment its source starts searching, so every declared source is visible from the first frame.
It carries the source's declared position, which is where the menu keeps it whatever order the network answers in.
It is resolved in place rather than replaced, so a row never jumps as the search fills in.
A source may answer several times, once per variety, and each answer becomes one more reading on the row.

## `public ObservableCollection<PNotationReading> PNotationItemReading`

The readings this source gave so far, in the order they arrived, empty while none has.

## `public string PNotationItemNotice`

The one line a row without a reading says: searching, no entry, or failed to retrieve.

## `public bool PNotationItemReady`

Whether the row carries a reading the user can take now, which is what `PClipItemReady` means on an audio row.
It is what the template switches the readings and the notice on.

## `internal void PNotationItemShow(LCandidate candidate, PNotationReading? reading, string missing, string broken)`

Resolves the row from one thing its source said.
A reading is appended and makes the row takeable.
The editor builds the reading, because its label and flag come from the window and the language pack.
No transcription reads as `missing` when the source answered and `broken` when it never did.
An empty answer after a reading has landed changes nothing, so a found row never falls back to a notice.

## `internal static void PNotationItemApply(FrameworkElement container, object item, RoutedEventHandler select)`

Fills one source row: its name, its readings, and the italic line that stands in for them.
The readings show once the row is ready, and the line shows until then.
It attaches the reading fill to the row's own list, handing on the selection handler.
