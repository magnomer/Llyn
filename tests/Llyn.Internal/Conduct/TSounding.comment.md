# TSounding.cs

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

## `public void SoundingFanqieResolve_StoredEntry_AnnouncesTheChange()`

A rebuild that lands raises the change event once and shows no notice.

## `public void SoundingFanqieResolve_RefusedRebuild_ShowsTheNotice()`

Each refused gate asks the envoy to show its own notice key and announces no change.

## `public void SoundingFanqieSet_StoredEntry_SendsTheRank()`

The rank edit reaches the engine with the stored entry, the row and the rank.

## `public void SoundingAnchorScan_StoredEntry_ComparesInTheDraftLanguage()`

The anchor scan hands the engine the language of the draft on the desk, and shapes the rows it answers.

## `public void SoundingParadigmRead_TwoSlotsOneForm_JoinsThemIntoOneRow()`

The paradigm rows the engine joined keep their part, name, form and doubt.
The paradigm language comes from the engine as well.

## `private static long TSoundingEntrySave(LEngine engine)`

Stores one English entry and answers its id.

## `private static CEditor TSoundingEditorPrepare(LEngine engine, long? entry)`

An editor on the library tab, holding `entry` or a fresh draft.

## `private static CSounding TSoundingCreate(`

A sound sheet over the editor's desk with fake ports answering `answers`.
Every notice the envoy is asked to show lands in `notices`.
