# CLedgerNoticed.cs
Hash: `2f38e70b00bec262`

## `internal sealed class CLedgerNoticed`

The repaint memory: the failure keys a repaint read has already shown.
A read that runs on every repaint would otherwise open one box per repaint while a fault lasts.
The atelier's [CLedger](CLedger.comment.md) holds one, so its lifetime is the atelier's and no state is static.
A user action never routes here, so its failure shows on every retry.

## `private readonly HashSet<string> _cLedgerNoticed = new(StringComparer.Ordinal);`

The keys shown since the user last acted.

## `internal void LLedgerRepaintShow(CEnvoy envoy, LSettingsPort settings, string key, Exception exception)`

Shows a repaint read's failure through `CLedger.LLedgerFailureShow` the first time `key` fails.
A key already shown is merged: nothing is read, recorded or shown again until the memory clears.
The first occurrence keeps its fault record and its notice.
A lock guards the set, since a background read may fail while the dispatcher shows another failure.

## `internal LLedgerAnswer LLedgerRepaintRead<LLedgerAnswer>(CEnvoy envoy, LSettingsPort settings, Func<LLedgerAnswer> read, LLedgerAnswer fallback, string key)`

The one try, report and fallback rule of a repaint read.
It runs `read`, and on a refusal shows `key` through `LLedgerRepaintShow` and answers `fallback`.
CSounding, CDisplaySound and CSentence read through it, each binding its own envoy and settings port.
A user action keeps its own try, since its failure shows on every retry through `CLedger.LLedgerFailureShow`.
`LDisplaySound` holds no envoy and no memory, so it raises its failures to `LDisplay`, which routes them to `LLedgerRepaintShow`.

## `internal void LLedgerNoticedClear()`

Forgets every key shown, so the next failure under any key shows again.
The ledger calls it on each settings, workspace and vista bulletin, since each follows a user act.
