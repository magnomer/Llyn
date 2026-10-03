# TCardEtymology.cs
Hash: `8fb076bb0cec6e22`

## `public sealed class TCardEtymology`

Covers the etymology gates of a card over an entry desk on a real workspace, with no delay.
The etymology gates write the narrative, the source links and the spans of the held draft.
The etymology read names a linked span by its words and its headword.
An empty desk reads no etymology chips.
Each test but the empty desk builds its card through `TCard.TCardPrepare`.

## `private static LEtymologyDraft TCardEtymologyRead(CDesk desk)`

The held draft's etymology.
