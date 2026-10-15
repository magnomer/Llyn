# Timbre

This folder holds the sound facts of the entry an editor holds.
They are the accent sheet, glyph scheme, playback facts and the playback gate of the held draft.

## `CTimbre`

The sound facts of the entry an editor holds, read for the held draft's language.
The editor builds it over its desk, its language and settings ports, and its envoy.
It keeps no copy of the draft.
The reflex block lives in `CKindred`, which has its own ports and lookup.

### Accent sheet

`CTimbreAccentRead` answers the held draft's accent sheet with every row after the primary.
The engine resolves each row's printed form, so the respelling rule keeps its one owner below.
An empty desk answers the mute sheet and asks the engine nothing.

`CTimbreAccentSet` answers the text the row now holds, which the driver writes in place of its own copy.
A taken edit answers the typed text, since the deferred draft has not caught up yet.
A desk that holds no tenure or fills its view takes no edit.
It then answers the text its sheet holds for the row.

`CTimbrePronunciationAdd` maps null to the engine's id 0, so the driver never sends a magic id.
Null means the primary, which carries no row of its own.
`CTimbrePronunciationRemove` with null names no row, so Conduct removes nothing then.

`CTimbreFlagRead` loads the flags the held sheet draws into `store`, then answers a fresh sheet.
A language whose pack labels varieties by name answers null at once.
A language or desk changed meanwhile answers null, so a late load paints nothing.
A failed load or read answers null too, and the labels stand.
That failure shows the `Sound.LoadFailed` notice through `CLedger`, so it is never silent.

### Contour, font and glyph

`CTimbreContourRead` is empty when the language is not tonal or the reading carries no tone.
The levels are drawn over the language port's scale, the one the reading view uses.
The editor reads it on every keystroke, since the typed text runs ahead of the draft.

`CTimbreFontRead` goes through the one font rule `CFont.CFontRead` holds.
A fresh draft reads its pack the same way, and an empty desk answers the blank font.

`CTimbreGlyphRead` is read on every draft repaint, since a language change moves the split.
An empty desk answers the hidden block with no rows.

### Observers and flags

`LTimbreObserverAttach` hears the inflection subject of the held entry and the script subject.
An inflection bulletin without an entry id names every entry, so the held entry hears it too.
The custom analysis switch raises one, so the editor's inflection box repaints.

`CTimbrePhonemic` needs a respelling pack first.
`LTimbreTenure` is null while the desk fills its view, so a row the render writes raises no request.

## `CTimbreGlyph`

The engine picks the transcription rows under the pack's glyph scheme, so no driver matches schemes.
Blank rows are kept.

## `CPlayback`

The playback of the draft an editor holds, with its own recording, each accent row's recording and the tray.
`CPlaybackRead` asks the engine nothing for an empty desk.
`CPlaybackStart` answers null for a file gone since the button was painted, and the driver repaints from a fresh read.
`CPlaybackAccentStart` answers null for a file gone from disk, and the engine clears it off the row.
The draft bulletin then repaints the row without its play button.
A desk that fills its view answers null, so a render never resolves audio.

## `CTimbreAccent`, `CTimbrePlayback`

The editor's accent sheet and playback facts, each ready to paint.
The playback audio is null once its file is gone, which hides the play button.
The tray shows when the own recording or any accent row has audio.

## Stamp

Commit: `2f9a06b8eb95`
- `CPlayback.cs` `91617d371b5019f5`
- `CTimbre.cs` `5c0a0d1031381e5d`
- `CTimbreAccent.cs` `d8db05a6ce76e326`
- `CTimbreGlyph.cs` `effdd102dadc3581`
- `CTimbrePlayback.cs` `2aea6bb0d1a8aaf4`
