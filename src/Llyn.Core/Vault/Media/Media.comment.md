# Media

This folder holds the ports for images, videos, recordings, markup files and exported portraits.
Images and videos are rows in the workspace database, referenced from Meanings, Collocations and Situations.
Recordings, markup and portraits cross the file system or the network.
Their adapters wrap failures so a caller past the adapter names no file or network type.

## `LImageVault`

The persistence port for the Image rows the engine reads and writes.
It lists exactly what the engine asks of image storage, and nothing about how rows are kept.
`LImageArchive` in Infrastructure is its adapter over the workspace database.
Each referrer kind has its own read, attach and detach, named after the Meaning, Collocation or Situation.

## `LImageCreate`

Inserts an image with a fresh opaque id and returns the stored Image with that id filled in.
The new Image is referenced by nothing until it is attached to a referrer.

## `LImageRead`

Reads the Image by id, or null when no such Image exists.

## `LImageMeaningRead`

Reads the Images a Meaning references, in the order that Meaning gives them.
The Collocation and Situation reads do the same for their referrer.

## `LImageUpdate`

Rewrites the location of the Image.
Every referrer sees the change at once, because a reference points at the row and never at its text.

## `LImageMeaningAttach`

References the Image from a Meaning at the index named, renumbering that Meaning's other references around it.
The Collocation and Situation attaches renumber their own referrer's references alike.

## `LImageMeaningDetach`

Drops a Meaning's reference and closes the gap it leaves.
The Image and every other reference stay.
The Collocation and Situation detaches behave alike.

## `LVideoVault`

The persistence port for the Video rows the engine reads and writes.
It mirrors `LImageVault` and keeps nothing about how rows are kept.
`LVideoArchive` in Infrastructure is its adapter over the workspace database.
Each referrer kind has its own read, attach and detach, named after the Meaning, Collocation or Situation.

## `LVideoCreate`

Inserts a video with a fresh opaque id and returns the stored Video with that id filled in.
The new Video is referenced by nothing until it is attached to a referrer.

## `LVideoRead`

Reads the Video by id, or null when no such Video exists.

## `LVideoMeaningRead`

Reads the Videos a Meaning references, in the order that Meaning gives them.
The Collocation and Situation reads do the same for their referrer.

## `LVideoUpdate`

Rewrites the location of the Video.
Every referrer sees the change at once, because a reference points at the row and never at its text.

## `LVideoMeaningAttach`

References the Video from a Meaning at the index named, renumbering that Meaning's other references around it.
The Collocation and Situation attaches renumber their own referrer's references alike.

## `LVideoMeaningDetach`

Drops a Meaning's reference and closes the gap it leaves.
The Video and every other reference stay.
The Collocation and Situation detaches behave alike.

## `LMarkupVault`

The port for markup files, the plain-text form entries travel in.
`LMarkupFile` in Infrastructure is its adapter over the file system and the XML parser.
The engine reads and writes the `LMarkupNode` tree and never sees the text.

## `LMarkupRead`

The element tree of the file at the path.
A file past the markup ceiling, malformed XML or nesting too deep refuses with `LRefusal.LRefusalMarkup`.

## `LMarkupSave`

Writes the tree to the path as markup text, replacing whatever was there.

## `LPortraitVault`

The port for writing a Portrait out of the program as a document.
`LPortraitFile` in Infrastructure is its adapter, which loads the theme the rendered page wears.
The engine composes the page and never learns how a format is rendered or written.

## `LPortraitSave`

Writes a page to the path in the given format.
Markup and PDF are not written here, since the engine formats the one and the press prints the other.
The Livery overload writes an entry's HTML export through the entry sheet writer.
It takes the Livery page beside the portrait page, so the file can show the entry as view mode does.
The lookup words the labels the Livery page carries as keys.

## `LPortraitSheetFormat`

The HTML page wearing the current theme, which the press prints or saves as PDF.
The Livery overload gives the entry sheet, the same page the HTML export writes.

## `LRecordingVault`

The port that brings a chosen recording's bytes into the workspace and answers with a local path.
`LRecordingArchive` in Infrastructure is its adapter over the workspace root and the shared `HttpClient`.
The engine holds the port from the rig, so it names neither the folder nor the client.
A failed fetch or file write raises `LVaultFault`, so a caller past the adapter names no file or network type.

## `LRecordingSave`

Saves the recording under the language's audio folder and answers with the file it landed on.

## `LRecordingPrepare`

Caches the recording for immediate playback and answers with the cached file.

## `LRecordingSweep`

Deletes every file under the audio folder whose full path is not in the kept set.
The engine computes the set from stored and drafted pronunciations and never walks the folder itself.

## Stamp

Commit: `c2f335c331fe`
- `LImageVault.cs` `d78a56fcecd0d006`
- `LMarkupVault.cs` `5fd18fc7c6622ed6`
- `LPortraitVault.cs` `cdf6145db9d1726f`
- `LRecordingVault.cs` `39b6741017af9521`
- `LVideoVault.cs` `4c3a94abc0b9b854`
