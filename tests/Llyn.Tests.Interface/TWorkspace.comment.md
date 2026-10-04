# TWorkspace.cs
Hash: `b63f4db5dd78bb50`

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

Disposing a workspace clears every pooled SQLite connection in the process.
So two workspaces must not be alive in parallel.

## `internal sealed class TWorkspace : IDisposable`

A throwaway workspace folder with its own database, so every test runs against a database nothing else has touched.
Disposing removes the folder.
The connection pool is cleared first.
A pooled connection would otherwise keep the file open and the delete would fail on Windows.

## `private readonly string _tWorkspaceRoot;`

The folder under the temporary path that this workspace owns.

## `private TWorkspace(string root)`

Stores the folder and binds a database to it.
Only the two create forms call it.

## `public LDatabase TWorkspaceDatabase { get; }`

The database bound to this workspace folder.

## `public string TWorkspaceFolder`

The folder itself, for a test that has to reach the file directly.

## `public TClockFake TWorkspaceClock { get; }`

The fake clock every rig of this workspace carries.

## `public TPress TWorkspacePress { get; }`

The fake press every rig of this workspace carries, so a print test reads what reached it.

## `public TPhonographFake TWorkspacePhonograph { get; }`

The fake phonograph every rig of this workspace carries.

## `public void TWorkspaceClockSet(Func<DateTimeOffset> clock)`

Freezes the clock on `clock`, for every engine and clerk built over this workspace.

## `public static TWorkspace TWorkspaceCreate()`

Creates an empty workspace folder whose database has not been initialized yet.

## `public static TWorkspace TWorkspacePrepare()`

Creates a workspace folder whose database is initialized and ready to use.

## `public string TWorkspaceMarkupSave(string text)`

Writes markup text into the workspace folder and returns the path it stands at.
The engine imports from a path, so a test with a document in hand needs a file to hand over.

## `public LEngine TWorkspaceEngineStart()`

Binds an engine to this workspace folder, so a test never constructs one itself.
Its sources fetch through a stub that answers 404 to every address, so no test reaches the network.
A saved entry starts a background frequency fetch, and a real client would hit the language pack's web sources.

## `public LEngine TWorkspaceEngineStart(HttpClient client)`

Binds an engine whose sources fetch through `client`, so a discovery test runs against a stub handler.
The rig is built through the real factory, so the suite starts its engines the way the bootstrap does.
A workspace change builds its rig the same way, and no test ever writes the user's workspace pointer.
The clock is swapped for the workspace's `TClockFake`, so a test can set time through `TWorkspaceClockSet`.
The rig carries the workspace's `TPress` and `TPhonographFake`, so a test reads what reached them.
An engine over fakes instead starts from `TRigFake`.

## `public LEngine TWorkspaceEngineStart(LSourceFactory sources)`

Binds an engine like the client form, over a client that answers 404, with `sources` as its source factory.
A workspace change keeps the same `sources`.
A throwing factory lets a test fail a search before its harvest or lookup starts.

## `private LEngine TWorkspaceEngineStart(HttpClient client, LSourceFactory? sources)`

The one engine build both public forms reach, so the rig shape lives in one place.
Both the first rig and every workspace change's rig fetch through `client` and carry the workspace clock.
When `sources` is given, a local function puts it on each rig as its source factory.
Null keeps the factory the rig built.

## `public LRig TWorkspaceRigCreate()`

A rig over the workspace with a client that answers 404, for a clerk test.

## `public LRig TWorkspaceRigCreate(HttpClient client)`

A rig over the workspace with `client` as its web, the workspace's clock, press and phonograph in place.

## `public SqliteConnection TWorkspaceConnectionRead()`

Opens a raw connection to the workspace database, bypassing the store layer.

## `public long TWorkspaceCountRead(string sql)`

Runs one statement against the workspace database and returns its first value as a number.

## `public IReadOnlyList<long> TWorkspaceColumnRead(string sql)`

Runs one statement and returns its first column for every row.
It is for a test that has to see a column the store layer does not hand out.
An association's position is one such column.

## `public IReadOnlyList<string> TWorkspaceRowRead(string sql)`

Runs one statement and returns every row as one comparable string.
It is for a test that must see whole rows rather than one column or a count.
A null is written as a value of its own, so an empty field never reads as an empty string.
The fields are joined by a separator no stored text carries.

## `public void TWorkspaceScriptRun(string sql)`

Runs statements that set the database up in a shape the store layer would not produce.

## `public void Dispose()`

Clears every pooled connection, then deletes the workspace folder.
A delete that fails on an unreleased file is ignored.

## Inline notes

### end of file

A file the operating system has not released yet is not a test failure.
