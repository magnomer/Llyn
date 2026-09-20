# LMention.cs

## `public sealed record LMention(`

One word inside an Example that stands for an Entry.
It holds the span of the sentence text the word occupies and the Entry it points at.
The Entry may be narrowed to one Meaning of it.
The word may instead be marked as standing for nothing, such as a proper name or a typo.
A Mention lives on the Example, never on the card that quotes it.
So every card citing one sentence reads the same words the same way.
`LMentionOffset` and `LMentionLength` count Unicode scalar values, not UTF-16 units.
A surrogate pair is one character on both sides of the store boundary.
A Mention has no state of its own.
A row exists or it does not, which is the state convention for a link.

**Parameters**

- `LMentionId` — Opaque, program-generated stable id, zero for a row the store has not written.
- `LMentionOffset` — The code-point offset the word begins at, counted from zero.
- `LMentionLength` — The number of code points the word occupies, always above zero.
- `LMentionEntryId` — The Entry the word stands for, or zero when it is marked as standing for nothing.
- `LMentionSenseId` — The one Meaning the word is narrowed to, or zero when no Meaning is chosen.
  A Meaning is never given without its Entry.

## `public static bool LMentionOverlapCheck(IReadOnlyList<LMention> mentions)`

Answers whether any two spans in `mentions` share a character.
The archive refuses such a list, and the engine checks it before asking.
Both share this one rule, so it lives here.

## `public static IReadOnlyList<LMention> LMentionSort(IReadOnlyList<LMention> mentions)`

The same Mentions ordered by where they start.
Every store reads them in this order, and every normalize writes them in it.
