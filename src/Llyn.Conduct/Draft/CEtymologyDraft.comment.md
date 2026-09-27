# CEtymologyDraft.cs

## `public sealed record CEtymologyDraft(string CEtymologyDraftText, IReadOnlyList<CMentionDraft> CEtymologyDraftMentions)`

The etymology of an entry, as the etymology field shows it.
Its etymons stay in the engine, and the editor reads them through its own gate.

**Parameters**

- `CEtymologyDraftText`: the etymology's narrative.
- `CEtymologyDraftMentions`: the entries the narrative mentions.
