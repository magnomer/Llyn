# TImprint.cs
Hash: `e5dd5fa28f16b82e`

## `public sealed class TImprint`

Covers the source editor's fields and draft on a real workspace.
A typed title crosses raw and the engine holds it specified, while a blank year holds nothing.
A kind tag round-trips through the draft and takes one chronicle step, and a null tag sends nothing.
A tag naming the kind the Source already has sends nothing either.
A save on an unchanged Source keeps the draft, and one after a change commits it.
Opening a stored Source announces its fields, and the empty read gives the blank Source's hints.
The kind menu offers every kind in menu order, each with its tag and localization key.
Hostile kind rows give an empty menu, or one option per ordinal tag, the first kind kept.
A cancel drops the draft and closes the byline with one notice.

## `internal static CImprint TImprintPrepare(LEngine engine, CAtelier atelier)`

Builds the editor through `TInterfaceCitation.TImprintCreate` over the atelier and a source vista.
Its marshal runs each desk notice at once.
The credit and byline suites share it.
