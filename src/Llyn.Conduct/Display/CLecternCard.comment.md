# CLecternCard.cs
Hash: `4f3140b154d29b23`

## `public sealed record CLecternCard(IReadOnlyList<CLeaf> CLecternCardMeanings, IReadOnlyList<CLeaf> CLecternCardOther, bool CLecternCardDefined, bool CLecternCardCollocated)`

The cards of the reading view, ready to paint, with whether their sections show.

**Parameters**

- `CLecternCardMeanings`: the entry's meaning cards, each ready as a leaf.
- `CLecternCardOther`: the collocations linked to no meaning the draft still holds, each a leaf under Other collocations.
- `CLecternCardDefined`: whether the entry has a Meaning, which shows the Meaning section.
- `CLecternCardCollocated`: whether `CLecternCardOther` holds any card, which shows the Other collocations section.
  Linked collocations show inside the leaf of the top-level meaning above their home instead.
