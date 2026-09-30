# CLeaf.cs

## `public sealed record CLeaf(`

One meaning or collocation card of the reading view, ready to paint.
Every text arrives worded and every row arrives mapped, so the leaf fill only looks keys up and paints.

**Parameters**

- `CLeafPosition`: the card's number, shown in its badge.
- `CLeafTitle`: the title, with the unknown mark's key while unknown and muted while never written.
  A muted title leaves the card's kind caption in its place.
- `CLeafExpression`: the collocation's expression, muted while never written, so its line folds.
- `CLeafMeaning`: the definition, muted while never written, so its line folds.
- `CLeafSituation`: the situation chips, in the order the card keeps them.
- `CLeafRegister`: the register chips, in the order the card keeps them.
- `CLeafTag`: the tag chips, in the order the card keeps them.
- `CLeafTranslation`: the entries the card links to, named, without a link the engine no longer finds.
- `CLeafSentence`: the example lines, each ready with its frame, sentence and Source line.
- `CLeafImage`: the picture rows, each carrying whether its location is empty.
- `CLeafVideo`: the video rows, each carrying whether its location is empty.

## `internal static IReadOnlyList<CLeaf> LLeafRead(`

Maps the shown entry's cards of one list to their ready form.
The order, the unknown mark, the Source lines and the link targets are read once for the entry.
Each card's links are its own entry of the target map, which the engine keys by every card.
The rules stay below, so this map only pairs and chooses no key but the unknown mark's.
Each picture row carries the address `media` resolves, so the page loads it without asking.
