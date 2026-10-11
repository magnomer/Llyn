# TEnvoyFake.cs
Hash: `5afb6f4c63ea5fd7`

## `internal static class TEnvoyFake`

Builds fake envoys over `TEngineFake`, so a gate's questions are answered and recorded without a driver.
Each fake answers only the questions its facts expect, and any other question throws.

## `internal static CEnvoy TEnvoyCreate(bool? answer, List<string> asked)`

Builds an envoy that records every key it is asked in `asked`.
A confirm answers `answer`, or no when it is null.
The leave question records `Leave` and answers `answer` as given.
The union question records its key and both names joined by `>`, and answers like a confirm.

## `internal static CEnvoy TEnvoyFileCreate(string? path, CPortraitMedium medium, List<string> asked)`

A fake envoy whose file question answers `path` and `medium` and records `File:` plus the offered name.
It records each failure key it is shown, so a fact reads the question and the failure in order.

## `internal static CEnvoy TEnvoyPressCreate(Func<CSPress, bool> answer, List<string> asked)`

A fake envoy whose print window question hands the gate to `answer` and returns its verdict.
Its file question answers `press.pdf` as a PDF and records `File:` plus the offered name.
It records each failure key it is shown.
The window question itself goes unrecorded, so `asked` holds only file questions and failures.

## `internal static CEnvoy TEnvoyPressCreate(string? path, Func<CSPress, bool> answer, List<string> asked)`

The same fake, with the file question answering `path`.
A null path stands for a reader who declines the Save as PDF file dialog.

## `internal static CEnvoy TEnvoyCoinageCreate(string? wording, bool? leave, List<string> asked)`

A fake envoy whose wording question records `Coinage:` plus the message key and answers `wording`.
The leave question records `Leave` and answers `leave` as given.
It records each failure key it is shown, so a fact reads the questions and the failure in order.

## `internal static CEnvoy TEnvoyMarkupCreate(string? path, List<string> asked)`

A fake envoy whose markup file question answers `path` unrecorded.
It records only each failure key it is shown, so a fact counts the notices alone.
