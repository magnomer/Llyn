# Sentence

Sentence holds how one card holds an Example, and the translation links a card carries.
The Example is shared and owned by nothing, so facts that belong to one owner live here instead.

## `LSentence`

One Example as a single Meaning or Collocation holds it.
Two owners citing the same Example each keep their own Sentence over it, and editing one never reaches the other.
A Sentence carries the frame the owner reads the Example under.
`LSentenceParticle` is the grammatical marker the frame uses, and `LSentenceDependence` the role it fills.
Neither is ever shipped with a value.
Nothing in the program or a language pack names a marker or a role.
A Sentence may state a frame and no Example at all.
The frame belongs to the owner, so a marker written before any sentence is not the Example's to lose.
A Meaning and a Collocation hold an Example on identical terms.
The row does not say which kind owns it, because the store it was read from already does.

## `LSentenceDraft`

One card's hold on one Example, as a draft carries it.
The card holds these rows and never holds Examples directly.
Two cards quoting one sentence are two rows naming one Example.
Editing the frame on one never reaches the other.
A marker and a role are written by hand, because nothing ships either of them.
A row may state a frame and no Example at all.
The store keeps such a row, and it must survive a load and a save unchanged.
A row stating neither an Example nor a frame records nothing.
So the commit skips it rather than write a row the store would refuse.
The engine still holds such a row in the draft.
The form adds a row blank and fills it afterwards.
The order of the list holding the rows is their order, and the save rewrites every stored position from it.
The row carries the id of the stored row it edits.
An empty id means it has not been stored yet.
A save that kept the frame but issued a fresh id would rewrite a row nothing asked it to touch.
`LSentenceDraftLanguage`, `LSentenceDraftGloss` and `LSentenceDraftMention` are empty when the row quotes no Example.
A frame without a sentence has no words to link, so a reader never checks the Example itself.
`LSentenceDraftEmpty` is true when the row names no Example and states no frame.
A row whose Example has no sentence and no stored id names no Example.
A minted id is not a stored one, so an Example the engine named but nobody wrote is still nothing.
An Example carrying a stored id names a stored row, however little the sentence says.
`LSentenceDraftCited` is true when the quoted Example cites a stored Source, and a row without an Example cites nothing.

## `LSentenceOrder`

The one fact a language pack states about a Sentence's frame.
It says which of the two written fields the form writes first, and nothing about what either may hold.
Each field is stated as the place it is written in, counted from zero.
English writes its markers before what they mark, so the marker stands at nothing.
A language whose markers follow the word they attach to writes the role there instead.
`LSentenceOrderDefault` writes the marker first when a pack states nothing.
That is a layout choice and not a claim about the language.

## `LTranslation`

One translation link a Meaning or Collocation carries to another Entry.
The link stores the target Entry's id and never its text, so renaming a headword changes nothing about the link.
Links are held in order on the card that carries them.

## `LTranslationOffer`

What a translation field keeps after a gate, and the Entries its word may link to.
The engine answers it whole, so the field shows it without deciding anything.

## `LTranslationTarget`

The resolved Entry an `LTranslation` points at, with the headword and language a shown link needs.
The record is read back from the target Entry and never stored beside the link.

## Stamp

Commit: `c2f335c331fe`
- `LSentence.cs` `d36b8b375f438d75`
- `LSentenceDraft.cs` `506a1ef90d2ee42f`
- `LSentenceOrder.cs` `d1834db7de7938f4`
- `LTranslation.cs` `d6e1755ff6d48650`
- `LTranslationOffer.cs` `948a55995d01fb21`
- `LTranslationTarget.cs` `4f49dca1ad725101`
