# TDisplayAccent.cs
Hash: `5ee40ff6798ed4f6`

## `public sealed class TDisplayAccent`

Covers the reading view's pronunciation block, read and flag load alike, on a real workspace.
The wing opens the entry, so each case stands on its display's accent area.
Each flag load goes through the `TInterfaceConductSound.TDisplayAccentLoad` relay.

## `public void DisplayAccentRead_ShownEntry_AnswersTheBlockReadyToShow()`

An English entry answers square brackets, no contour, its phonetic reading and one row for its second pronunciation.
Each variety carries its label and flag keys, and the pack draws varieties as flags.

## `public void DisplayAccentRead_RespellingShown_PrintsEachReadingRespelled()`

With the switch on, the primary reading prints the respelling stored beside it.

## `public void DisplayAccentRead_NothingShown_AnswersTheMuteBlock()`

With nothing open, the block has no reading, no row and no flag.

## `public async Task DisplayAccentLoad_FlaggedPack_StoresTheVarietyFlagsAndAnswersTheBlock()`

A pack that draws its varieties as flags hands the store each flag under its language and variety.
The block then answers again, with the pack's tone and flag verdicts.

## `public async Task DisplayAccentLoad_AnotherEntryShownMeanwhile_AnswersNothing()`

An entry opened while the flags load wins, so the late load answers nothing to paint.

## `public async Task DisplayAccentLoad_StoreFails_ShowsTheLoadFailureOnce()`

A flag store that throws shows `Sound.LoadFailed` once, and the load answers nothing.

## `public async Task DisplayAccentLoad_NothingShown_AnswersNothing()`

With nothing open, nothing loads and nothing answers.

## `internal static TLanguageFixture TDisplayPackCreate()`

A tonal pack whose one variety draws a flag kept in a local file, so no flag is fetched.

## `private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A wing on its left side whose envoy notes every question and failure in `asked`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string language, string reading)`

Stores an entry with a British and an American pronunciation of `reading`.
