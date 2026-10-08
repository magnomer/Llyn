# LQuillReflex.cs
Hash: `a9fff8a4c16834c1`

## `public sealed class LQuillReflex`

The reflex row edits of one tenure, each building exactly one request, and the reflex reads they need.
A reflex row has six typed cells and its fanqie anchors, so its edits get their own quill.
The row's language respells its text, which needs the reflex port.

## `private readonly LTenure _lQuillReflexTenure;`

The tenure every request is built for and handed to.

## `private readonly LReflexPort _lQuillReflexPort;`

The port whose reflex guise tells whether a row's language respells.

## `public LQuillReflex(LTenure tenure, LReflexPort reflexes)`

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

## `public void LReflexAnchorSet(long reflex, long fanqie, bool anchored)`

Ties or unties one reflex row and one fanqie row, sent at once.

## `public bool LQuillReflexCheck()`

Whether the held draft shows its reflex box.
A draft already holding reflex rows shows them even when its pack lists no reflex rules.
So stored rows never vanish when a pack drops its rules.

## `public void LQuillReflexStart()`

Starts the reflex lookup for the held draft's stored entry while the draft holds no reflex row.
A draft never stored, or one already reflected, starts nothing.
The reflex clerk refuses a second lookup for the same entry itself.

## `public IReadOnlyList<LAnchorRow> LQuillAnchorScan(long reflex)`

The stored fanqie rows the reflex row `reflex` may anchor to, each marked when the row holds it.
The row's tone is compared under the draft language's tone classes.
A draft never stored, an unknown row or a refused read answers empty, so the menu shows its notice.
Any other failure of the scan reaches the caller.

## `private bool LQuillRespellingCheck(long reflex)`

Whether row `reflex` prints its respelling, by the guise the reflex port answers for its language.
A row the draft no longer holds reads false.
