# CTimbre.cs
Hash: `b8355282ea3e753f`

## `public sealed class CTimbre`

The sound facts of the entry an editor holds, as the editor shows or offers them.
The pack facts are read for the held draft's language.
The reflex block lives in `CKindred`, which has its own ports and lookup.
The editor builds it over its desk, its language and settings ports, and its envoy.
So it keeps no copy.

## `internal CTimbre(CDesk desk, LLanguagePort languages, LSettingsPort settings, CEnvoy envoy)`

Takes `envoy`, so a failed flag load shows its notice through `CLedger`.

## `public event Action? CTimbreParadigmChanged;`

The held entry's paradigm changed, raised on the driver's thread.

## `public event Action? CTimbreScriptChanged;`

The held entry's script groups changed, raised on the driver's thread.

## `public IReadOnlyList<CContour> CTimbreContourRead(string ipa)`

The tone contour of `ipa` as typed, in the held draft's language.
It is empty when the language is not tonal or the reading carries no tone, which hides the contour.
The levels are drawn over the language port's scale, the one the reading view uses.
The editor reads it on every keystroke, since the typed text runs ahead of the draft.

## `public CAccentTyped CTimbreAccentSet(long accent, string text)`

The user typed `text` on the accent row `accent`.
The engine defers it and picks the respelled or phonetic form by the pack.
The answer names the text the row now holds, which the driver writes in place of its own copy.
A taken edit answers the typed text, since the deferred draft has not caught up yet.
A desk that holds no tenure or fills its view takes no edit.
It then answers the text its sheet holds for the row.

## `private string LTimbreAccentFind(long accent)`

The text the held sheet shows on one accent row, or nothing for a row it lacks.

## `public void CTimbrePronunciationAdd(long? accent)`

The user pressed plus on the accent row `accent`, or on the primary with null.
Null means the primary, which carries no row of its own.
Conduct maps null to the engine's id 0, so the driver never sends a magic id.
A blank pronunciation follows that row.

## `public void CTimbrePronunciationRemove(long? accent)`

The user pressed minus on the accent row `accent`.
The driver always calls it, with null when no row was carried.
Null names no row, so Conduct removes nothing then.

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
That failure shows the `Sound.LoadFailed` notice through `CLedger`, so it is never silent.

## `private static CTimbreAccent LTimbreAccentRead(LAccentSheet sheet)`

Maps the engine's sheet into the Conduct record, reading no rule.

## `public CFont CTimbreFontRead(CFontRole role)`

The typography the held draft's pack declares for `role`, through the one font rule `CFont.CFontRead` holds.
The editor paints its headword field, its example cards and its glyph row from it.
So no driver hands the editor's language back to the catalog.
A fresh draft reads its pack the same way, and an empty desk answers the blank font.

## `public CTimbreGlyph CTimbreGlyphRead()`

The held draft's glyph block, ready to paint, with the transcription rows under the glyph scheme.
The editor reads it on every draft repaint, since a language change moves the split.
An empty desk answers the hidden block with no rows.

## `internal void LTimbreObserverAttach(Action<Action> marshal)`

Hears the inflection subject of the held entry and the script subject.
An inflection bulletin without an entry id names every entry, so the held entry hears it too.
The custom analysis switch raises one, so the editor's inflection box repaints.

## `public bool CTimbreSpoken`

Whether the held draft's pack is spoken, so the pronunciation and accent rows show.

## `public bool CTimbrePhonemic`

Whether the reading field shows a phonemic respelling, which needs a respelling pack first.

## `private string LTimbreLanguage`

The held draft's language, or empty while no draft is held.

## `private LTenure? LTimbreTenure`

The held draft's tenure for a write, or null while the desk fills its view.
So a row the render writes raises no request.
