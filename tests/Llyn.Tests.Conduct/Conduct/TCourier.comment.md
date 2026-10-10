# TCourier.cs
Hash: `3e5eb47d1fab6129`

## `public sealed class TCourier`

Covers whether the courier shows Llyn as connected to Joplin, over a real workspace engine.
The token is written into the workspace's settings file before the engine starts, as a past connect left it.
A stored token reads as connected once the workspace opens, and not before.
A workspace whose settings hold no token reads as unconnected after a switch, since each workspace keeps its own.
A workspace with no settings file at all would inherit the token, so the second one gets an empty one.
A push whose token cannot be restored shows `Courier.SendFailed` and drops the token.
The courier then reads unconnected while the push still runs, and ends idle with no line.
The push facts run the clerk against `TOutpostFake`, so every saved notebook and note can be read back.
The receipt counts every entry note and every reconstruction note, but never the style note.
A first push saves the series notebook and the series note, but no empty rime-table notebook.
A second push keeps the series note, and a series gone from the language read is trashed.
The entry's stem chip links to the series note, while a stem with no note stays a plain chip.
A language gone from both the entries and the scan loses its notebooks from the trash guard.
Its series note is then asked for trash, refused by the guard, and stays in Joplin.
The category fact checks where the initial, rime and tone notes land.
Rime table sits in the language notebook, and Onset and Rime sit in Rime table.
Tone notes sit in Rime table itself.
The entry's initial, rime and tone chips each link to their category note.
A series note whose save fails lands in the failed list, and its chip stays plain.
A language whose read throws gets no reconstruction notes, while its entry notes still go.
Its entry note is sent again, since its chips now stay plain.
That push trashes nothing, so the manifest keeps the old digests and a later push keeps the old notes.

## `private const string TCourierLanguage = "Classical Chinese";`

The language of the one entry each push fact stores, so the entry and its series share one language's notebooks.

## `private const string TCourierStray = "Stray";`

A second language that no language pack names, so it stands in the scan only while an entry uses it.

## `private static LEngine TCourierEngineStart(TWorkspace workspace, TOutpostFake outpost)`

Starts an engine whose rig talks to `outpost`, with a token the fake warrant restores.
It stores one entry, `瀧`, so every push has an entry note to write.

## `private static LEntry TCourierEntrySave(LEngine engine, string headword, string language)`

Stores an entry with one meaning card, so a push fact can add an entry in a second language.

## `private static Task<LReceipt> TCourierSeriesSend(LEngine engine, bool series, string unread = "")`

Pushes with each entry page carrying one rime group whose stems are `龍` and `瀧`.
The language read holds the series `龍` only when `series` is true.
So `瀧` never has a series note, and its chip must stay plain.
The read of the language named `unread` throws, as a broken language pack would.

## `private static string TCourierFolderFind(TOutpostFake outpost, string title, string? parent = null)`

Answers the id of the one notebook with `title`, under `parent` when one is given.
Two languages each hold notebooks of the same title, so the parent tells them apart.

## `private static LOutpostNote TCourierNoteFind(TOutpostFake outpost, string title, string? folder = null)`

Answers the one held note with `title`, in `folder` when one is given.

## `private static void TCourierWarrantSave(TWorkspace workspace, string warrant)`

Writes a settings file holding only the language and `warrant`.
The fake warrant restores only a value marked `fake:`, so any other value reads as refused.
