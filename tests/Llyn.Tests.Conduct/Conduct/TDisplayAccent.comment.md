# TDisplayAccent.cs
Hash: `6a6438c3f6c27907`

## `public sealed class TDisplayAccent`

Covers the reading view's pronunciation block, read and flag load alike, on a real workspace.
The wing opens the entry, so each case stands on its display's sound area.

## `public void DisplayAccentRead_ShownEntry_AnswersTheBlockReadyToShow()`

An English entry answers square brackets, no contour, its phonetic reading and one row for its second pronunciation.
Each variety carries its label and flag keys, and the pack draws varieties as flags.

## `public void DisplayAccentRead_RespellingShown_PrintsEachReadingRespelled()`

With the switch on, the primary reading prints the respelling stored beside it.

## `public void DisplayAccentRead_NothingShown_AnswersTheMuteBlock()`

With nothing open, the block has no reading, no row and no flag.

## `public async Task DisplayEnsignLoad_FlaggedPack_StoresTheVarietyFlagsAndAnswersTheBlock()`

A pack that draws its varieties as flags hands the store each flag under its language and variety.
The block then answers again, with the pack's tone and flag verdicts.

## `public async Task DisplayEnsignLoad_AnotherEntryShownMeanwhile_AnswersNothing()`

An entry opened while the flags load wins, so the late load answers nothing to paint.

## `public async Task DisplayEnsignLoad_NothingShown_AnswersNothing()`

With nothing open, nothing loads and nothing answers.

## `internal static TLanguageFixture TDisplayPackCreate()`

A tonal pack whose one variety draws a flag kept in a local file, so no flag is fetched.

## `private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A wing on its left side whose envoy notes every question and failure in `asked`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string language, string reading)`

Stores an entry with a British and an American pronunciation of `reading`.
