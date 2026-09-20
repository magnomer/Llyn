# TWorkspace.cs

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

Disposing a workspace clears every pooled SQLite connection in the process.
So two workspaces must not be alive in parallel.

## `internal sealed class TWorkspace : IDisposable`

A throwaway workspace folder with its own `llyn.db`, so every test runs against a database nothing else has touched.
Disposing removes the folder.
The connection pool is cleared first.
A pooled connection would otherwise keep the file open and the delete would fail on Windows.

## `public LDatabase TWorkspaceDatabase { get; }`

The database bound to this workspace folder.

## `public string TWorkspaceFolder => _tWorkspaceRoot;`

The folder itself, for a test that has to reach the file directly.

## `public TClockFake TWorkspaceClock { get; }`

The frozen clock every rig of this workspace carries.

## `public TPress TWorkspacePress { get; }`

The fake press every rig of this workspace carries, holding the last sheet and ticket it received.

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
Only the clock is swapped for the workspace's `TClockFake`, so a test can freeze time through `TWorkspaceClockSet`.
The rig carries the workspace's `TPress`, so a print test reads what reached it from the workspace.
An engine over fakes instead starts from `TRigFake`.

## `public LRig TWorkspaceRigCreate()`

A rig over the workspace with a client that answers nothing, for a clerk test.

## `public LRig TWorkspaceRigCreate(HttpClient client)`

A rig over the workspace with `client` as its web, the workspace's clock and press in place.

## `public SqliteConnection TWorkspaceConnectionRead()`

Opens a raw connection to the workspace database, bypassing the store layer.

## `public long TWorkspaceCountRead(string sql)`

Runs one statement against the workspace database and returns its first column.

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

## Inline notes

### end of file

A file the operating system has not released yet is not a test failure.
