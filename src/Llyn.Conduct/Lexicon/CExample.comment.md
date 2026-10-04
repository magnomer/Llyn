# CExample.cs
Hash: `432033e6ca4b834f`

## `public sealed record CExample(string CExampleLanguage, CStateValue CExampleText, long? CExampleSource, string CExampleCitation, string CExampleTally, IReadOnlyList<CGlossDraft> CExampleGloss, IReadOnlyList<CMentionPiece> CExamplePiece)`

One stored Example, as the corpus transcript and excerpt read it.

**Parameters**

- `CExampleLanguage`: the language the example is written in.
- `CExampleText`: the example's text, uncertain when it is unknown.
- `CExampleSource`: the reference the example cites, null when none.
- `CExampleCitation`: the line the cited reference is shown under, ready from the engine, empty when none.
- `CExampleTally`: the tally chip's sentence for the chosen Example, worded by the engine.
- `CExampleGloss`: the translations of the example, in order.
- `CExamplePiece`: the text divided at the Mentions the excerpt links, plain unless the text reads soundly.

## `public string CExampleTextHint`

The key of the sentence field's placeholder, which Conduct chooses and the driver looks up.

## `public string? CExampleWording`

The key the excerpt words instead of the text, which Conduct chooses and the driver looks up.
An unknown text reads the unknown mark, and a never-written one reads `Example.Unwritten`.
Null while the text itself shows.

## `public bool CExampleMuted`

Whether the excerpt shows its sentence muted, which only a never-written text does.
An unknown text keeps the ink, since the unknown mark is a written state.

## `internal static string LExampleHintRead(bool uncertain)`

A never-written sentence asks for a sentence, and an unknown one reads the unknown mark.
The text gate answers the typed form, since typing ends an unknown mark.

## `internal static CExample LExampleBlankRead(string tally)`

The empty Example the transcript shows while no draft is held, carrying the tally.
The corpus raises it instead of null, so the driver keeps no default of its own.
