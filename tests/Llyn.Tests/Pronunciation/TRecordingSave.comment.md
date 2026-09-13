# TRecordingSave.cs

## `public sealed class TRecordingSave`

Covers the workspace naming a downloaded recording.
A tagged recording is saved under the headword and its variety, so two varieties never overwrite each other.
An untagged recording keeps the bare headword name.
A preview asked twice for one address returns the same cache file, even while a reader holds it open.
Two addresses sharing a file name land in two cache files.
The bytes come from a stub handler, so the test runs offline.
