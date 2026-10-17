# Configuration

This folder holds the adapters for everything the program keeps outside the database.
It covers settings, posture, print setting, language catalogs, the audit log, paths, the clock, the lease and the workspace pointer.
It stands apart from Database because each file here is small JSON or text that the user may hand-edit.
Each adapter owns its JSON shape, and Core holds no JSON.

## `LWorkspaceRoot`

It resolves and persists the user's workspace folder.
That folder owns the user's settings and database.
The chosen path is the one piece of state that cannot live in the workspace.
So it is kept in a small pointer file under the user's application-data folder.
Everything else the program persists goes inside the resolved workspace.

`LWorkspaceRootRead` returns the current folder and creates it if needed.
With no choice made, a default under the user profile is used.
The same default applies when the pointer file exists but cannot be read.

`LWorkspaceRootChange` records the path in full form and creates it.
The pointer then reads the same as the rig's root, whatever the user typed.

`LWorkspaceDatabaseRead` gives the database file path, and the database lives only there.

`LWorkspaceDraftRead` gives the folder holding one file per tentative record.
Drafts live beside the database and never inside it.
That is what lets unsaved work survive a forced shutdown.
The pronunciation cache under `temp` is a separate folder and is never reused for this.

`LWorkspaceCourtRead` gives the folder holding one file per tentative link.
`LWorkspaceClaimRead` gives the folder holding one file per claim on a tentative record.
`LWorkspaceBrokenRead` gives the folder where the sweep sets aside a draft file it cannot read.
All three sit under `drafts`, because they mean nothing once the draft they name is gone.
Their files sit one folder down, so a listing of the drafts folder never picks them up.
Each folder is created if needed.

`LWorkspacePendingCommit` moves a fully written pending file over its final path.
The move is retried a few times with a growing pause because Windows scanners hold a fresh file briefly.
That hold surfaces as an access-denied or sharing error on the first move and clears within milliseconds.
The last attempt lets the error through, since a path that stays locked is a real fault.
Every draft, link and claim archive completes its save through this one place.

`LWorkspaceFileSave` writes the bytes beside the target and moves them over it in one step.
A kill mid-write leaves a `.tmp` file, never a truncated recording or flag that would be served forever.
The recording archive and the flag cache in `LEnsignLoader` both save through it.

## `LSettingsLoader`

It loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The caller supplies the folder through `LWorkspaceRoot`, and this loader never decides where it is.
A missing file yields defaults, and unknown keys are ignored.
Removed window-posture keys therefore do not block loading recognized settings.
Unparseable JSON is copied to `settings.broken.json` before defaults are returned.
A failed copy blocks later saves until another read clears the unread state.
Valid JSON whose root is not an object reads as defaults and is not copied aside.
So the next save writes over it.
A file that cannot be opened reads as defaults too, so a locked file never stops the start.
It also marks the loader, so a save refuses to write until a later read succeeds.

`LSettingsExist` lets the engine ask before moving onto a workspace, so one with its own preferences keeps them.

`LSettingsSave` writes through a pending name and moves it into place, so a crash mid-write leaves the old file whole.
A folder that cannot be created or a file that cannot be written raises `LVaultFault` around the system's exception.
After a read that failed to open the file, it raises `LVaultFault` without writing.

## `LKeepFile`

It is the disk behind the keep port, with one `{name}.json` file per name under the workspace root.
The root is fixed at construction, so the engine builds a fresh one for whatever workspace stands open.
Writes go under a pending name and move into place, the same move the settings loader makes.
So both files survive a crash the same way.

`LKeepRead` reads a missing file as nothing, so the owner falls back to its defaults and starts.
A file that exists but will not read throws.
The owner then keeps what it holds and does not write over the file.

`LKeepSave` creates the workspace folder first, so the first save into a fresh workspace succeeds.
A failed write throws the system's own exception, unlike the settings save, which wraps it in `LVaultFault`.

## `LPostureFile`

It keeps the posture as JSON under the workspace root, through the keep it is built over.
The keys are the ones the settings file carried before the posture moved out, so an old file reads as-is.
The window block and panel widths an old file carries are ignored, since the GUI driver's Capsule keeps them.
A layout block holds one object per tab under its lowercase name.
The two conversions are public so a test can run them on text.

`LPostureRead` gives the posture kept under the name, or nothing while the keep holds none.
`LPostureSave` replaces it whole.
A keep that cannot be read or written raises `LVaultFault` around the file system's own exception.

`LPostureFileParse` reads nothing, a blank, or text that is not JSON as the default posture, so the window always opens.
Only the editor word opens a split.
A volume outside the range is clamped, so a hand-edited number never reaches the player.
A missing number means full, so a workspace written before the setting existed plays as it did.

`LPostureFileFormat` keeps persisted keys lowercase and independent of member names, as they are a data contract.
The split is written as one of two words rather than a boolean.
So a hand reader sees which side stood open.
A mode and a layout are written only once each was set.
So the file says only what the user set.

`LPostureLayoutRead` drops a tab whose ordering and filter are both missing or unusable.
An unusable block as a whole yields an empty list, so every tab opens as designed.

`LPostureLayoutCreate` leaves out an ordering never chosen and a filter never touched.
An ordering is written by its stored name, never by its position in the enumeration.

`LPostureOrderResolve` keeps an ordering only when its stored name is one this build offers.
A name this build no longer knows counts as missing, and the panel opens on its designed ordering.

`LPostureFilterResolve` reads a filter as the joined text the catalog writes it in.
An empty text reads as the empty filter, which hides nothing.

## `LPressFile`

It keeps the print setting as JSON in `press.json` beside the posture file, through the keep it is built over.
The keys are the record's member names, so renaming a member drops that choice from a kept file.
The two conversions are public so a test can run them on text.

`LPressVaultRead` gives the kept setting, or nothing while the keep holds none.
`LPressVaultSave` replaces it whole.
A keep that cannot be read or written raises `LVaultFault` around the file system's own exception.

`LPressFileParse` answers null when the text is blank or malformed, and never throws.
So a damaged file only loses the remembered choices.
The result is not yet valid, so the caller clamps it against a real printer.

`LPressFileFormat` writes the setting exactly as given.
The caller empties the page range first, since pages belong to one document.

## `LLocalizationLoader`

It reads the embedded interface-language file of one language into raw pairs.
The files are embedded in this project, so `LLocalization` in the application ring opens no resource of its own.
The engine reads through here and hands the pairs to `LLocalization.LLocalizationLoad`, which resolves and keeps the catalog.

`LLocalizationScan` lists the languages the build embeds, read off the manifest resource names in ordinal order.
`LLocalization` tests a chosen language against that list, so no language is named in code.

`LLocalizationRead` parses the raw pairs and closes the file before it returns.
A language the build does not embed throws `InvalidDataException`.

`LLocalizationLoaderParse` lifts one catalog off its JSON as a flat map, `terms.*` keys beside the text keys.
A file opens with `terms.*` entries and closes with one `texts` object, and any other shape is refused.
Every value must be a string and no key may repeat.
It is public so a test can parse its own text the way the engine's rig does.

The private scan adds every text of the `texts` object and refuses one that wears the term prefix.
The private add names its kind in the refusal when a value is not a string or repeats.

## `LAuditWriter`

It is the workspace's record of faults the program did not expect.
The shell can then say something plain to the user and still keep what a developer needs.
It is written beside the database as `audit.log`, because a fault belongs to the workspace it happened in.

`LAuditRecord` appends one fault as a note carrying the whole exception rather than its message alone.
A fault is read later by someone who was not there.
The type and the stack are the part worth keeping.

`LAuditWriterRecord` appends one note stamped in UTC and answers with the file it wrote.
Not every entry is a fault.
A migration that drops data reports what it dropped, with no exception behind it.
It answers `null` when the log cannot be written.
A workspace on a read-only disk still has to report its faults.
Failing to record one must never become a second failure shown over the first.

## `LTrailSystem`

It answers the trail port through `System.IO.Path`, the rules of the operating system the program runs on.
It never touches the disk, so every answer is a pure function of its text.

`LTrailResolve` returns a fully qualified path in full form.
Any other path is anchored under `root`, and one that climbs back out resolves to nothing.
Text the system cannot read as a path resolves to nothing rather than throwing.

`LTrailRelativeResolve` answers nothing when the system gives a rooted path or a climb out of `root`.
Only a whole `..` segment is a climb, so a name like `..notes.txt` is kept.

`LTrailNameRead` trims a trailing separator first, so a folder path names the folder.
`LTrailNameNormalize` turns every character the system bars from a file name into an underscore.

## `LClockSystem`

It answers the clock port with the system clock.
`LClockRead` gives UTC, so a stamp reads the same wherever the workspace travels.
`LClockPause` is a real wait through `Task.Delay`, cancelled by the token.

## `LLeaseFile`

It answers the lease port with the file `workspace.lease` under the workspace root.
The file is opened exclusively and set to vanish when closed, so a killed program leaves no stale claim.
`LLeaseOpen` does nothing while this lease is already held.
A sharing or lock violation raises `LLeaseFault` around the system's exception, since another holder has the file.
Any other file fault passes through as it came.
`LLeaseClose` releases the file, and closing an unheld lease is harmless.

## Inline notes

### LSettingsLoader.cs: Dictionary<string, object> payload = new(StringComparer.Ordinal)

Persisted keys are a data contract, so they stay lowercase and independent of member names.
An older file's `fanqie` and `script` keys are ignored on read and dropped on the next save.
The editor's box states now live per entry in the database.

### LSettingsLoader.cs: flag.ValueKind == JsonValueKind.True

The respelling switch is read only as a JSON boolean, and anything else means off.
A missing key means off, so a workspace written before the switch existed shows source transcriptions as before.

### LSettingsLoader.cs: fetch.ValueKind != JsonValueKind.False

The frequency switch defaults on, so only an explicit JSON `false` turns the fetch off.
A missing key means on, so an older workspace starts fetching without being edited.

### LSettingsLoader.cs: custom.ValueKind != JsonValueKind.False

The custom analysis switch is read under the `analysis` key and defaults on the same way.
A missing key means on, so an older workspace keeps its custom sheets and marked letters.

### LSettingsLoader.cs: byname.ValueKind != JsonValueKind.False

The epithet switch defaults on the same way, so an older workspace lists its Han characters with their readings.

### LSettingsLoader.cs: set.ValueKind == JsonValueKind.True

The tally switch defaults off like the respelling switch, so an older workspace prints its tallies in IPA.

### LSettingsLoader.cs: && speech.GetString() is { Length: > 0 } gloss

The gloss language is read only as a non-empty JSON string, and anything else keeps the record's default.
A missing key keeps the default, so an older workspace starts new translations in English as before.

### LSettingsLoader.cs: port.TryGetInt32(out int number) &&

The Joplin port is read under the `outpost` key, only as a whole number from 1 to 65535.
Anything else means the default Joplin port, so a hand-edited file never aims the push at no port.

### LSettingsLoader.cs: token.ValueKind == JsonValueKind.String

The Joplin token is read under the `warrant` key, only as a string.
Anything else means empty, so an older workspace starts with no token set.

### LSettingsLoader.cs: title.ValueKind == JsonValueKind.String

The Joplin notebook title is read under the `notebook` key, only as a string.
The record trims it, and a blank or missing one becomes `Llyn`.
So an older workspace keeps syncing into the notebook it always had.

### LSettingsLoader.cs: folder.ValueKind == JsonValueKind.String

The deck sync folder is read under the `depot` key, only as a string.
Anything else means empty, so an older workspace starts with no folder chosen.

## Stamp

Commit: `52c21d68b4c4`
- `LAuditWriter.cs` `dd4172f169ed53a1`
- `LClockSystem.cs` `b36331da33b322ac`
- `LKeepFile.cs` `c6c78aa9341281fa`
- `LLeaseFile.cs` `c576bdb367b6ba21`
- `LLocalizationLoader.cs` `c26518d6f35865af`
- `LPostureFile.cs` `ac5bf5c7f5c371e1`
- `LPressFile.cs` `d65a8925b22bf89c`
- `LSettingsLoader.cs` `cfe342fe0adef4c1`
- `LTrailSystem.cs` `5a881c39877d0491`
- `LWorkspaceRoot.cs` `17802ced47eeb2be`
