# LTenureLanguage.cs
Hash: `3f59345c70c0f3b2`

Answers about the held draft that need an engine lookup beyond the draft itself.
Language-dependent flag queries read the held draft instead of an editable control.
An ended tenure or empty language answers false for flags.

## `public bool LTenureFlaggedCheck()`

Whether the draft's language shows its varieties as flags.
An ended tenure or an empty language answers false.

## `public LTranscriptionSheet? LTenureTranscriptionRead()`

The held draft's transcription rows outside the glyph scheme, each with the schemes its pack declares.
The engine answers the pack's schemes and the glyph split, and the clerk builds the sheet from them.
The clerk marks each scheme taken per row and names the first scheme still free.
An ended tenure holds no draft, so it answers nothing.

## `public LGlyphBlock? LTenureGlyphRead()`

The held draft's transcription rows split by its pack's glyph section, as the editor lists them.
An ended tenure holds no draft, so it answers nothing.

## `public bool LTenureReflexCheck()`

Whether the held draft shows its reflex box.
A draft already holding reflex rows shows them even when its pack lists no reflex rules.
So stored rows never vanish when a pack drops its rules.

## `public void LTenureReflexStart()`

Starts the reflex lookup for the held draft's stored entry while the draft holds no reflex row.
A draft never stored, or one already reflected, starts nothing.
The reflex clerk refuses a second lookup for the same entry itself.

## `public IReadOnlyList<LAnchorRow> LTenureAnchorScan(long reflex)`

The stored fanqie rows the reflex row `reflex` may anchor to, each marked when the row holds it.
The row's tone is compared under the draft language's tone classes.
A draft never stored, an unknown row or a refused read answers empty, so the menu shows its notice.
Any other failure of the scan reaches the caller.

## `public string LTenurePronunciationRead()`

The primary reading as the field shows it, respelled when the draft's pack respells.
An ended tenure or a draft without a reading answers empty.

## `public LAccentSheet? LTenureAccentRead()`

The live draft's accent sheet with every row after the primary, notated or not.
It is null while no draft is held.
The language clerk resolves each row's printed form.

## `public async Task<LAccentSheet?> LTenureAccentLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags the live draft's accent sheet draws into `store`, then answers a fresh sheet.
A pack that labels varieties by name answers null at once and builds no sheet.
Nothing loads while no draft is held, and the answer is null.
The sheet is read again after the load.
A language changed meanwhile answers null instead of the fresh sheet.

## `public void LTenurePronunciationSet(string text)`

Defers the typed reading as a respelling or as a phonetic one, whichever the pack shows.
The choice is the pack's, so every driver's field writes the same request.

## `public void LTenureAccentSet(long accent, string text)`

Defers the text typed on the accent row `accent` as a respelling or a phonetic reading.
The pack's respelling rule picks the form, as for the primary field, so the row's printed form matches.

## `public void LTenurePronunciationAdd(long pronunciation)`

Adds a blank pronunciation after the row `pronunciation`, or after the primary for id zero.
The reading clerk places it, so every driver adds at the same place.

## `public void LTenurePronunciationRemove(long pronunciation)`

Drops the row `pronunciation`, or the primary for id zero.

## `public Uri? LTenureAudioResolve(long pronunciation)`

The address of the row's recording, or null when the draft holds no such row.
A file gone from disk also answers null and clears the row's audio, so its play button goes.

## `public void LTenureIpaSet(string text)`

Defers the typed phonetic reading of the primary pronunciation.

## `public void LTenureRespellingSet(string text)`

Defers the typed respelling of the primary pronunciation.

## `public void LTenureVarietySet(bool primary, long pronunciation, string variety)`

Names the variety a stored reading was heard in, at once.
The primary mark targets whatever primary the draft now holds, read after its reading was written.
The engine mints the primary row late, so the id is read here rather than when the menu opened.
Any other call targets the row it names.
A missing reading or a blank variety changes nothing.

## `public IReadOnlyList<LTranslationTarget> LTenureEtymonRead()`

The etymons of the held entry, resolved to their headwords.
A refused read answers empty, so the etymology field still draws.
Any other failure reaches the caller.

## `public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LTenureTranslationRead(LEntryDraft draft)`

The link targets of every meaning and collocation of the draft, keyed by card id.
Each card answers its targets in the order the card lists them.
One engine read resolves every card's links, so a paint asks nothing per card.
A target the engine no longer finds is left out, so the chip row never shows a blank.
A refused read answers every card empty, so the cards still draw.
Any other failure reaches the caller.
Every card of the draft has an entry, so the map to the editor's cards never misses one.

## `private string? _lTenureSpeechPending;`

The part of speech the last typed text appended to the draft, or null when the text appended none.
It is draft-edit state, so the committed chips are always read off the draft itself.

## `public (IReadOnlyList<string> LSpeechNames, string LSpeechTyped, LSpeechOffer LSpeechFound) LTenureSpeechRead(string typed)`

The chips the draft holds and the typed text the caller heard, settled against the draft.
A draft changed from elsewhere answers its own parts and clears the typed text.
It also answers the catalog's offer for the settled text and chips.

## `public LSpeechOffer LTenureSpeechSet(string typed)`

Defers the draft's chips with the typed text as a pending part.
It answers the catalog's offer for the typed text, marked against the chips before the pending part.

## `public void LTenureSpeechAdd(string name)`

Declares the name in the draft language's catalog, then sends the chips with it.
The catalog comes first, so the new part of speech carries its catalog value.
A blank name sends nothing.

## `public void LTenureSpeechRemove(string name, string typed)`

Sends the chips without the named one, the pending typed text still included.

## `public LUnit LTenureUnitRead()`

The lexical unit of the displayed draft, unchosen when no draft is held.

## `public IReadOnlyList<LUnit> LTenureUnitScan()`

The units the draft's language offers, in dropdown order.

## `public void LTenureUnitSet(LUnit unit)`

Applies the chosen unit to the draft at once.
Choosing the unit already held clears it, so the dropdown both sets and unsets.

## `public void LTenureGlossAdd(long card, long sentence)`

Appends a Gloss to the sentence's list, in the settings' gloss language when it is loaded.
The engine's gloss read owns that fallback, so no language name is written in code.
It is the insert at the end place, past the list.

## `public void LTenureGlossInsert(long card, long sentence, int position)`

Places a Gloss at the position in the sentence's list, in the same gloss language as the append.
The clerk's clamp keeps the place inside the list.

## `public bool LTenureGlossPrepare(long card, long sentence)`

Appends a first Gloss only when the sentence's Example holds none, and answers whether it did.
A draft without that Example answers false.

## `private IReadOnlyList<LSpeechDraft> LTenureChipRead()`

The draft's parts without the pending one, cleaned, so every edit starts from the draft.

## `private void LTenureSpeechSend(IReadOnlyList<LSpeechDraft> held, string typed, bool deferred)`

Remembers the part of speech the typed text appends, then sends the chips with the typed text.
The request is deferred when `deferred` is true and applied at once otherwise.

## `private LSpeechOffer LTenureSpeechFind(IReadOnlyList<LSpeechDraft> held, string typed)`

Reads the draft language's catalog once and hands it to the clerk's find.
A refused catalog read offers nothing, as an empty catalog does.
Any other failure reaches the caller.

## `private LSpeechValue? LTenureSpeechCreate(string language, string name)`

Declares the name in the language's catalog and answers its value.
A catalog that refuses the name answers nothing, so the part of speech is kept as typed.
Any other failure of the catalog write reaches the caller.
