# TSounding.cs
Hash: `8cc444dcb7358512`

## `public sealed class TSounding`

Covers the editor's sound sheet gates and reads end to end, driven with no window.
A real desk holds the entry, and fake ports let a test shape or refuse each engine answer.

## `public void SoundingVarietyRead_NamedOrBlankVariety_KeysTheLabelAndTheFlag()`

A named variety keys its label under `Variety.` and its flag under the ensign format.
A blank variety has no flag key, and its label key finds no text.

## `public void SoundingFanqieRead_FreshDraft_AnswersEmpty()`

A fresh draft has no stored entry, so every read answers empty without asking the engine.

## `public void SoundingFanqieRead_StoredEntry_ShapesTheEngineGroups()`

The sheet asks the engine for the stored entry's groups and shapes every row.

## `public void SoundingFanqieRead_RefusedRead_AnswersEmpty()`

A refused read answers empty, since a box that cannot fetch still has to draw.
A refused pack check offers no rebuild.

## `public void SoundingFanqieResolve_StoredEntry_AnnouncesTheChange()`

A rebuild that lands raises the change event once and shows no notice.

## `public void SoundingFanqieResolve_RefusedRebuild_ShowsTheNotice()`

Each refused gate asks the envoy to show its own notice key and announces no change.

## `public void SoundingFanqieSet_StoredEntry_SendsTheRank()`

The rank edit reaches the engine with the stored entry, the row, the held rank and the raise flag.

## `public void SchemeKeyRead_SchemeOrBlank_PrefixesTheSchemeKey()`

A scheme is labelled under `Scheme.` plus its name, and a blank one keys `Scheme.` alone.

## `public void SoundingParadigmRead_UnansweredSlot_AnswersTheTipOfItsPendingAndMorphologyVerdicts(`
An unanswered slot answers the pending tip while the fetch runs, whatever the morphology setting says.
After the fetch it answers the held tip when morphology is on, since the editor's draft holds the missing form.
With morphology off it answers the absent tip.

## `public void SoundingParadigmRead_TwoSlotsOneForm_JoinsThemIntoOneRow()`

The paradigm rows the engine joined keep their part, name and form.
An unknown slot answers its mark and tip key.
The block also carries the font of the language the engine resolves.

## `public void SoundingScriptRead_StoredEntry_AnswersTheWholeBlockInTheDraftLanguage()`

The script and rime-book blocks carry the waiting checks and the rebuild offers.
The pack checks and the glyph fonts are asked in the draft's language.

## `public void SoundingFanqieRead_NoStoredEntry_OffersNoRebuildAndWaitsForNothing()`

A desk with no stored entry offers no rebuild and waits for nothing, whatever the pack says.

## `public void SoundingDiweiOpen_HeldDraft_OpensTheCellInTheDraftLanguage()`

A pressed initial or rime key reaches the navigation's yunjing opener with the draft's language.
The engine names the kind from the initial flag.

## `public void SoundingDiweiOpen_EmptyKey_OpensNothing()`

A blank key of either kind never reaches the navigation.

## `private static CEditor TSoundingDiweiPrepare(LEngine engine, CAtelier atelier, List<string> cells)`

An editor holding a stored English entry, beside a yunjing tab whose opener records each cell in `cells`.

## `private static long TSoundingEntrySave(LEngine engine)`

Stores one English entry and answers its id.

## `private static CEditor TSoundingEditorPrepare(LEngine engine, long? entry)`

An editor on the library tab, holding `entry` or a fresh draft.

## `private static CSounding TSoundingCreate(`

A sound sheet over the editor's desk with fake ports answering `answers`.
Every notice the envoy is asked to show lands in `notices`.
A test that reads fonts or morphology hands its own settings as `pack`.

## `private static LSettingsPort TSoundingPackCreate(List<(string, LFontRole)> asked, bool morphology)`

A settings port that answers one font, records each language and role asked, and answers `morphology`.
