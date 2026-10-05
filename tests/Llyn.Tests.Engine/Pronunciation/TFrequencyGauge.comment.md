# TFrequencyGauge.cs
Hash: `340d7d7ba7c75f43`

## `public sealed class TFrequencyGauge`

Covers the rules that gather an entry's frequency rows into the one answer its chip shows.

## `public void FrequencyGaugeResolve_NoRows_ReturnsNone()`

An entry no source answered has no frequency, so the chip hides.

## `public void FrequencyGaugeResolve_UnbandedRows_BandsZero()`

Rows that carry no band resolve to the unknown band.
It is unranked, named unknown, and spares the whole star row.

## `public void FrequencyGaugeResolve_BandedRow_BandsItsRank()`

The first row that carries a band resolves to that band's rank.

## `public void FrequencyGaugeResolve_UnknownBandFirst_BandsTheNextRank()`

A first row whose band is not a ladder name is passed over for the next row's band.
The top rank is named core and spares no star.

## `public void FrequencyGaugeCreate_BandPastScale_ReadsUnknownRank(int band)`

A band past the ladder's end names no ladder entry, so it reads unranked and unknown.
It spares no star and never reads past the ladder, so it cannot throw.

## `public void FrequencyGaugeCreate_NegativeBand_SparesAFullRow(int band)`

A negative band reads unranked and unknown and spares exactly one full row.
The lowest integer proves the band is held before the subtraction, so it cannot overflow.

## `public void FrequencyGaugeResolve_OnceInterval_FormatsTheInterval()`

A row with a once interval shows the interval in the given pattern, and a plain row shows its figure.
Each source takes its own line.
