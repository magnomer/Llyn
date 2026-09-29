# LQuill.cs

## `public sealed class LQuill`

The typed text edits of one tenure, each building exactly one request.
A driver hands it raw values, and it chooses whether the request is sent or deferred.
Which member runs for a field is the caller's decision, not the quill's.
A card's chip rows went to `LQuillChip` in job39, the tag members with them, so this type stays small.

## `private readonly LTenure _lQuillTenure;`

The tenure every request is built for and handed to.

## `public LQuill(LTenure tenure)`

Builds the quill over one tenure, which it never swaps.

## `public void LQuillAuthorSet(string name)`

Defers the Author's typed name.

## `public void LQuillExampleSet(string text)`

Defers the held Example's typed text, always as known.
Typing is what makes an unknown text known.

## `public void LQuillSpeakerSet(string language)`

Sends the held Example's chosen language at once, since it came from a click.

## `public void LQuillReferenceSet(long reference)`

Points the held Example at the picked Source, sent at once.

## `public void LQuillAnchorSet(long reflex, long fanqie, bool anchored)`

Ties or unties one reflex row and one fanqie row, sent at once.

## `public void LQuillGlossRemove(long card, long sentence, long gloss)`

Drops one Gloss, sent at once.

## `public void LQuillGlossSet(long card, long sentence, long gloss, string? language, string? text)`

Sends a chosen language at once when one is given.
Otherwise it defers the typed text, which must then be given.

## `public void LQuillMentionRemove(long card, long sentence, long mention)`

Drops one Mention, sent at once.

## `public void LQuillSentenceAdd(long card, int position)`

Adds an empty sentence row to a card at the given position, sent at once.

## `public void LQuillSentenceRemove(long card, long sentence)`

Drops one sentence row from a card, sent at once.

## `public void LQuillSentenceSet(long card, long sentence, string text)`

Writes a sentence's text as known, deferred so typing folds into one change.

## `public void LQuillParticleSet(long card, long sentence, string text)`

Writes a sentence's particle as known, deferred like the text.

## `public void LQuillDependenceSet(long card, long sentence, string text)`

Writes a sentence's dependence as known, deferred like the text.

## `public void LQuillTitleSet(long card, string text)`

Defers a card's typed title, so each keystroke joins one undo step.

## `public void LQuillExpressionSet(long card, string text)`

Defers a card's typed expression, like its title.

## `public void LQuillMeaningSet(long card, string text)`

Defers a card's typed definition, like its title.

## `public void LQuillTitleSet(string text)`

Defers the held Source's typed title.

## `public void LQuillYearSet(string text)`

Defers the held Source's typed year.

## `public void LQuillUrlSet(string text)`

Defers the held Source's typed address.

## `public void LQuillNoteSet(string text)`

Defers the held Source's typed note.

## `public void LQuillKindSet(string? tag)`

Sends the kind a menu tag names at once, since it came from a click.
The reference clerk decides the kind, and a tag the Source already has sends nothing.

## `public bool LQuillAuthorAdd(string? name, int position, long former)`

Sends the credit of a typed name at `position`, in place of the credit `former` stood for.
The draft clerk trims the name and resolves it to an Author.
A blank name sends nothing and answers false, as `LAuthorNameCheck` rules.

## `public bool LQuillAuthorInsert(long author, int position, long former)`

Sends the credit of a stored Author at `position`, in place of the credit `former` stood for.
Picking the Author `former` already names sends nothing and answers false, as `LDraftFormerCheck` rules.

## `public void LQuillAuthorRemove(long author)`

Sends the removal of one credit.

## `public void LQuillAuthorMove(long author, int position)`

Sends one credit to a new place, which the draft clerk clamps to the list.

## `public void LQuillEtymologySet(string text)`

Defers the held entry's typed etymology narrative.

## `public void LQuillEtymonAdd(long entry, int position)`

Links a source entry at the given position, sent at once.

## `public void LQuillEtymonRemove(long entry)`

Drops one source link, sent at once.

## `public void LQuillMentionSave(int offset, int length, long entry)`

Links a span of the narrative to an entry, sent at once.
Entry zero drops the span over that whole length.

## `public void LQuillCitationSet(long card, long sentence, long reference)`

Points a card sentence's citation at a Source, sent at once.
