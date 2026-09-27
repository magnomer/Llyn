# QWorkspace.cs

## `public sealed class QWorkspace`

The deportment of the open workspace, reached through the window deportment.
The workspace owns the settings and the database, so every read and save of them runs here.
Those are the settings, the folder path, the workspace state, the interface language and the switches.
It also answers the status bar, the notice wording, the flags and the recordings kept in the folder.
The stored entries a dialog or a menu asks for by id or by headword are read here too.
It is sealed, so its public members name only .NET types and Conduct shapes.
It holds no port of its own and reaches the engine through the window deportment's ports.

## `internal QWorkspace(LWindow window)`

Only the window deportment builds one, over itself.

## `public string QWorkspacePathRead()`

The workspace folder as the settings panel shows it in its path field.

## `public CWorkspaceState QWorkspaceStateRead()`

The entries the duplex wings last stood on.

## `public string? QWorkspaceNoticeRead(Exception exception)`

The words a refusal shows, read by the engine since the refusal type is its own.

## `public Task<string> QWorkspaceRecordingPrepare(CRecording recording, CancellationToken cancellation)`

Downloads a remote recording into the workspace and answers the local path it was saved to.

## `public Task QWorkspaceEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Loads the cached flags, handing the store each row as the Conduct shape.
The engine's rows are copied in a local function, so no engine answer sits in a local.

## `public IReadOnlyList<long> QWorkspaceMarkupFind(string headword, string language)`

The stored entries a markup entry could stand for, by id.

## `public IReadOnlyList<CMeaning> QWorkspaceMeaningRead(long entryId, string unknown)`

The Meanings of one Entry as sense-menu rows in reading order.

## `internal static IReadOnlyList<CMeaning> QWorkspaceMeaningSort(IReadOnlyList<LMeaning> meanings, string unknown)`

The Meanings of one Entry as sense-menu rows in reading order, each with its depth.
A Meaning is named by its title, or by its definition when it has no title.
The unknown label names it when it has neither.

## `private static void QWorkspaceMeaningAppend(...)`

Walks one level of the tree and recurses under each Meaning it lists.
The engine returns the Meanings grouped by parent, and the menu wants them in reading order.
So the children of each parent are picked out and sorted by position before their own children follow.
A row that names itself as its parent is skipped rather than followed.
So a bad row cannot loop the walk.

## `internal static CSettings QWorkspaceSettingsRead(LSettings settings)`

Copies the engine's settings, with its count of online lookups, into the Conduct shape.
