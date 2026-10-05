# TEnvoyFake.cs
Hash: `34b14489609601be`

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

## `internal static CEnvoy TEnvoyCoinageCreate(string? wording, bool? leave, List<string> asked)`

A fake envoy whose wording question records `Coinage:` plus the message key and answers `wording`.
The leave question records `Leave` and answers `leave` as given.
It records each failure key it is shown, so a fact reads the questions and the failure in order.

## `internal static CEnvoy TEnvoyMarkupCreate(string? path, List<string> asked)`

A fake envoy whose markup file question answers `path` unrecorded.
It records only each failure key it is shown, so a fact counts the notices alone.

## `internal static CEnvoy TEnvoyTicketCreate(Func<CPressTicket?> answer, List<string> asked)`

A fake envoy whose printer question records `Ticket` and answers from `answer`, which may throw as a failing dialog does.
It records each failure key it is shown.
