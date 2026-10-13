# Workspace

The clerks for what belongs to one workspace as a whole: its paths and locations, and its own ports.
They stand apart from the entry clerks because they serve no single entry.

## `LTrailClerk`

It resolves paths and locations against one workspace root.
It holds the trail, the usher and the root of one rig, so the engine names no path port itself.
A location is a web address or a file path, absolute or relative to the workspace.
The same resolution serves recordings, images, videos and the markup import.

### `LWorkspaceFormat`

The workspace is shown by its folder name, or by the full root when the name is empty.

### `LTrailRootCheck`

It tells a flag code that is a path from a remote code.

### `LTrailPathResolve`

It answers null when the trail refuses the path.

### `LTrailClerkResolve`

A web address stays a web address.
A path that starts with a separator is refused, since it names no drive and no workspace.
A drive-qualified path is taken as a file and any other scheme is refused.
The rest is resolved as a workspace-relative file.

### `LTrailFileResolve`

A path the trail cannot place, or one the Uri parser refuses, answers null.

### `LTrailClerkRead`

It answers null when the location is a file that does not exist.

### `LTrailScreenRead`

It pairs the resolved address with the hosted film id Core's `LVideoFilmRead` names in it.
It answers null where `LTrailClerkRead` does.
A local file names no hosted film, so its film id is null without asking.

### `LRecordingResolve`

It answers the text as given when it is not a file.

## `LWorkspaceClerk`

It puts the workspace-level ports of one rig behind one clerk.
Settings, audit, posture, workspace state and size, localization, the clock and the post-migration refill live here.
The open sequence is static, so the engine can test a new rig before it swaps any field.
The constructor keeps the clerks the refill writes through.

### `LWorkspaceSettingsRead`

The static form answers the fallback when no settings are stored yet.
Its `settled` flag says which, so the caller can write the fallback down.

### `LWorkspaceSettingsSave`

A vault fault is thrown on to the caller unrecorded.
The caller restores what it held and lets the gate show the failure.
The gate's failure policy records the fault, so it is recorded once.

### `LWorkspaceFallbackSave`

A vault fault is recorded in the audit and swallowed, so the open goes on.
The engine keeps the fallback in memory, so the session goes on with it.

### `LWorkspaceNoticeRead`

It walks the inner exceptions for the refusal reason, or answers null.

### `LWorkspaceChosenRead`

It answers the chosen folder, trimmed, when it names a workspace other than the current folder.
A blank choice or the folder already in use answers null, since moving there changes nothing.
The move's check and the move itself both read it, so the rule has one owner.

### `LWorkspaceIllegibleCheck`

It tells whether the exception is a refusal over an unreadable value.
So the shell can offer to sweep the draft.
Only the exception itself is read, since a wrapped refusal is not the commit's own answer.

### `LWorkspaceRefusedCheck`

It tells whether the exception is a refused edit rather than a lost draft.
The shell skips it and goes on.
A refusal saying the draft is gone is left out, since that is a lost hold and halts the tenure.

### `LWorkspaceStaleCheck`

It tells whether the exception is the refusal of a stale draft, and nothing else.
A recording search, a reading search or a byline find over such a draft answers nothing.
A stale draft is no failure, so the shell shows no notice for it.

### `LWorkspacePostureRead`

It answers false on a vault fault, after recording it, so the caller keeps what it has.
No user action triggers a read, so a failure is recorded and shows no notice.

### `LWorkspacePostureSave`

A vault fault is recorded in the audit and handed back as `fault`, never thrown.
The fault is a plain `Exception`, so the shell above names no Core type.
The posture's notice must not record it again.
A layout save runs during drags and bulletins, where a throw would break the view.
So the posture decides how to tell the user, and the clerk only reports.

### `LWorkspaceClerkUpdate`

This is the refill a migration needs.
Every entry gets its respellings, anatomies, epithet and paradigm recomputed in one session.
One entry that fails is recorded and skipped, so one bad row never blocks the workspace.

### `LRespellingUpdate`

It fills the respelling of every pronunciation that has a reading but no respelling.
It fills a missing reflex respelling the same way, and recomputes the anatomy of every reflex with text.
The reflex list is written back only when something changed, so a settled entry costs no write.

## Stamp

Commit: `c2f335c331fe`
- `LTrailClerk.cs` `2097b4a2eb283d08`
- `LWorkspaceClerk.cs` `c7be97a92bcdd963`
