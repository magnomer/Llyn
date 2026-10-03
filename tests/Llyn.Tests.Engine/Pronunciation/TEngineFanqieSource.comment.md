# TEngineFanqieSource.cs
Hash: `04c73d65ec8a727d`

## `public sealed class TEngineFanqieSource`

Covers the fanqie sources that read plain text by line pattern, over a fixture pack with stubbed pages.
It builds through `TEngineFanqie.TFanqieFetchRead`, `TFanqieSettle`, `TFanqieCountCheck` and `TFanqieDraftCreate`.
A book with a line pattern is fetched by GET and read as plain text, one row per match.
Its rows carry its own source label.
Two sources of one book both store, since each counts its positions from zero under its own source.
A not-found answer from such a book counts as reached with nothing, so the character is not asked again.
The wiki pack and its page are kept here for the rank facts in `TEngineFanqieRank` to reuse.

## `public async Task FanqieSourceFind_PatternTimesOut_AnswersNotReached()`

A pack pattern that times out answers no rows and not reached, so the miss is not remembered.
