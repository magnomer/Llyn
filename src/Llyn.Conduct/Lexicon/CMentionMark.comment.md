# CMentionMark.cs

## `public sealed record CMentionMark(`

One stored Mention of a text, as a mention text holds it.
Drivers feed their controls this record directly, with no copy of their own.

**Parameters**

- `CMentionMarkId`: the Mention's own id within its text.
- `CMentionMarkOffset`: where the Mention starts in the text.
- `CMentionMarkLength`: how many characters the Mention spans.
- `CMentionMarkEntry`: the entry the Mention links to, zero when unlinked.
- `CMentionMarkSense`: the sense the Mention links to, zero when none.
