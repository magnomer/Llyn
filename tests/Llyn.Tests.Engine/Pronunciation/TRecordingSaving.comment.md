# TRecordingSaving.cs
Hash: `72b0224e0435c9d9`

## `public sealed class TRecordingSaving`

Covers the workspace naming a downloaded recording.
A tagged recording is saved under the headword, its variety and a digest of its address.
So two varieties never overwrite each other.
An untagged recording leaves the variety out.
Two headwords differing only by case land in two files, since Windows would fold them onto one.
A headword Windows reserves as a device name is prefixed, so the file can be created.
An address naming no audio extension is saved as `.mp3` rather than under whatever the source said.
No pending file is left once a save lands.
A preview asked twice for one address returns the same cache file, even while a reader holds it open.
Two addresses sharing a file name land in two cache files.
A host answering 429 once is asked again and the second answer is saved.
A host that never stops refusing raises `LVaultFault` after the third try, with the refusal inside.
It leaves no cache folder behind.
An audio folder blocked by a file raises `LVaultFault` with the file failure inside.
A cancellation the caller asked for is raised as a cancellation, not wrapped.
The bytes come from a stub handler, so the test runs offline.
