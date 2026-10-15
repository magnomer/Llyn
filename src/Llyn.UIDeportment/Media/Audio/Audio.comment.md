# Audio

Audio playback and recording download for the editor and the reading views.
It stands apart from Image and Video because its drivers share one MediaPlayer and one loudness.
Every driver here only paints ready state and hands raw values to gates.

## `QVolumeCatalog`

The one loudness the program plays at, held where anything that plays can reach it.
The slider lives in the pronunciation's tray, and a video card knows nothing of that tray.
So both are wired to this value instead of to each other.
The level is a mirror, not the truth.
The truth is the workspace's audio level in the posture, which `QVolume` sets as it moves.

### `QVolumeCatalogLevel`

The level is taken as given, with no clamp.
Conduct promises a finite level between silence and full, and the slider cannot leave it.
Only `QVolume` sets it.
A set to the level it already holds raises no notice.

## `QClip`

The audio download menu as the editor shows it.
One popup is owned by the editor and retargeted at whichever row's button was pressed.
The search and the menu's state live on the desk's errand, and the menu keeps neither.
The menu repaints the ready clip state each errand notice carries.
So the menu decides nothing about a step, a preview or a download.
The engine already narrowed recordings to the variety of the row that opened the menu.

### `QClipButtonObserve`

The button's open is heard before its start, since placing the popup shuts it and cancels the search.

### `QClipRecordingStart`

A null id is the primary row, which the engine may not have minted yet.
Null is Conduct's word for no row, so no magic id is sent.
The input editor's accent rows call it, so no Conduct roll crosses to the surface.

### `QClipClosedObserve`

A closed menu ends the errand's searches and forgets the marked preview.
Nothing is repainted, since the next opening paints the fresh state.

### `QClipOpenRefine`

The popup is shut first, so a press on another row's button moves it.

### `QClipEnsignRefine`

The state is painted again once the variety flags loaded.
A recording that landed first gains its flag then.

### `QClipRefine`

The running line follows the search alone, since a source that answered does not end it.
The notice shows only while no row stands.

### `QClipReadingApply`

The play button carries the preview state as its cue, playing before fetching before refused.
Both buttons are unsubscribed first, since a recycled container would otherwise hear twice.

### `QClipEndObserve`

The player's end and failure both finish the preview.
The player is shared, so an end with no preview marked changes nothing.

### `QClipPreviewRefine`

No address means the fetch failed or another preview took over, so nothing sounds.

### `QClipCloseRefine`

Taking a recording closes the menu.
A recording the draft had moved away from leaves the menu open.

## `QClipItem`

One source row of the audio menu, painted from the errand's ready clip item.
It raises no change, since the menu builds its rows afresh from each clip state.
The source name is a proper name, so it is shown without a catalog lookup.
The notice is looked up from the errand's key.

## `QClipReading`

One recording a source returned, as a previewable and takeable entry.
It raises no change.
So a preview or a download keeps its look when another search step lands.

### `QClipReadingLabel`

The label is localized when a `Variety.*` key exists and raw otherwise.
In flag mode it is the flag's tooltip.

### `QClipReadingFlag`

Null means the label stands in for the flag.

### `QClipReadingReady`

False while a download runs, and false once one has been saved.

### `QClipReadingFetching`

The play button fills orange, so a slow source is seen to be working.

### `QClipReadingPlaying`

The play button fills blue, and clears when the sound ends or another preview starts.

### `QClipReadingRefused`

The play button fills with the warning colour.
So a host that refused is told apart from one that answered nothing.

## `QPlayback`

The driver for the play button of the entry's own recording and the tray holding the volume.
Which recording plays is the editor's playback fact in `CPlayback`.
This driver only paints it and plays the address the play gate answers.
A headword or language change drops recordings in the engine, and the draft render empties the tray.
The player is the editor's one player, shared with the clip preview and the accent rows.

### `QPlaybackActionRefine`

No address means the file went missing since the button was painted.
The button and the tray are then painted again from a fresh read, which lets the recording go.

### `QPlaybackAudioRefine`

The button's tag is its paint memory, holding the recording the button offers.
A recording other than the painted one stops the player and repaints the button.
An unchanged recording is left alone, so an edit elsewhere never cuts a playing sound short.
The comparison is against what the button painted, never against the draft.

## `QVolume`

The window's one driver of the shared volume, to which every tray attaches.
The level belongs to the workspace rather than to the form, so it outlives the entry being edited.
One owner means one listener per notice and one gate call per moved grip.

### `QVolumeSliderAttach`

The grip is set from the shared volume before listening, as the binding did at load.
It listens only for the gestures that end a change.
Those are a finished drag, a track click and a released key.
A drag that wrote on every step would put a file write behind every pixel.

### `QVolumePlayerAttach`

A reading view holds no player, since it hands the slider's level to each playback it starts.

### `QVolumeObserve`

A moved grip becomes the shared volume and goes to the volume gate unsettled.
So the engine plays at it without a write.
Only a grip moved by hand gets here, since painting goes unheard.

### `QVolumeLevelRefine`

Every grip and player follows the shared volume.
It makes no request, since a moved grip made its own and an opened workspace needs none.

### `QVolumeSaveObserve`

The level goes to the volume gate settled once the hand comes off the grip.

### `QVolumeRefine`

An opened workspace's volume is a ready answer, so it is painted and makes no request.
It first becomes the shared volume, so a tray attached later starts at it.
Every grip and player is then painted directly.
That paint still lands when the shared volume already held the level and raised no notice.

### `QVolumeShow`

Each grip is unheard while painted, so a painted grip makes no second request.

## Stamp

Commit: `2f9a06b8eb95`
- `QClip.cs` `73c0340db4b50623`
- `QClipItem.cs` `fcd2539c79608e01`
- `QClipReading.cs` `3a7de531b7f11376`
- `QPlayback.cs` `490b1f668fbb68ec`
- `QVolume.cs` `e6cdd3ecad5136ff`
- `QVolumeCatalog.cs` `34c57b0f4686524a`
