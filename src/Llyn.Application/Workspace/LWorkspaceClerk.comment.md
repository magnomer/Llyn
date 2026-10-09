# LWorkspaceClerk.cs
Hash: `c7be97a92bcdd963`

## `public sealed class LWorkspaceClerk`

The workspace-level ports of one rig behind one clerk.
Settings, audit, posture, workspace state and size, localization, the clock and the post-migration refill live here.
The open sequence is static, so the engine can test a new rig before it swaps any field.

## `public LWorkspaceClerk(LRig rig, LLanguageCache languages, LReflexClerk reflexes, LParadigmClerk paradigms)`

Reads the ports out of `rig` and keeps the clerks the refill writes through.

## `public static LDoctorRescue LWorkspaceRescueCreate(LRig rig)`

The database made or repaired on `rig`, reported as a rescue.

## `public static LSettings LWorkspaceSettingsRead(LRig rig, LSettings fallback, out bool settled)`

The stored settings of `rig`, or `fallback` when none are stored yet.
`settled` says which, so the caller can write the fallback down.

## `public LSettings LWorkspaceSettingsRead()`

The stored settings of this workspace.

## `public void LWorkspaceSettingsSave(LSettings settings)`

Writes the settings.
A vault fault is thrown on to the caller unrecorded.
The caller restores what it held and lets the gate show the failure.
The gate's failure policy records the fault, so it is recorded once.

## `public void LWorkspaceFallbackSave(LSettings settings)`

Writes the fallback settings a newly opened workspace had none of.
A vault fault is recorded in the audit and swallowed, so the open goes on.
The engine keeps the fallback in memory, so the session goes on with it.

## `public bool LWorkspaceClerkMigrated`

Whether the vault migrated its schema when it opened.

## `public string? LWorkspaceAuditRecord(Exception exception)`

Records `exception` in the audit and answers the file it went to.

## `public static string? LWorkspaceNoticeRead(Exception exception)`

The refusal reason inside `exception`, walking the inner exceptions, or null.

## `public static string? LWorkspaceChosenRead(string chosen, string folder)`

The folder a user chose, trimmed, when it names a workspace other than `folder`.
A blank choice or the folder already in use answers null, since moving there changes nothing.
The move's check and the move itself both read it, so the rule has one owner.

## `public static bool LWorkspaceIllegibleCheck(Exception exception)`

Whether `exception` is a refusal over an unreadable value, so the shell can offer to sweep the draft.
Only the exception itself is read, since a wrapped refusal is not the commit's own answer.

## `public static bool LWorkspaceRefusedCheck(Exception exception)`

Whether `exception` is a refused edit rather than a lost draft, which the shell skips and goes on.
A refusal saying the draft is gone is left out, since that is a lost hold and halts the tenure.

## `public static bool LWorkspaceStaleCheck(Exception exception)`

Whether `exception` is the refusal of a stale draft, and nothing else.
A recording search, a reading search or a byline find over such a draft answers nothing.
A stale draft is no failure, so the shell shows no notice for it.

## `public IReadOnlyDictionary<string, string> LWorkspaceLocalizationLoad(string language)`

The localization table of `language` through the localization port.

## `public DateTimeOffset LWorkspaceClockRead()`

The moment now, through the clock port.

## `public LWorkspaceState LWorkspaceStateRead()`

The workspace state row.

## `public void LWorkspaceStateSave(LWorkspaceState state)`

Writes the workspace state row.

## `public long LWorkspaceSizeRead()`

The bytes of the main database file, for the status bar.

## `public bool LWorkspacePostureRead(string name, out LPostureState? state)`

Reads the posture stored under `name`.
Answers false on a vault fault, after recording it, so the caller keeps what it has.
No user action triggers a read, so a failure is recorded and shows no notice.

## `public bool LWorkspacePostureSave(string name, LPostureState state, out Exception? fault)`

Writes the posture under `name` and answers whether the write succeeded.
A vault fault is recorded in the audit and handed back as `fault`, never thrown.
`fault` is a plain `Exception`, so the shell above names no Core type.
The posture's notice must not record it again.
A layout save runs during drags and bulletins, where a throw would break the view.
So the posture decides how to tell the user, and the clerk only reports.

## `public void LWorkspaceClerkUpdate()`

The refill a migration needs.
Every entry gets its respellings, anatomies, epithet and paradigm recomputed in one session.
One entry that fails is recorded and skipped, so one bad row never blocks the workspace.

## `private void LRespellingUpdate(LEntry entry)`

Fills the respelling of every pronunciation that has a reading but no respelling.
Fills a missing reflex respelling the same way, and recomputes the anatomy of every reflex with text.
The reflex list is written back only when something changed, so a settled entry costs no write.
