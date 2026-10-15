# Shipment

These tests pin the deck export through its file format, manifest store, latch and clerk.

## `TDepotFolder`

These tests pin the phone file format the folder adapter writes.
Each save test writes a sheet into a fresh workspace folder and reads the text back.
The expected text is literal, so a quoting or order change shows at once.
The backup test plants a file under `Backups/` and proves the digest read never reaches it.

## `TDeckManifestFile`

This test pins the manifest file store over a real keep file.
A save then a read gives back every deck's digest, media names and time written.
The media names read back sorted, since the store writes them in ordinal order.

## `TDepotLatch`

These tests pin the export latch each workspace holds.
Two engines over two workspaces hold two latches, so a run in one leaves the other idle.
A started latch refuses a second start until it finishes.

## `TDepotClerk`

These tests pin the deck export over a fake depot and a fake manifest.
A deck file the manifest never wrote is refused and left as it was.
Two sources with one content name are copied once.
Only a name the manifest owns is deleted, so a phone's own file stays.
An http picture is written as its address and copies nothing.
A started latch refuses a run, and another workspace's latch blocks nothing.
A blank phone folder refuses with the depot refusal and leaves the latch free.
An orphaned cue never becomes a row.

## Stamp

Commit: `2f9a06b8eb95`
- `TDeckManifestFile.cs` `fe2b883882b6283f`
- `TDepotClerk.cs` `2abdb0327195a6b9`
- `TDepotFolder.cs` `84a907edc3a0bdba`
- `TDepotLatch.cs` `3aad50a929a35ba7`
