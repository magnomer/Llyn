# CLecternCard.cs
Hash: `5150e6ad9d7f0a78`

## `public sealed record CLecternCard(IReadOnlyList<CLeaf> CLecternCardMeanings, IReadOnlyList<CLeaf> CLecternCardCollocations, bool CLecternCardDefined, bool CLecternCardCollocated)`

The cards of the reading view, ready to paint, with whether their sections show.

**Parameters**

- `CLecternCardMeanings`: the entry's meaning cards, each ready as a leaf.
- `CLecternCardCollocations`: the entry's collocation cards, each ready as a leaf.
- `CLecternCardDefined`: whether the entry has a Meaning, which shows the Meaning section.
- `CLecternCardCollocated`: whether the entry has a Collocation, which shows the Collocation section.
