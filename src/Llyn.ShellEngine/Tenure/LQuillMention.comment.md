# LQuillMention.cs
Hash: `6deb5fd9b4c21639`

## `public sealed class LQuillMention`

The removal of one Mention by id, building exactly one request, and the finds of a Mention by its span.
A card sentence and a held Example's text both drop their chips and ask their menus through it.
The unlink that names a Mention by its span lives in `LQuillChip` instead, which finds the Mention here.

## `private readonly LTenure _lQuillMentionTenure;`

The tenure every request is built for and handed to.

## `public LQuillMention(LTenure tenure)`

Builds the edit over one tenure, which it never swaps.

## `public void LQuillMentionRemove(long card, long sentence, long mention)`

Drops one Mention, sent at once.

## `public LMentionDraft? LQuillMentionFind(long cardId, long sentenceId, LMentionDraft span)`

The Mention the span lies inside, in the Example one sentence field holds, or none.
It reads the draft through the tenure herald's `LTenureKeptRead`, so a menu asking many times sends nothing.

## `public bool LQuillMentionCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether a field's selection lies inside any Mention of that sentence, so the unlink command may run.
The selection arrives in UTF-16 units and is measured in code points here.

## `public bool LQuillSenseCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether the selection lies inside a Mention that stands for an Entry, so a sense may be chosen.

## `public long? LQuillSenseRead(long cardId, long sentenceId, string text, int start, int length)`

The Entry whose Meanings the sense menu offers, or none when the Mention links nothing.
Pending typing is persisted first, so the Mention is found against the text the field shows.
