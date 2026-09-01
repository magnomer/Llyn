# LCollocation.cs

## `public sealed record LCollocation(`

One Collocation owned by an entry: a stable-id node mirroring the Meaning construction, carrying an `LCollocationExpression` — the phrase itself — beside the `LCollocationMeaning` that explains it, which is the definition analogue of a Meaning. The card has a field for each, so a collocation keeps both, beside the `LCollocationTitle` the card is headed with. `LCollocationId` is the identity — an opaque, program-generated stable id — and is the base the job08/job09 associations target. Reordering collocation cards rewrites `LCollocationPosition` only; the id never changes. A collocation's synonym is not text held here: it is an interlink, modelled by `LCollocationSynonym`.

**Parameters**

- `LCollocationId` — Opaque, program-generated stable id.
- `LCollocationEntryId` — Owning entry id.
- `LCollocationPosition` — Order among the entry's collocations.
- `LCollocationTitle` — Title typed on the Collocation card; `null` when none was typed.
- `LCollocationExpression` — The collocation expression text; empty when unset.
- `LCollocationMeaning` — What the expression means; empty when unset.
