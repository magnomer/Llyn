# LWorkspaceClerk.cs

## `public sealed class LWorkspaceClerk`

The workspace-level ports of one rig behind one clerk.
Settings, audit, posture, workspace state, localization, the clock and the post-migration refill live here.
The open sequence is static, so the engine can test a new rig before it swaps any field.

## `public LWorkspaceClerk(LRig rig, LLanguageCache languages, LReflexClerk reflexes, LParadigmClerk paradigms)`

Reads the ports out of `rig` and keeps the clerks the refill writes through.

## `public static LDoctorRescue LWorkspaceRescueCreate(LRig rig)`

The database made or repaired on `rig`, reported as a rescue.

## `public static LRealm LWorkspaceRealmRead(LRig rig)`

The realm of `rig`.

## `public static LSettings LWorkspaceSettingsRead(LRig rig, LSettings fallback, out bool settled)`

The stored settings of `rig`, or `fallback` when none are stored yet.
`settled` says which, so the caller can write the fallback down.

## `public LSettings LWorkspaceSettingsRead()`

The stored settings of this workspace.

## `public void LWorkspaceSettingsSave(LSettings settings)`

Writes the settings.
A vault fault is recorded in the audit and swallowed, since settings are never worth a crash.

## `public bool LWorkspaceClerkMigrated`

Whether the vault migrated its schema when it opened.

## `public string? LWorkspaceAuditRecord(Exception exception)`

Records `exception` in the audit and answers the file it went to.

## `public static string? LWorkspaceNoticeRead(Exception exception)`

The refusal reason inside `exception`, walking the inner exceptions, or null.

## `public static bool LWorkspaceIllegibleCheck(Exception exception)`

Whether `exception` is a refusal over an unreadable value, so the shell can offer to sweep the draft.
Only the exception itself is read, since a wrapped refusal is not the commit's own answer.

## `public static bool LWorkspaceRefusedCheck(Exception exception)`

Whether `exception` is a refused edit rather than a lost draft, which the shell skips and goes on.
A refusal saying the draft is gone is left out, since that is a lost hold and halts the tenure.

## `public IReadOnlyDictionary<string, string> LWorkspaceLocalizationLoad(string language)`

The localization table of `language` through the localization port.

## `public DateTimeOffset LWorkspaceClockRead()`

The moment now, through the clock port.

## `public LWorkspaceState LWorkspaceStateRead()`

The workspace state row.

## `public void LWorkspaceStateSave(LWorkspaceState state)`

Writes the workspace state row.

## `public bool LWorkspacePostureRead(string name, out LPostureState? state)`

Reads the posture stored under `name`.
Answers false on a vault fault, after recording it, so the caller keeps what it has.

## `public void LWorkspacePostureSave(string name, LPostureState state)`

Writes the posture under `name`, recording and swallowing a vault fault.

## `public void LWorkspaceClerkUpdate()`

The refill a migration needs.
Every entry gets its respellings, anatomies, epithet and paradigm recomputed in one session.
One entry that fails is recorded and skipped, so one bad row never blocks the workspace.

## `private void LRespellingUpdate(LEntry entry)`

Fills the respelling of every pronunciation that has a reading but no respelling.
Fills the respelling and anatomy of every reflex the same way and writes the list back only when something changed.
