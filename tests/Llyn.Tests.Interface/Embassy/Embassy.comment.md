# Embassy

These tests pin the Joplin push through the real embassy entrance, with a fake Joplin standing behind its HTTP client.
They stand apart because the batch, the pairing, the shelf and the manifest show their contract only over HTTP.
Each test starts an engine whose outpost is a `LEmbassyEntrance` over a client with a stub handler.
So no network is touched.
The manifest is the real `joplin.json` under the test workspace.
Tests run in parallel, so every test owns its workspace and its fake.

## `TEmbassyFake`

A message handler that plays the part of Joplin's Data API.
It answers the ping with the banner, hands out a ticket and answers the poll with a configurable status.
It keeps folders, notes and tags in memory, so a test reads back what the push wrote.
A note it has not seen answers not found on update, so the adapter creates it.
Titles in its refusal set answer a server error.
Titles in its silence set raise a connection failure, which the wire reads as a timeout.
It records the id of every update and every delete.
A delete of a known note removes it, which is how a trash shows in the fake.

## `TInterfaceEmbassy`

Covers what one push writes and keeps.
A first push saves the series note under the stems notebook.
No rime-table notebook is made when no rime-table note exists.
The receipt counts every entry note and every reconstruction note, but never the style note.
The manifest holds the id of each note pushed.
A second push with nothing changed keeps every note and updates none.
A series gone from the language read is trashed and leaves the manifest.
The entry's stem chip links to the series note, while a stem with no note stays a plain chip.
A renamed notebook keeps its id, so the next push renames the same top notebook and adds no folder.
A language gone from both the entries and the scan loses its notebooks from the trash guard.
Its series note is then asked for trash, refused by the guard, and stays in Joplin.
The rime-table notebook sits in the language notebook, and the onset and rime notebooks sit in it.
The tone notes sit in the rime-table notebook itself.
The entry's initial, rime and tone chips each link to their category note.
A series note whose save fails lands in the failed list, and its chip stays plain.
A language whose read throws gets no reconstruction notes, while its entry notes still go.
That push trashes nothing, so the manifest keeps the old digests and a later push keeps the old notes.
A stem whose entry is outside the send leaves its chip unlinked.

## `TInterfaceEmbassyFailure`

Covers the push under a failing or foreign Joplin.
Three entry notes in a row that time out refuse the whole push with the outpost refusal.
Two timeouts in a row only list both headwords as failed, and the other note is still saved.
A refused push still saves the manifest, so the notes it did send are remembered.
A manifest of another realm is dropped, so every note is sent again and the realm is rewritten.
A note whose digest matches but whose body in Joplin was edited is sent again.
An accepted pairing answers the token in its hidden form.
A rejected pairing refuses with the warrant refusal.

## Stamp

Commit: `1f3467bbe5c6`
- `TEmbassyFake.cs` `b32e2d6c5b1f25e0`
- `TInterfaceEmbassy.cs` `ee1c3aa47ab7e104`
- `TInterfaceEmbassyFailure.cs` `f22b29a8bd8e00be`
