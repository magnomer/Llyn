# CProspect.cs
Hash: `ff7d458631523cb8`

## `public sealed record CProspect(`

What a translation field keeps after a gate, and the dropdown of Entries it offers, ready to show.

**Parameters**

- `CProspectText`: the text the entry keeps once the gate has linked what it could.
- `CProspectWord`: the word the dropdown offers a fresh Entry for in each of `CProspectLanguages`.
- `CProspectRows`: the stored Entries the word matches, whole headwords first.
- `CProspectShown`: the engine's verdict that the dropdown opens.
- `CProspectChosen`: the engine's verdict that its first row stands chosen, since one Entry answers the word.
- `CProspectLanguages`: the languages a fresh Entry is offered in, one create row each, the draft's own last.
