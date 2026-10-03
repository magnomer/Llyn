# TEngineShengfu.cs
Hash: `044f579d57392cb8`

## `public sealed class TEngineShengfu`

Covers the phonetic series from the pack rule to the block the fanqie box prints.
The pages are fakes, so nothing here reaches the web.

## `public async Task ShengfuStart_SeriesModule_PrintsItOnTheCharactersFirstBlock()`

One start fetches the rime books and the series together and stores one series row.
The series rides on the character's block, which is where the box prints it.

## `public async Task ShengfuStart_TwoSeriesRows_JoinsThemInAnswerOrder()`

A character listed under two series keeps both stems, in the order the answer gave them.

## `public async Task ShengfuStart_PackWithoutSeries_LeavesEveryBlockWithout()`

A pack declaring no series source fetches none and stores none.

## `public async Task ShengfuSourceFind_PatternTimesOut_AnswersNotReached()`

A pack pattern that times out answers no series and not reached, so the miss is not remembered.
