# TFaultPortrait.cs
Hash: `b3f577a92abedc3e`

## `public sealed partial class TFault`

The portrait half of the fault sweep, holding one row per print or export gate.
It keeps `TFault.cs` within the line limit, and the rows join the sweep there.

## `private static IReadOnlyList<TFaultRow> TFaultPortraitRows =>`

A property, not a field, so the sweep's list reads it whatever the field order across files.
Print gates fault `LPortraitPort.LEnginePressOpen` and expect `Print.Failed`.
Export gates fault `LPortraitPort.LEnginePortraitExport` and expect `Export.Failed`.
Each faults the port member its gate awaits inside `CPortrait`, after any envoy question.
A gate that prints only a chosen row is first given one, or it would return before the port.
Corpus and repertoire prints take the quotation or occurrence on display.
Shelf print and export take the footnote entry.
Guild print takes a source of the chosen author.
Xiesheng and yunjing prints need a language pack and a chosen character row.
Their exports, and the favorite, library and phonology gates, need nothing chosen.

The print window's own gates run inside a session opened by `TFaultPressOpen`.
Its system dialog and print fault the board and expect `Print.Failed`.
Its draw faults the board's draw and expects `Print.RenderFailed`.
The sight row draws a proof first and sets the galley's edit and sight delays to zero.
Its page requests then fault the proof's draw and expect `Print.RenderFailed`.
The sight pass is the only page draw, so no other row faults the proof's draw.
The restore row faults the printer listing through the fake queue's own failure.
The fault double cannot reach the queue, so the thrown and faulted runs fault it alike.

## `private static Task<Func<Task>> TFaultPressOpen(TFaultStage stage, Func<CSPress, Task> gate)`

Opens a print window over a fake board wrapped in the fault double, and runs `gate` in it.
The board also wraps each proof it hands out, so a proof member can fault too.
The returned call opens the session, so the stage's heard list starts empty before it.
Every ledger key the session shows lands in that list.

## `private static CAtelier TFaultAtelierCreate(TFaultStage stage, LEngine engine) =>`

Builds the atelier over a given engine's faulted outlets, disposed with the stage.
Rows that save their own rows first need the engine in hand.

## `private static CEnvoy TFaultEnvoyCreate(TFaultStage stage) =>`

An envoy that answers the file question unrecorded.
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
