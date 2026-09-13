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
