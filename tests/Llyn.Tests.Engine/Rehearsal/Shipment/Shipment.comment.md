# Shipment

These tests pin the deck export through the locker entrance, its manifest, the latch and the clerk.

## `TDepotFolder`

These tests pin the phone file format the locker entrance writes.
Each test exports one shipment into a fresh workspace and reads the file text back.
The expected text is literal, so a quoting or order change shows at once.
A cell with a break is quoted with inner quotes doubled.
The Basic style writes a bar for a break.
Columns given out of order are written in the phone's order.
The file starts with `T`, so no byte order mark leads it.
Two pictures with the same bytes are copied once and share one name.
A missing picture is named in the receipt's failures and leaves its cell empty.
An http picture is written as its address.

## `TDeckManifestFile`

These tests pin the digest check and the media ownership of the manifest the entrance keeps.
A deck file the manifest never wrote is refused with the phone refusal and left as it was.
A file Llyn wrote itself is written again.
A media file the deck dropped is deleted only when the manifest lists it.
So a file the phone placed there stays.

## `TDepotLatch`

These tests pin the export latch each workspace holds.
Two engines over two workspaces hold two latches, so a run in one leaves the other idle.
A started latch refuses a second start until it finishes.

## `TDepotClerk`

These tests pin what the clerk hands a fake depot.
A local picture ships as its full path, and an http picture ships as its address.
A location the trail cannot resolve ships empty beside its typed text.
A local picture ships its typed text, and an http picture ships none.
A started latch refuses a run, and another workspace's latch blocks nothing.
A blank phone folder refuses with the depot refusal and leaves the latch free.
An orphaned cue never becomes a row.

## Stamp

Commit: `1f3467bbe5c6`
- `TDeckManifestFile.cs` `d399953c3da7c7ec`
- `TDepotClerk.cs` `70a8494187fa4bcc`
- `TDepotFolder.cs` `f2ae7c3006a4a00b`
- `TDepotLatch.cs` `3aad50a929a35ba7`
