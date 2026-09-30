# CCandidate.cs

## `public sealed record CCandidate(`

One reading a search found, as the notation popup lists it.

**Parameters**

- `CCandidateSource`: the name of the source that answered.
- `CCandidatePhonetic`: the reading the source gave, or null when it had none.
- `CCandidateOrder`: the source's place in the pack, so rows keep the pack's order.
- `CCandidateReached`: whether the source answered at all, which picks the notice for a missing reading.
- `CCandidateVariety`: the variety the reading speaks, empty when the source names none.
- `CCandidateRespelling`: the reading respelled for the language, or null when no respelling applies.

## `public bool CCandidateRegional`

Whether the reading names a variety, so the row shows its flag.

## `public bool CCandidateNotated`

Whether the source gave a reading, so the row lists it instead of a notice.
