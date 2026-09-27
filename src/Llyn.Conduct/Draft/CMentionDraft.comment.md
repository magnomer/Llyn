# CMentionDraft.cs

## `public sealed record CMentionDraft(`

One Mention of a text, as a driver's menus and mention lines read it.
It keeps the span and the sense, so a mention line can resolve its labels.

**Parameters**

- `CMentionDraftId`: the Mention's own id within its text.
- `CMentionDraftEntry`: the entry the Mention links to, zero when unlinked.
- `CMentionDraftOffset`: where the Mention starts in the text.
- `CMentionDraftLength`: how many characters the Mention spans.
- `CMentionDraftSense`: the sense the Mention links to, zero when none.

## `public bool CMentionDraftLinked`

Whether the Mention names an entry, so the menu offers to follow or unlink it.
