# CLecternCard.cs

## `public sealed record CLecternCard(`

What the card templates of the reading view read beside the cards, ready to show.

**Parameters**

- `CLecternCardOrder`: the order a sentence's particle and dependence stand in.
- `CLecternCardCitations`: the ready line of every Source the entry cites, keyed by its id.
  A Source that names itself nowhere is already written as its id.
- `CLecternCardTargets`: the entries the cards and the etymology link to, named.
- `CLecternCardDefined`: whether the entry has a Meaning, which shows the Meaning section.
- `CLecternCardCollocated`: whether the entry has a Collocation, which shows the Collocation section.
