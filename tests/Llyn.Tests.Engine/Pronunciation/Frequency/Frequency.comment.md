# Frequency

This folder holds the tests of frequency values.
They cover the engine fetch, bands, entry, order, session and gauge.

## `TEngineFrequency`

Covers the engine's frequency fill over a fixture pack with three sources.
Every source is asked, and each one that answers stores a row, in written order.
A source that stays silent leaves no row while the others still store theirs.
A source with a unit stamps it on numeric rows only.
A source without a `once` estimate leaves the interval null.
A fill is observed as the user sees it.
The entry is saved, the bulletin lands, and the store is read.
Grading is in `TEngineFrequencyBand`, asking again in `TEngineFrequencySession`, the entry follow in `TEngineFrequencyEntry`.
The pack, patience, observer, draft, stored-row and count helpers here serve the `TEngineFrequency` siblings.

## `TEngineFrequencyPack`

The fixture pack's JSON with three sources.
`First` has a `once` total and band patterns, `Second` a `once` factor, and `Third` a unit and no bands.

## `TEngineFrequencyPatience`

How long a test waits for a bulletin or a request count before it fails.

## `TFrequencyObserver`

Completes a task when the first frequency bulletin arrives, and ignores every other subject.

## `TFrequencyStoredSet`

Writes one frequency row straight into the database, with a null band when none is given.

## `TEngineFrequencyBand`

Covers how a stored frequency row is graded into a band when it is read.
A numeric raw is graded by interval from the source's `once` estimate, ahead of its band patterns.
A raw that is not a number is graded by the first band pattern it matches.
It grades null when none matches.
A stale or missing band is regraded from the raw and written back.
A blank language leaves the band and interval null.
The read helper drops the entry's earlier rows first, so each call reads exactly one row.

## `TEngineFrequencyEntry`

Covers how the fill follows the entry it was started for.
With the setting off, a start writes nothing and raises no bulletin.
A headword change clears the stored rows at once and refetches.
The client holds its answer behind a gate, so the cleared store is observable before the refetch lands.
A fill whose entry was deleted or whose language changed while its answer was pending writes nothing.
The language-change fixture pack is empty, so only the stale fill could write.
Nothing observable follows a deletion, so a short wait stands in for the fill settling.
An entry with a blank language reads empty and starts no fill.

## `TEngineFrequencyOrder`

Covers the order an entry's frequency rows are shown in.
Rows stored out of pack order read back in the pack's declared source order.
Sources the pack does not declare follow by name.
Undeclared names sort by codepoint, so a supplementary ideograph follows a fullwidth letter.
A first source refetched last still gives the gauge its band and heads the tooltip.

## `TEngineFrequencySession`

Covers when the fill asks its sources again within one session.
A word every source reached yet none knew is asked once, however often it is read.
A word no source could be reached for is asked again on the next read.
A read while a fill is pending leaves that fill running instead of restarting it.
The request-count poll waits up to the patience for the count, and callers then pause so the fill settles.

## `TFrequency`

Covers how a source spec turns a raw figure into a once interval and a band.
The interval is the number of words in which the word appears once, rounded to two figures.
A spec with a total divides the total by the raw count, and a per-million total divides the million.
A spec with a factor and a power raises the power to the raw class and scales it.
A spec with only a factor multiplies the raw rank by the factor.
The band grades the unrounded interval by decade into core, everyday, advanced and rare.
A spec with no figures yields no interval and no band.
A zero count against a total yields no interval.

## `TFrequencyGauge`

Covers the rules that gather an entry's frequency rows into the one answer its chip shows.
An entry no source answered has no frequency, so the chip hides.
Rows that carry no band resolve to the unknown band, which is unranked and spares the whole star row.
The first row that carries a band resolves to that band's rank.
A first row whose band is not a ladder name is passed over for the next row's band.
The top rank is named core and spares no star.
A band past the ladder's end reads unranked and unknown, spares no star and never throws.
A negative band reads unranked and unknown and spares exactly one full row.
The lowest integer proves the band is held before the subtraction, so it cannot overflow.
A row with a once interval shows the interval in the given pattern, and a plain row shows its figure.

## Stamp

Commit: `c2f335c331fe`
- `TEngineFrequency.cs` `1d4be01c6335f7b0`
- `TEngineFrequencyBand.cs` `fd58ade5d27080e7`
- `TEngineFrequencyEntry.cs` `7c9da840e11489cf`
- `TEngineFrequencyOrder.cs` `7b50311f228195d8`
- `TEngineFrequencySession.cs` `0c059ea184e978ba`
- `TFrequency.cs` `9bf0396b5536deed`
- `TFrequencyGauge.cs` `340d7d7ba7c75f43`
