# Recording

This folder holds the tests of audio recording discovery and storage.
They cover harvest and its steps, the recording clerk, finding, saving and the throttling handler.

## `THarvest`

Covers the harvest fanning one source's readings out into recordings and narrowing them to a variety.
An untagged reading in a language with varieties becomes one recording per declared variety, all sharing the address.
Without varieties the untagged reading stays untagged, so a pack like Spanish is unchanged.
A variety the answer already tags is not fanned out again.
The returned set always holds every variety, so a later menu can be narrowed from it without a fetch.
Asking for one variety streams the tagged recording of that variety alone.
An untagged recording streams as one row carrying that variety.
A variety the source did not serve still streams one recording without an address.
Narrowing a kept set keeps one row per source, an addressless one where a source served nothing fitting.
An empty variety returns the kept set itself.
A lost source yields one recording that was never reached, and the listener hears the finish once.
Sources keep their declared order in the returned set.
One source keeps its readings in the order it gave them.

## `THarvestStep`

Covers the two step relays that turn a streamed contract into records handed to a delegate.
A listener relay receives one source step, one recording per reading, and one end, in that order.
The source step names the source and its position, the end names nothing, and only a recording step carries one.
A receiver relay receives the same shape with candidates.
The respelling wrapper over a relay still respells, so the candidate step carries the recast text.

## `TRecordingClerk`

Covers the recording clerk over a real rig with a canned source client.
A harvest over the pack's one source reaches the listener step by step and answers the same recordings.
The sweep deletes a file no pronunciation row names and keeps a file a row names.
A file that is not on disk, or no file at all, never reaches the phonograph.
A workspace-relative file reaches the phonograph as its full path.
A volume above one plays at full level, and a volume below zero sets silence.
Two plays on one clerk share one phonograph.
A play of two files starts the first, and the phonograph's finish starts the second.
  A stop with the run's ticket plays no more files after the finish.
The ticket of an earlier play stops nothing, and the latest play's ticket stops it.

## `TRecordingFinding`

Covers the engine reading the opening row's variety from the draft before it searches.
A primary row tagged American returns American recordings only, an untagged source collapsing to one American row.
A primary row without a variety returns every variety, an untagged source fanned out once per declared variety.
A row that gains a variety after a search is searched again rather than replayed from the trove.

## `TRecordingSaving`

Covers the workspace naming a downloaded recording.
A tagged recording is saved under the headword, its variety and a digest of its address.
So two varieties never overwrite each other, and an untagged recording leaves the variety out.
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

## `TRefusalHandler`

A message handler that answers the first few requests with 429 and every later one with audio bytes.
Each refusal names a one-second Retry-After, the value Wikimedia's audio host sends.
It counts the requests it saw, so a test can read how many tries a fetch took.
It stands in for a host throttling bursts, so the workspace's retry runs offline.

## Stamp

Commit: `bd299c638b2c`
- `THarvest.cs` `6195e46689619707`
- `THarvestStep.cs` `47edebe26fa56bfd`
- `TRecordingClerk.cs` `373ca9025d21c62a`
- `TRecordingFinding.cs` `365b70e5aa73e843`
- `TRecordingSaving.cs` `72b0224e0435c9d9`
- `TRefusalHandler.cs` `650e47d07ed9c632`
