# CTimbre.cs

## `public sealed class CTimbre`

The sound facts of the entry an editor holds, as the editor shows or offers them.
The pack facts are read for the held draft's language, and the waiting reflexes from the editor's display.
The editor builds it over its desk, its phonology and media ports and its display, so it keeps no copy.

## `public event Action? CTimbreParadigmChanged;`

The held entry's paradigm changed, raised on the driver's thread.
`CTimbreScriptChanged` and `CTimbreReflexChanged` do the same for the script groups and the reflexes.

## `public bool CTimbreFlagged`

Whether the held draft's language has a flag.

## `public IReadOnlyList<string> CTimbreVarietyNames`

The varieties the held draft's language offers, empty while no draft is held.

## `public bool CTimbreReflexShown`

Whether the reflex block shows for the held draft.

## `public bool CTimbreReflexPending`

Whether the held entry's reflex lookup is still running, so the reflex block shows its loading line.

## `public IReadOnlyList<CContour> CTimbreContourRead(string ipa)`

The tone contour of `ipa` as typed, in the held draft's language.
It is empty when the language is not tonal or the reading carries no tone, which hides the contour.
The editor reads it on every keystroke, since the typed text runs ahead of the draft.

## `public CTimbrePlayback CTimbrePlaybackRead()`

The held draft's own recording while its file exists, and whether the playback tray shows.
An empty desk has nothing to play and asks the engine nothing.

## `public Uri? CTimbrePlaybackStart(string? audio)`

The user pressed play on the recording `audio`, and the gate answers the address the driver's player opens.
A file gone since the button was painted answers null, and the driver repaints from a fresh read.

## `public Uri? CTimbreAudioStart(long accent)`

The user pressed play on the accent row `accent`, and the gate answers the address the driver's player opens.
A file gone from disk answers null, and the engine clears it off the row.
The draft bulletin then repaints the row without its play button.

## `public void CTimbreAccentSet(long accent, string text)`

The user typed `text` on the accent row `accent`.
The engine defers it and picks the respelled or phonetic form by the pack.

## `public void CTimbrePronunciationAdd(long accent)`

The user pressed plus on the accent row `accent`, or on the primary for id zero.
A blank pronunciation follows that row.

## `public void CTimbrePronunciationRemove(long accent)`

The user pressed minus on the accent row `accent`, or on the primary for id zero.

## `public CTimbreAccent CTimbreAccentRead()`

The held draft's accent sheet, ready to paint.
It carries every row after the primary, the primary variety, the mark and the flag verdict.
The engine resolves each row's printed form, so the respelling rule keeps its one owner below.
An empty desk answers the mute sheet and asks the engine nothing.

## `public async Task<CTimbreAccent?> CTimbreFlagRead(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags the held sheet draws into `store`, then answers a fresh sheet.
A language whose pack labels varieties by name answers null at once, so the editor paints nothing more.
A language or desk changed meanwhile answers null, so a late load paints nothing.
A load or read that fails answers null too, and the labels stand.

## `private static CTimbreAccent LTimbreAccentRead(LAccentSheet sheet)`

Maps the engine's sheet into the Conduct record, reading no rule.

## `public void CTimbreReflexStart()`

Starts the reflex lookup for the held entry, which the editor asks when a stored entry opens without reflexes.
Nothing is started for a fresh draft, since a lookup needs a stored entry.

## `public void CTimbreReflexRebuild()`

The user asked to look the held entry's reflexes up again.

## `internal void LTimbreObserverAttach(Action<Action> marshal)`

Hears the paradigm and reflex subjects of the held entry and the tenure's script subject.

## `public bool CTimbreSpoken`

Whether the held draft's pack is spoken, so the pronunciation and accent rows show.

## `public bool CTimbrePhonemic`

Whether the reading field shows a phonemic respelling, which needs a respelling pack first.

## `private string LTimbreLanguage`

The held draft's language, or empty while no draft is held.

## `private long? LTimbreEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `private LTenure? LTimbreTenure`

The held draft's tenure for a write, or null while the desk fills its view.
So a row the render writes raises no request.
