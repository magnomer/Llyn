# LTenureLanguage.cs

Answers about the held draft that need an engine lookup beyond the draft itself.
Language-dependent flag and variety queries read the held draft instead of an editable control.
An ended tenure or empty language answers false for flags and an empty variety list.

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

## `public IReadOnlyDictionary<long, LTranslationTarget> LTenureTargetRead()`

The link targets of the held draft, keyed by entry, for the cards' translation chips.
A refused read answers empty, so the cards still draw.
