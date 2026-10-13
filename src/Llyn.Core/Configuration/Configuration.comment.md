# Configuration

Configuration holds the plain records and the port that carry settings, posture and status between runs.
None of it is lexical data, so none of it lives in the workspace database.
Each record is plain data that knows nothing of files, SQLite or the window that shows it.

## `LDoctorRescue`

What the workspace doctor had to do before the program could open its database.
A launch either found the database usable or found it unusable and started over.
The record carries that answer out of the logic layer so the shell can tell the user.
`LDoctorRescueHealthy` is the answer for a database that opened on its own, with no backup and no reason.

## `LEstablishment`

What the status bar prints about the open workspace, read from the engine in one call.
It is shown as it came.
The bar never counts drafts or rows itself, because the engine alone holds both.
`LEstablishmentSingle` picks the singular wording, which only a count of one takes.
`LEstablishmentLarge` counts the file in megabytes once it reaches one, and in kilobytes below that.
`LEstablishmentAmount` rounds kilobytes up, so a workspace of a few words never counts as zero.

## `LKeep`

A named text document kept under the workspace root.
It is the port a ring writes its own state through without knowing the disk.
The infrastructure gives each name a file, and a test gives it a dictionary.
The engine hands the port out, so an outer ring persists beside the engine rather than through it.
`LKeepRead` answers nothing while none was ever written.
A document that exists but cannot be read raises the adapter's exception rather than reading as nothing.
`LKeepSave` replaces the text whole.

## `LLayout`

The ordering one tab lists by and the languages it hides, carried between two runs.
Both are view state of the same tab, and a console lists by them as the GUI does.
It is part of the posture the shell keeps, so it belongs to the workspace.
An ordering or filter left empty means the tab opens as it was designed to.
It lives in the posture file rather than the workspace database.
Panel widths are GUI-only, so they live in the GUI driver's Capsule, not here.

## `LPostureState`

How the session stands in every medium: each tab's listing, the open tab, the split and the volume.
Window geometry and panel widths are GUI-only, so the GUI driver keeps them in its own Capsule.
It lives beside the settings file as `posture.json` rather than inside it, since none of it is an engine fact.
The posture holds one of these and swaps it whole on every change.
So a read never sees half a change.

## `LSettings`

The user's persisted engine settings, kept as `settings.json` inside the workspace folder and nowhere else.
So a workspace carries its own preferences.
Every field is a preference, except `LSettingsWarrant`, the protected Joplin credential.
Only the platform twin of the warrant port can turn it back into the token.
Window posture is not part of this record.
The workspace folder path is the bootstrap locator, so it is kept in a small fixed pointer outside the workspace.
The epithet is the reading a pack's reflex rule names, such as the Korean 훈 and 음.
The pack list holds no such default, so the user's own language is kept here.
The analysis is stored on every write either way, so a flip needs no new analysis.
The notebook keeps one fixed id, so a new title renames the same notebook at the next sync.
Every workspace shares that id, so two workspaces with different titles rename it in turn.

## `LSettingsNotebook`

The stored title is always already resolved, with `LSettingsNotebookFallback` as the title a workspace starts with.
The property redeclares the positional title so the constructor and a `with` copy both pass through its rule.
So a padded or blank title never reaches a sync, whichever layer built the record.
`LSettingsNotebookResolve` trims the title, and a blank or missing one becomes the fallback.

## `LSettingsOnline`

How many of the web lookups are on, counting frequency and morphology.
The settings Web page shows it against the number of lookups there are.

## `LWorkspaceState`

The shell's own view state for one workspace.
It holds which Entry each duplex side shows, the session revision and the identity floor.
The floor is the lowest temporary id this workspace has ever issued.
This is session data, not lexical data, so clearing it loses no dictionary content.
Deleting an Entry simply empties whichever duplex side was showing it.
State that names a row of this workspace's database is stored here, beside that database.
State that names only panels is stored in `LPostureState` instead.
That is the open tab, its split, each ordering and each hidden language.

## Stamp

Commit: `c2f335c331fe`
- `LDoctorRescue.cs` `e975d172db3ce09b`
- `LEstablishment.cs` `9bf61dd8ca2a9225`
- `LKeep.cs` `a5eff2fe2750869f`
- `LLayout.cs` `730225fc03370b2d`
- `LPostureState.cs` `ea33d7f7b2e30456`
- `LSettings.cs` `4179d400e6422dfb`
- `LWorkspaceState.cs` `f1376a62e761ae0b`
