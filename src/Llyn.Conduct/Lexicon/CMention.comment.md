# CMention.cs

## `public sealed record CMention(`

One stored Mention of a text, as a mention text holds it.

**Parameters**

- `CMentionId`: the Mention's own id within its text.
- `CMentionOffset`: where the Mention starts in the text.
- `CMentionLength`: how many characters the Mention spans.
- `CMentionEntry`: the entry the Mention links to, zero when unlinked.
- `CMentionSense`: the sense the Mention links to, zero when none.
