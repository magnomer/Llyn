# TOutpostFake.cs
Hash: `cee6a1e7570dc877`

## `public sealed class TOutpostFake : LOutpost`

A Joplin that lives in memory, so a push test needs no running Joplin and no network.
It records every notebook, note, save and trash, so a test reads back what the push did.
It fails only a note save whose title a test lists, so a push fact can watch one note fail.
Its trash keeps Joplin's guard, so a push fact sees which trash the guard refuses.

## `private const int TOutpostFakePort = 41184;`

The port every find answers, so the push never refuses for an absent Joplin.

## `public Dictionary<string, (string TOutpostFakeParent, string TOutpostFakeTitle)> TOutpostFakeFolder { get; } = [];`

Every notebook saved, by id, with its parent id and title.

## `public Dictionary<string, LOutpostNote> TOutpostFakeNote { get; } = [];`

Every note held, by id, as last saved, and without the trashed ones.

## `public List<string> TOutpostFakeSaved { get; } = [];`

The id of each note save in order, so a test tells a kept note from a sent one.

## `public List<string> TOutpostFakeTrash { get; } = [];`

The id of each trash asked for in order, whether the guard let it through or refused it.

## `public HashSet<string> TOutpostFakeRefusal { get; } = new(StringComparer.Ordinal);`

The titles whose note save fails, empty unless a test lists one.

## `public Task<int?> LOutpostFind(int port, CancellationToken cancellation)`

Answers `TOutpostFakePort` whatever port is asked.

## `public Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation)`

Throws, since the push facts never connect.

## `public Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation)`

Throws, since the push facts never connect.

## `public Task LOutpostFolderSave(int port, string token, string id, string parent, string title, CancellationToken cancellation)`

Records the notebook under `id`, replacing an earlier save as Joplin does.

## `public Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)`

Holds the note under its id and records the save.
A title listed in `TOutpostFakeRefusal` throws instead, before anything is held or recorded.
The throw is no timeout and no refusal, so the push counts the note as failed and goes on.

## `public Task<string?> LOutpostNoteRead(int port, string token, string id, string folder, string title, CancellationToken cancellation)`

Answers the held body, or null for a note never saved or already trashed.
So the push keeps a note only while the fake still holds it unchanged.

## `public Task<bool> LOutpostNoteRemove(int port, string token, string id, IReadOnlySet<string> folders, string mark, CancellationToken cancellation)`

Records the trash ask, then applies the rule `LOutpostHttp.LOutpostNoteRemove` applies.
It refuses a note it does not hold and a note outside `folders`.
It also refuses a body that does not start with `mark`.
A refused trash answers false and drops nothing, so a note outside Llyn's notebooks survives.
Otherwise it drops the note and answers true.

## `public Task LOutpostTagSave(int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation)`

Accepts the tags and keeps nothing, since no push fact reads them.

## `public Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation)`

Accepts the parcel and keeps nothing, since no push fact reads parcels.
