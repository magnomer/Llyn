# LTenureLanguage.cs

Answers about the held draft that need an engine lookup beyond the draft itself.
Language-dependent flag and variety queries read the held draft instead of an editable control.
An ended tenure or empty language answers false for flags and an empty variety list.

## `public IReadOnlyList<LSchemeRow> LTenureSchemeRead(long transcription)`

The schemes the draft's pack declares, each marked when a row other than `transcription` holds it.
The mark is the clerk's one-scheme rule, so the dropdown greys out what a pick would refuse.
An ended tenure has no language, so it answers no schemes.

## `public string LTenurePronunciationRead()`

The primary reading as the field shows it, respelled when the draft's pack respells.
An ended tenure or a draft without a reading answers empty.

## `public void LTenurePronunciationSet(string text)`

Defers the typed reading as a respelling or as a phonetic one, whichever the pack shows.
The choice is the pack's, so every driver's field writes the same request.

## `public void LTenureIpaSet(string text)`

Defers the typed phonetic reading of the primary pronunciation.

## `public void LTenureRespellingSet(string text)`

Defers the typed respelling of the primary pronunciation.

## `public void LTenureVarietySet(long pronunciation, string variety)`

Names the variety a stored reading was heard in, at once.
A missing reading or a blank variety changes nothing.

## `public IReadOnlyList<LTranslationTarget> LTenureEtymonRead()`

The etymons of the held entry, resolved to their headwords.
A refused read answers empty, so the etymology field still draws.

## `public IReadOnlyList<LTranslationTarget> LTenureTranslationRead(long card)`

The link targets of one held card, in the order the card lists them.
A target the engine no longer finds is left out, so the chip row never shows a blank.
A refused read answers empty, so the card still draws.

## `private string? _lTenureSpeechPending;`

The part of speech the last typed text appended to the draft, or null when the text appended none.
It is draft-edit state, so the committed chips are always read off the draft itself.

## `public (IReadOnlyList<string> LSpeechNames, string LSpeechTyped) LTenureSpeechRead(string typed)`

The chips the draft holds and the typed text the caller heard, settled against the draft.
A draft changed from elsewhere answers its own parts and clears the typed text.

## `public bool LTenureSpeechSet(string typed)`

Defers the draft's chips with the typed text as a pending part.
It answers whether the typed text names a part of speech at all.

## `public void LTenureSpeechAdd(string name)`

Declares the name in the draft language's catalog, then sends the chips with it.
The catalog comes first, so the new part of speech carries its catalog value.
A blank name sends nothing.

## `public void LTenureSpeechRemove(string name, string typed)`

Sends the chips without the named one, the pending typed text still included.

## `private IReadOnlyList<LSpeechDraft> LTenureChipRead()`

The draft's parts without the pending one, cleaned, so every edit starts from the draft.

## `private LSpeechValue? LTenureSpeechCreate(string language, string name)`

A catalog that refuses the name answers nothing, so the part of speech is kept as typed.
