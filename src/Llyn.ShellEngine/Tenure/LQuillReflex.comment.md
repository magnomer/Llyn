# LQuillReflex.cs
Hash: `829c5d2bb19cfa43`

## `public sealed class LQuillReflex`

The reflex row edits of one tenure, each building exactly one request.
A reflex row has six typed cells, so the edits get their own quill beside `LQuill`, as `LQuillSituation` does.
The row's language respells its text, which needs the phonology port.

## `private readonly LTenure _lQuillReflexTenure;`

The tenure every request is built for and handed to.

## `private readonly LPhonologyPort _lQuillReflexPhonology;`

The port whose reflex guise tells whether a row's language respells.

## `public LQuillReflex(LTenure tenure, LPhonologyPort phonology)`

Builds the quill over one tenure, which it never swaps.

## `public void LQuillReflexAdd(long reflex)`

Sends a new row at once, placed and seeded by the clerk from the pressed row `reflex`.

## `public void LQuillReflexRemove(long reflex)`

Sends the removal of row `reflex` at once.

## `public void LQuillReflexToggle(long reflex)`

Sends row `reflex` with its main mark flipped.
A row the draft no longer holds sends nothing.

## `public IReadOnlyList<LReflexDraft> LQuillLanguageSet(long reflex, string language)`

Defers the typed language of row `reflex`.
It answers the held rows with the typed language standing in, since the deferred edit is not applied yet.

## `public void LQuillKindSet(long reflex, string kind)`

Defers the typed kind of row `reflex`.

## `public void LQuillTextSet(long reflex, string text)`

Defers the typed text of row `reflex` as its respelling when its language respells.
Otherwise the text is the phonetic form.

## `public void LQuillRomanizationSet(long reflex, string romanization)`

Defers the typed romanization of row `reflex`.

## `public void LQuillMeaningSet(long reflex, string meaning)`

Defers the typed meaning of row `reflex`.

## `public void LQuillNoteSet(long reflex, string note)`

Defers the typed note of row `reflex`.

## `private bool LQuillRespellingCheck(long reflex)`

Whether row `reflex` prints its respelling, by the same guise the row scan reads.
A row the draft no longer holds reads false.
