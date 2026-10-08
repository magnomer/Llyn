# LQuillEtymology.cs
Hash: `d06d61da2598794e`

## `public sealed class LQuillEtymology`

The edits of the held entry's etymology, each building exactly one request.
The narrative, its source links and its spans all change here.
Its two reads resolve the source links and find a narrative span for the etymology gates.

## `private readonly LTenure _lQuillEtymologyTenure;`

The tenure every request is built for and handed to.

## `public LQuillEtymology(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LQuillEtymologySet(string text)`

Defers the held entry's typed etymology narrative.

## `public void LEtymonAdd(long entry, int position)`

Links a source entry at the given position, sent at once.

## `public void LEtymonRemove(long entry)`

Drops one source link, sent at once.

## `public void LEtymologyMentionSave(int offset, int length, long entry)`

Links a span of the narrative to an entry, sent at once.
Entry zero drops the span over that whole length.

## `public IReadOnlyList<LTranslationTarget> LQuillEtymonRead()`

The etymons of the held entry, resolved to their headwords.
A refused read answers empty, so the etymology field still draws.
Any other failure reaches the caller.

## `public LMentionDraft? LQuillEtymologyFind(LMentionDraft span)`

The span of the live draft's etymology that the given span lies inside.
It reads the live draft, since the etymology gates act on what the user sees now.
