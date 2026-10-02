# CEtymologyDraft.cs
Hash: `5f58fb76c9a452bd`

## `public sealed record CEtymologyDraft(string CEtymologyDraftText);`

The etymology of an entry, as the etymology field shows it.
Its etymons stay in the engine, and the editor reads them through its own gate.
Its Mentions arrive as ready chips through `CCard.CCardEtymologyRead`.

**Parameters**

- `CEtymologyDraftText`: the etymology's narrative.
