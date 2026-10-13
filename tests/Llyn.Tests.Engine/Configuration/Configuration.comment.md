# Configuration

Tests of the settings, print setting, localization, theme and file adapters.
They stand apart because they pin what survives a restart and how stored state degrades.
The settings and print setting loaders read missing, junk or legacy input as defaults rather than throw.

## `TLocalization.cs`

Covers term resolution, the fallback for a missing key, and the language switch.
The group find reads each group's texts by the shared match, trimmed.
A blank text reads every group.

## `TLocalizationLoader.cs`

A shipped language reads as raw pairs, and one the build lacks is refused.
Every embedded catalog must carry exactly the keys of the default one.
Markup binds texts as dynamic resources, so a key one catalog lacks draws a blank control, not an error.

## `TPressClerk.cs`

Covers the restore and save policy of the print setting clerk over an in-memory store.
An empty store restores nothing, so the session keeps its own setting.
A kept printer that is no longer listed falls back to the default printer.
A kept Save as PDF choice restores with no printer.
The page range comes back empty, and a save keeps the range empty but keeps the scale.
The Conduct memory facts check the empty store and the dropped range through the window's gates.

## `TPressFile.cs`

Covers the print setting adapter over a keep file.
No file reads as nothing, and a saved setting reads back equal under `press.json`.
Unreadable, blank or missing text reads as nothing and never throws.

## `TSettings.cs`

Covers the engine's settings file, including interface language and its switches.
The notebook title is saved trimmed, a blank one stores `Llyn`, and the save answers what it stored.
A missing, blank or non-string `notebook` key loads as `Llyn`, so an older workspace keeps its notebook.
A language the catalog does not list is kept as saved but reads back as the default.

Switches follow two opposite defaults, so only the explicit value moves them.
- Epithet round-trips under the `epithet` key and is written even when off.
- Tally loads as off unless the key is an explicit true.
- Respelling loads as off unless the key is a JSON boolean true.
  A missing key or a non-boolean value never turns it on, so an older or hand-edited file stays safe.
- Frequency, morphology and custom analysis load as on unless the key is an explicit false.
  Frequency round-trips under `frequency`, morphology under `morphology`, analysis under `analysis`.
- The engine's save reaches both the held settings and the file for respelling and tally.

Changing analysis raises one Settings bulletin and one all-entry Inflection bulletin.
A switch saved with the value already held raises no bulletin, so an echoing control is a no-op.
A switch that changes raises the bulletin once, and saving the same value again raises nothing more.

A file written before the window posture moved out still loads, its posture keys passed over.
A file written before the box states moved to the database still loads, its `fanqie` and `script` keys passed over.
The next save drops those keys.
A file that is not JSON loads as defaults and is copied aside as `settings.broken.json` first.
A save leaves no pending file behind and the saved file reports as existing.
A folder without a settings file reports none.

## `TThemePalette.cs`

The shell fills its resources from the palette, so every named value must be a hex colour.
The palette read refuses an unknown name.
The spare read answers a shipped name with its value and an unknown name with black.

## `TTrail.cs`

Covers the trail adapter over `System.IO.Path`, which touches no disk.
A path outside the root, or one that climbs out, has no relative form, and one inside has.
A name that merely begins with two dots, such as `..notes.txt`, is inside.
A qualified path resolves to itself and a relative one anchors under the root, unless it climbs out.
The name reads past a trailing separator, and a barred character becomes an underscore.
The root check accepts a rooted path and refuses a bare name.

## `TUsher.cs`

Covers the file usher behind the `LUsher` port: the presence check, the quiet delete and the lock check.
The lock check is pinned on a wrapped sharing fault and on a missing file.
Opening a folder or a link lives in the media ring, which the tests do not reference.

## Inline notes

### TTrail.cs: TTrailRoot

The root sits under the temp folder and never has to exist, since no call here reads the disk.

## Stamp

Commit: `c2f335c331fe`
- `TLocalization.cs` `1407ac74b05cbe5c`
- `TLocalizationLoader.cs` `aef25aefe3308c74`
- `TPressClerk.cs` `bacfa606e4abd25e`
- `TPressFile.cs` `82f2578d9e5e9c1d`
- `TSettings.cs` `34444d3b99812ac2`
- `TThemePalette.cs` `32851b33adb43566`
- `TTrail.cs` `0f416a889d3d95cd`
- `TUsher.cs` `2bb1b70a37a362bc`
