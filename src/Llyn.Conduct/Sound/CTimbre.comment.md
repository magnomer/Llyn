# CTimbre.cs

## `public sealed class CTimbre`

The sound facts of the entry an editor holds, as the editor shows or offers them.
The pack facts are read for the held draft's language, and the waiting reflexes from the editor's display.
The editor builds it over its desk, its display and its phonology, media and draft ports.
So it keeps no copy.

## `public event Action? CTimbreParadigmChanged;`

The held entry's paradigm changed, raised on the driver's thread.
`CTimbreScriptChanged` and `CTimbreReflexChanged` do the same for the script groups and the reflexes.

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

## `public CTimbreGlyph CTimbreGlyphRead()`

The held draft's glyph block, ready to paint, with the transcription rows split by the glyph scheme.
The editor reads it on every draft repaint, since a language change moves the split.
An empty desk answers the hidden block with no rows.

## `public void CTimbreTranscriptionSet(long transcription, string text)`

The user typed `text` into the transcription row `transcription`, the glyph row among them.
The engine defers it, and a filling desk writes nothing.
It sits here beside the accent text, since a transcription is a sound fact of the held entry.

## `public CTimbreReflex CTimbreReflexRead()`

The held draft's reflex block, ready to paint, as the reading view reads its own.
Every row shows, the blank ones included, each resolved by the shared reflex scan.
The anchor labels are read against the draft's headword through the shared anchor map.
The fold comes from the editor's display, which shares it with every reading view.
An empty desk answers no rows, no anchor and no fetching line.
A refused scan reaches the draft bulletin's caller, as it did from the driver.

## `private void LTimbreReflexStart(LDraft _)`

Starts the reflex lookup whenever the desk prepares a draft, before the editor's bulletin repaints.
So the first paint of a stored entry without reflexes already shows the fetching line.
The tenure decides whether a lookup is due, and a refusal is swallowed as the driver swallowed it.

## `public void CTimbreReflexRebuild()`

The user asked to look the held entry's reflexes up again.

## `public void CTimbreReflexAdd(long reflex)`

The user pressed the plus of reflex row `reflex`, or of the header when it is zero.
The engine places the new row below the pressed one, in its language and kind.

## `public void CTimbreReflexRemove(long reflex)`

The user pressed the minus of reflex row `reflex`.

## `public void CTimbreReflexToggle(long reflex)`

The user pressed the star of reflex row `reflex`.
The engine flips the main mark the draft holds, so a row it no longer holds sends nothing.

## `public IReadOnlyList<CReflexHead> CTimbreReflexSet(long reflex, CReflexField field, string text)`

The user typed `text` into the cell `field` of reflex row `reflex`.
Each cell has its own engine member, and the engine defers every one.
A typed language answers every row's lead, with the typed language standing in for the row's stored one.
Other cells answer no lead, and so does a desk that fills its view.
Only the edit in hand is overlaid, so another row's edit still deferred reads as stored.
The lead compares the raw typed language, as the scan compares the stored one.

## `private static IReadOnlyList<CReflexHead> LTimbreLeadRead(IReadOnlyList<LReflexDraft> typed)`

Marks the rows the engine answered by the lead rule `CReflex.LReflexLeadRead`, the one the scan uses.

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

## `private LQuillReflex? LTimbreQuill`

The reflex row edits over the writing tenure and the editor's phonology port, or null while the desk fills.
