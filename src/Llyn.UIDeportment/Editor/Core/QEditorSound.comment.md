# QEditorSound.cs
Hash: `06dec3147a4ae555`

## `internal sealed class QEditorSound`

The editor's sound half: the pronunciation row, its contour, and the drivers of the sound panels.
The panels live in `QCadence`, the lookup menu in `QNotation`, the transcription rows in `QTranscription`.
The glyph row lives in `QGlyph`, the further pronunciation rows in `QAccent` and the reflex rows in `QReflex`.
The reflex anchor menu lives in `QAnchor` and the audio menu in `QClip`.
The play button lives in `QPlayback`.
The volume slider is painted by the window's one `QVolume`, which every editor shares.
It keeps the one recording player, which the playback, the volume, the clip preview and the accents share.

## `internal QEditorSound(FrameworkElement surface)`

Builds the sound drivers over the editor scope in the order the editor once built them.
The contour redraws as the pronunciation is typed, beside the gate that hears the same text.
The measuring twin follows the field's text and hint, where a style binding and trigger stood.

## `internal void QEditorSoundIntroduce(CEditor editor, QVolume volume, CLedger ledger, CEnvoy envoy)`

Hands each sound driver only the editor facets it uses.
The anchor menu gets its anchor over the editor's kindred facet.
The transcription facet is read once, so the glyph and transcription rows share one.
The volume slider and the player are attached to the window's one volume owner.
The ledger and the envoy go on to the sound panels, which show a picture's decode failure through them.
The ledger and the envoy are also kept for `QEditorFailureObserve`.
The player's failure is subscribed here once, since the clip, the accents and the playback share it.
The clip's own failure handler still runs beside it and only resets a marked preview.

## `private void QEditorFailureObserve(object? sender, ExceptionEventArgs e)`

Shows the player's failure through `CLedger.CLedgerFailureShow` under `Sound.PlayFailed`.
The player is this driver's own medium, and only it raises this event.
Another driver would play the address with its own player and meet its own failure.
So no Conduct gate carries it, as with a picture the script decoder refuses.

## `private CLedger _qEditorSoundLedger`

The window's ledger, which shows the player's failure.

## `private CEnvoy _qEditorSoundEnvoy`

The window's envoy, which the failure is shown through.

## `internal void QEditorPlayerRefine()`

Releases the editor's recording player, the Veneer half of a close.

## `internal void QEditorReadingRefine()`

The reading line follows the headword the field holds.

## `internal void QEditorPronunciationRefine(CEntryDraft _)`

Writes the primary reading as the editor reads it, respelled when the pack respells.
The draft is ignored, since only the editor knows the respelling.

## `internal void QEditorTimbreRefine(CEntryDraft _)`

The writes that follow the language's sound facts: the brackets, the tone contour and the silent switch.
The contour is read again here, since a new language may change it over the same text.

## `private void QEditorContourRefine()`

Reads the ready contour of the field's text.
The typed text runs ahead of the draft, so the read takes the field's text.

## `private void QEditorContourRefine(IReadOnlyList<CContour> syllables)`

Copies the ready syllables into the contour box's own items through `QContourInk.QContourInkBuild`.
Each item carries the theme brush of each level, resolved from the box.
It hands the box Conduct's scale too, so the box names no Conduct type.
