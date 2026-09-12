# PMentionConverter.cs

## `internal sealed class PMentionConverter : IValueConverter`

Maps the Mention drafts of a shown card to the stored shape the sentence control reads.
The display binds a draft, and the control speaks the store's record so the corpus panel can hand it one directly.
Every draft resolves through `LMentionDraftResolve`, which is the converter's only logic.
A binding that finds no list, as on a sentence with no Example, yields an empty list.
