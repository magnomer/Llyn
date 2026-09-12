# LEngineRequestMention.cs

## `public sealed partial class LEngine`

The Mentions of the Example a draft holds, on a card row or on the corpus panel.
Every handler goes through one locate routine, so the two panels differ only in where the Example lives.
The text handlers of both panels call the shift routine here, so a Mention follows the word it marks.

## `private LDraft LEngineMentionAdd(LDraft draft, LRequestMentionAddition request)`

Links one span to an Entry, replacing every Mention the span overlaps.
A span of no length or one past the end of the text is refused.
So is an Entry no row carries, or a Meaning of another Entry.
The new Mention gets a minted id and the list is sorted by offset.

## `private static LDraft LEngineMentionRemove(LDraft draft, LRequestMentionRemoval request)`

Drops the Mention named, and refuses when the Example holds none by that id.

## `private LDraft LEngineMentionChange(LDraft draft, LRequestMentionSense request)`

Narrows the Mention named to a Meaning of its own Entry, or clears the choice with 0.
A Mention standing for nothing is refused, because there is no Entry to narrow.

## `private void LEngineMentionValidate(long entryId, long senseId)`

Refuses an Entry no row carries and a Meaning that does not belong to that Entry.
An Entry of 0 stands for nothing and may carry no Meaning.
A minted negative id is refused too, because a Mention points only at stored rows.

## `private static LDraft LEngineMentionApply(`

Locates the Example on the card row named, or on the draft panel when card and sentence are both 0.
The panel holds an `LExample`, so its list is converted to drafts for the change and back afterwards.
A row quoting no Example has no words to link and is refused.

## `private static LExampleDraft LEngineMentionUpdate(LExampleDraft example, LStateValue text)`

Replaces the text of a row's Example and shifts its Mentions to follow.

## `private static LExample LEngineMentionUpdate(LExample example, LStateValue text)`

The same for the Example the corpus panel holds.

## `private static IReadOnlyList<LMentionDraft> LEngineMentionUpdate(`

Recomputes every span after the text changed from `before` to `after`.
The common prefix and the common suffix of the two texts are found in code points.
A Mention entirely inside the prefix keeps its offset.
One entirely inside the suffix shifts by the length delta.
One touching the edited middle is dropped, because the word it marked is no longer what it was.
This is right for every single-keystroke edit.
A paste over the whole sentence drops everything, which is what the user expects.

## `private static IReadOnlyList<LMentionDraft> LEngineMentionRead(IReadOnlyList<LMention> mentions)`

The stored Mentions as drafts, ids kept.

## `private static IReadOnlyList<LMention> LEngineMentionRead(IReadOnlyList<LMentionDraft> drafts)`

The drafts as Mentions, ids kept, negative for rows not yet written.

## `private static List<Rune> LEngineRuneRead(string text)`

The text as a list of code points, so offsets count what the store counts.
