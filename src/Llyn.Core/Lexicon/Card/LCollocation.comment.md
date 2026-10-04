# LCollocation.cs
Hash: `7410a18b42121456`

## `public sealed record LCollocation(long LCollocationId, long LCollocationEntryId, int LCollocationPosition, LStateValue LCollocationTitle, LStateValue LCollocationExpression, LStateValue LCollocationMeaning)`

One Collocation owned by an entry, a stable-id node mirroring the Meaning construction.
It carries an `LCollocationExpression`, the phrase itself.
Beside it stands the `LCollocationMeaning` that explains it, the definition analogue of a Meaning.
The card has a field for each, so a collocation keeps both, beside the `LCollocationTitle` the card is headed with.
`LCollocationId` is the identity, an opaque and program-generated stable id.
Reordering collocation cards rewrites `LCollocationPosition` only.
The id never changes.

**Parameters**

- `LCollocationId` — Opaque, program-generated stable id.
- `LCollocationEntryId` — Owning entry id.
- `LCollocationPosition` — Order among the entry's collocations.
- `LCollocationTitle` — Title typed on the Collocation card, and what is known about it.
  Nothing was recorded when none was typed.
  It is unknown when the user marked it as not known.
- `LCollocationExpression` — The collocation expression text, and what is known about it.
- `LCollocationMeaning` — What the expression means, and what is known about it.

## `public LStateValue LCollocationTitle { get; init; }`

A null handed in becomes unspecified, so no reader checks for null.
`LCollocationExpression` and `LCollocationMeaning` do the same.
