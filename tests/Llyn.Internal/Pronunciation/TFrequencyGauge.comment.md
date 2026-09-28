# TFrequencyGauge.cs

## `public sealed class TFrequencyGauge`

Covers the rules that gather an entry's frequency rows into the one answer its chip shows.
They sat in the conduct's display before, and they now live beside the frequency record.

## `public void FrequencyGaugeResolve_NoRows_ReturnsNone()`

An entry no source answered has no frequency, so the chip hides.

## `public void FrequencyGaugeResolve_UnbandedRows_BandsZero()`

Rows that carry no band resolve to the unknown band.

## `public void FrequencyGaugeResolve_BandedRow_BandsItsRank()`

The first row that carries a band resolves to that band's rank.

## `public void FrequencyGaugeResolve_UnknownBandFirst_BandsTheNextRank()`

A first row whose band is not a ladder name is passed over for the next row's band.

## `public void FrequencyGaugeResolve_OnceInterval_FormatsTheInterval()`

A row with a once interval shows the interval in the given pattern, and a plain row shows its figure.
Each source takes its own line.
