# TPressQueue.cs
Hash: `ffe65c85def5c970`

## `internal sealed class TPressQueue : LPressQueue`

A printer queue that asks no spooler and answers fixed printers.
So a print window is checked without a printer installed.
It sits beside `TPress` because the Core port is apart from the press.

## `internal Exception? TPressQueueFailure`

The failure every listing raises, or null to answer the printers.
A test sets it to check that a broken spooler still leaves Save as PDF and reaches the ledger.

## `public Task<IReadOnlyList<LPressPrinter>> LPressQueueRead()`

Raises `TPressQueueFailure` as a faulted task when one is set.
Otherwise it answers two fixed printers.
`Office Laser` is the default and reports its capabilities as known.
It offers A4 and Letter, single or long-edge sides, and full colour or gray.
It collates and takes at most five copies.
`Plotter` reports nothing and is marked unknown, so the gate falls back.
