# Example

The edit requests for a sentence and what hangs on it: its renderings, its linked words and its card links.
They cover the example panel, the sentence rows of a card, glosses, mentions and translations.
They stand apart from the entry requests, which edit the entry's own fields and cards.
A card is named by its kind and id, since a Meaning and a Collocation may carry the same id.
Each list request names its row by id.
So a change lands on that row wherever the list has moved it.

## `LExampleSite.cs`

A site names the one Example a gloss or mention request edits.
The card site holds a card kind, a card id and a sentence id.
The draft site names the Example the draft holds on its own, the one the corpus panel shows.
Each site gives a key for the request key, so a deferred edit replaces only its own site's row.
Each site reads its Example draft from a held draft, or null when the draft has none there.
So one set of records serves the card editor and the corpus panel alike.

## `LRequestExample.cs`

These are the requests of the example panel, one per field of the sentence a draft holds on its own.
They name only the draft, because a draft holds one such sentence.
A sentence inside a card is edited through the sentence requests instead.
The reference request names the cited source, or zero to cite nothing.

## `LRequestGloss.cs`

These requests are one per gesture a shown sentence offers on its renderings.
Every one names its Example by site, with card and sentence ids real or minted.
The text and language requests key by site and gloss id.

### `LRequestGlossAddition`

The position is clamped into the list.
The engine mints the negative id, so the form never carries one of its own.

### `LRequestGlossRemoval`

A Gloss no row carries is refused.

## `LRequestMention.cs`

These requests are one per gesture a shown sentence offers on one of its words.
They name the Example by site, as the gloss requests do.
So one set of three records serves both editors.

### `LRequestMentionAddition`

The word is given by offset and length in code points.
Entry id 0 marks the word as standing for nothing, and sense id 0 chooses no Meaning.
A span overlapping an existing Mention replaces it.
So the form has one gesture and the engine decides whether it is new or a change.
A span past the end of the text is refused.
So is an Entry no row carries, or a Meaning of another Entry.

### `LRequestMentionSense`

Sense id 0 clears the choice.
A Mention standing for nothing has no Entry to narrow, so the request is refused on it.

## `LRequestSentence.cs`

These requests are one per structural change and one per field of a sentence row.
Every one names the card by kind and id, and the sentence by id, real or minted.
The addition alone names no sentence, because it has none yet.
The example a sentence shows is edited through the sentence request.
The form never sees an example apart from its row.
The field requests key by kind, card and sentence, so a deferred edit replaces only its own row's field.

### `LRequestSentenceAddition`

The position is clamped to the list.
The engine mints the row's id and gives it no example.
The example appears with the first text that arrives for the row.

### `LRequestSentenceExample`

It picks an existing example for the sentence.
The engine copies the row's content under its positive id, replacing whatever example the sentence had.

### `LRequestSentenceText`

On a sentence with no example the first non-empty text mints one.
On an example with a positive id it edits the draft copy, and commit updates the row.

### `LRequestSentenceReference`

A zero source cites nothing.
On a sentence with no example a non-zero source mints one.

## `LRequestTranslation.cs`

A translation links only to an existing entry or to a draft the court opened, so there is no addition.
The id is the entry's, or the target draft's until that draft commits and the court settles it.
Pick leaves an entry the card already links as it is.
Removal leaves a court link behind it for the form to close.

## Stamp

Commit: `8c403d1df768`
- `LExampleSite.cs` `9ce446374601be09`
- `LRequestExample.cs` `e9de61e4d141ceb2`
- `LRequestGloss.cs` `c6a9d59b087f8f14`
- `LRequestMention.cs` `7df8e50cf5d2bf0e`
- `LRequestSentence.cs` `b503eb33f72afb7a`
- `LRequestTranslation.cs` `4b1accd7270a4e38`
