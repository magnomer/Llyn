# LQuillSentence.cs
Hash: `0015d1ed21b5f7f1`

## `public sealed class LQuillSentence`

The edits of a card's sentence rows and their glosses, each building exactly one request.
A held Example's glosses use the same members with card and sentence zero.
A new Gloss takes the settings' gloss language, which the tenure's engine reads.

## `private readonly LTenure _lQuillSentenceTenure;`

The tenure every request is built for and handed to.

## `public LQuillSentence(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LSentenceGlossRemove(long card, long sentence, long gloss)`

Drops one Gloss, sent at once.

## `public void LSentenceGlossSet(long card, long sentence, long gloss, string? language, string? text)`

Sends a chosen language at once when one is given.
Otherwise it defers the typed text, which must then be given.

## `public void LSentenceCitationSet(long card, long sentence, long reference)`

Points a card sentence's citation at a Source, sent at once.

## `public void LQuillSentenceAdd(long card, int position)`

Adds an empty sentence row to a card at the given position, sent at once.

## `public void LQuillSentenceRemove(long card, long sentence)`

Drops one sentence row from a card, sent at once.

## `public void LQuillSentenceSet(long card, long sentence, string text)`

Writes a sentence's text as known, deferred so typing folds into one change.

## `public void LSentenceParticleSet(long card, long sentence, string text)`

Writes a sentence's particle as known, deferred like the text.

## `public void LSentenceDependenceSet(long card, long sentence, string text)`

Writes a sentence's dependence as known, deferred like the text.

## `public void LQuillGlossAdd(long card, long sentence)`

Appends a Gloss to the sentence's list, in the settings' gloss language when it is loaded.
The engine's gloss read owns that fallback, so no language name is written in code.
It is the insert at the end place, past the list.

## `public void LQuillGlossInsert(long card, long sentence, int position)`

Places a Gloss at the position in the sentence's list, in the same gloss language as the append.
The clerk's clamp keeps the place inside the list.

## `public bool LQuillGlossPrepare(long card, long sentence)`

Appends a first Gloss only when the sentence's Example holds none, and answers whether it did.
A draft without that Example answers false.
