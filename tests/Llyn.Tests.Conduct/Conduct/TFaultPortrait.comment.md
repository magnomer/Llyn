# TFaultPortrait.cs
Hash: `fefe1a1c4022ea77`

## `public sealed partial class TFault`

The portrait half of the fault sweep, holding one row per print or export gate.
It keeps `TFault.cs` within the line limit, and the rows join the sweep there.

## `private static IReadOnlyList<TFaultRow> TFaultPortraitRows =>`

A property, not a field, so the sweep's list reads it whatever the field order across files.
Print gates fault `LPortraitPort.LEnginePortraitPrint` and expect `Print.Failed`.
Export gates fault `LPortraitPort.LEnginePortraitExport` and expect `Export.Failed`.
Each faults the port member its gate awaits inside `CPortrait`, after the envoy answered.
A gate that prints only a chosen row is first given one, or it would return before the port.
Corpus and repertoire prints take the quotation or occurrence on display.
Shelf print and export take the footnote entry.
Guild print takes a source of the chosen author.
Xiesheng and yunjing prints need a language pack and a chosen character row.
Their exports, and the favorite, library and phonology gates, need nothing chosen.

## `private static CAtelier TFaultAtelierCreate(TFaultStage stage, LEngine engine) =>`

Builds the atelier over a given engine's faulted outlets, disposed with the stage.
Rows that save their own rows first need the engine in hand.

## `private static CEnvoy TFaultEnvoyCreate(TFaultStage stage) =>`

An envoy that answers the file and printer questions unrecorded.
It records only failure keys, so the sweep sees the gate's notice alone.
The faulted port never writes, so the answered file name is never created.

## `private static CTenor TFaultTenorStart(TFaultStage stage)`

A register tenor whose cohort has an entry open, so its print and export pass their guard.

## `private static CTaxonomy TFaultTaxonomyStart(TFaultStage stage)`

A taxonomy whose membership has an entry open, so its print and export pass their guard.

## `private static CCorpus TFaultCorpusStart(TFaultStage stage)`

A corpus with a quoting entry on display, so the quotation is shown.
The corpus print and the quotation export then both reach the port.

## `private static CRepertoire TFaultRepertoireStart(TFaultStage stage)`

A repertoire with an occurrence on display, so the repertoire print and export reach the port.

## `private static CShelf TFaultShelfStart(TFaultStage stage)`

A shelf with an entry selected, so the footnote press is allowed for print and export.
