# PMentionMark.cs

## `internal sealed record PMentionMark(`

One Mention a driver hands a mention text, in Deportment's own type.
A driver feeds its controls no Conduct shape, so it copies each stored Mention into one of these.
The mention text turns them back into the shape its window divides by.

**Parameters**

- `PMentionMarkId`: the Mention's own id within its text.
- `PMentionMarkOffset`: where the Mention starts in the text.
- `PMentionMarkLength`: how many characters the Mention spans.
- `PMentionMarkEntry`: the linked Entry, zero for a silent Mention.
- `PMentionMarkSense`: the chosen sense, zero when none is chosen.
