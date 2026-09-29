# CExampleDraft.cs

## `public sealed record CExampleDraft(`

The example a sentence holds, as the sentence row shows it.

**Parameters**

- `CExampleDraftText`: the example's text.
- `CExampleDraftReference`: the cited reference, or null when none is cited.
- `CExampleDraftGloss`: the example's glosses.

The chips of the entries the text mentions come from `CSentence.CSentenceMentionRead`, not from this record.
