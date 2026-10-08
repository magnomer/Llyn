# LQuillSpeech.cs
Hash: `1dd744211b040b7c`

## `public sealed class LQuillSpeech`

The parts of speech of the held entry while the user types them into the chip field.
It keeps the part the last typed text appended, so one quill lives as long as its tenure.
The desk builds it when a tenure starts and drops it when the tenure ends.

## `private readonly LTenure _lQuillSpeechTenure;`

The tenure every request is built for and handed to, and whose engine holds the catalog.

## `private string? _lQuillSpeechPending;`

The part of speech the last typed text appended to the draft, or null when the text appended none.
It is draft-edit state, so the committed chips are always read off the draft itself.

## `public LQuillSpeech(LTenure tenure)`

Builds the quill over one tenure, which it never swaps.

## `public (IReadOnlyList<string> LSpeechNames, string LSpeechTyped, LSpeechOffer LSpeechFound) LQuillSpeechRead(string typed)`

The chips the draft holds and the typed text the caller heard, settled against the draft.
A draft changed from elsewhere answers its own parts and clears the typed text.
It also answers the catalog's offer for the settled text and chips.

## `public LSpeechOffer LQuillSpeechSet(string typed)`

Defers the draft's chips with the typed text as a pending part.
It answers the catalog's offer for the typed text, marked against the chips before the pending part.

## `public void LQuillSpeechAdd(string name)`

Declares the name in the draft language's catalog, then sends the chips with it.
The catalog comes first, so the new part of speech carries its catalog value.
A name the draft already holds declares nothing.
A blank name sends nothing.

## `public void LQuillSpeechRemove(string name, string typed)`

Sends the chips without the named one, the pending typed text still included.

## `private IReadOnlyList<LSpeechDraft> LQuillSpeechScan()`

The draft's parts without the pending one, cleaned, so every edit starts from the draft.

## `private void LQuillSpeechSend(IReadOnlyList<LSpeechDraft> held, string typed, bool deferred)`

Remembers the part of speech the typed text appends, then sends the chips with the typed text.
The request is deferred when `deferred` is true and applied at once otherwise.

## `private LSpeechOffer LQuillSpeechFind(IReadOnlyList<LSpeechDraft> held, string typed)`

Reads the draft language's catalog once and hands it to the clerk's find.
A refused catalog read offers nothing, as an empty catalog does.
Any other failure reaches the caller.

## `private LSpeechValue? LQuillSpeechCreate(string language, string name)`

Declares the name in the language's catalog and answers its value.
A catalog that refuses the name answers nothing, so the part of speech is kept as typed.
Any other failure of the catalog write reaches the caller.
