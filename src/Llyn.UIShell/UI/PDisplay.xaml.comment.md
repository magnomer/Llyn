# PDisplay.xaml.cs

## `public partial class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.

## `internal void PDisplayAttach(PWindow host, LEngine engine)`

Puts the view on `engine`, the workspace the window opened.
The engine is read for the flag beside the language, the linked headwords and the incoming cards.
The window is held because an incoming row opens the entry it names.

## `internal void PDisplayShow(string id, LEntryDraft draft)`

Draws `draft` as the entry being read.
A field the draft left empty collapses instead of standing as a blank line.
The play button appears only when the entry owns a recording that is still on disk.
The id is taken as well as the draft, because a draft does not say which entry it is.
That id is what the incoming cards are looked up by.

## `internal void PDisplayClear()`

Empties the view and leaves the unselected notice in its place.
Playback stops, because what it was playing belonged to the entry that was shown.

## `internal Action? PDisplayFavoriteDispatcher { get; set; }`

Told to the panel hosting the view whenever a mark or an unmark lands.
The favorites panel re-reads its roster on it, so an unmarked row leaves at once.
A panel that shows no favorite catalog sets nothing and hears nothing.

## `internal void PDisplayClose()`

Releases this view's own playback.

## Inline notes

### `private void PDisplayCardUpdate()`

The chips a card shows are read through a converter that is filled just before they are drawn.
A converter holding a dictionary says nothing when that dictionary changes.
So the card lists are handed their items again once the words behind the ids are known.
That way the order of the two steps cannot quietly cost the reader every chip.

### `private readonly MediaPlayer _pDisplayPlayer = new();`

This view's own playback.
Each panel that hosts a display gets its own player with the view.
A player shared across panels made one panel's clearing stop another panel's sound.

### `private string? _pDisplayRecording;`

Full path of the audio the shown entry owns, or null when it has none.
That is what the play button plays.

### `private void PDisplayFavoriteShow(string id)`

Reads whether the shown entry is marked and sets the star to match.
An unreadable mark leaves the star empty rather than claiming the entry is marked.

### `private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)`

Marks or unmarks the shown entry, following the state the click left on the star.
A refused write puts the star back where it stood, so it never shows a mark the workspace does not hold.
Marking creates no entry and changes no lexical data.

### `private async void PDisplayLanguageShow(string language)`

The flag comes from `PEnsign`, which every tab holding a display reads too.
The first call may await a fetch, so a later entry may be shown before it arrives.
The language shown now is compared before the image is set.
