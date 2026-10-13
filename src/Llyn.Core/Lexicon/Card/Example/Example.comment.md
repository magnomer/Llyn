# Example

This folder holds the shared example sentence and its glosses, as stored and as a draft.

## `LExample`

One Example, the first independent shared entity in the lexicon.
It is owned by nothing, and any number of Entries, Meanings or Collocations reference it.
The order an Example appears in lives on each reference.
So it can sit first under one Entry and third under a Meaning.
The text is display text and never identity, so two Examples with identical text remain distinct rows.
It carries its own Glosses and references at most one Source.
A Gloss is the sentence rendered as plain text in one language, unlike the Translation a Meaning carries.
The Source reference is a pointer, not ownership.
Clearing it or deleting the Example never touches the Source.
The Gloss and Mention lists are the Example's own.
So every card quoting the sentence reads the same renderings and words.
`LExampleExcerpt` gives the Mentions a reading may link, which are none unless the text reads soundly.
A Mention's offsets point into the stated text, so an unknown or unreadable text links nothing.
Equality compares the Gloss and Mention lists row by row, because comparing by reference would make every read a change.
`LExampleNormalize` also normalizes each Gloss and sorts the Mentions by start.

## `LExampleDraft`

One Example as a draft carries it, with its id, sentence, renderings and cited Reference.
The draft never owns the sentence, because the id is the reference.
An empty id means the row has not been stored as an Example yet.
Two cards quoting one sentence carry one id, and the save writes one stored row.
The frame a card reads the sentence under is not here.
A marker and a role belong to the card holding the Example, so `LSentenceDraft` carries them.
Equality compares the lists row by row, because comparing by reference would make every load a change.
`LExampleDraftNormalize` sorts Mentions by start.
`LExampleDraftFind` asks `LMentionSpan.LMentionSpanFind`, so a sentence and a narrative match a selection alike.

## `LGloss`

One Gloss is an Example's sentence rendered as text in one language.
The list lives on the Example and never on the card that quotes it.
A Gloss links nothing.
The language may stand empty when the user has not chosen one yet.
`LGlossNamed` tells a row to show the language rather than its hint.
The list overload of `LGlossNormalize` keeps order.

## `LGlossDraft`

One Gloss as a draft carries it, with the same three facts as `LGloss`.
The id is zero for a row the database has not stored yet.
`LGlossDraftNamed` asks the gloss it resolves to, so the rule has one owner.
`LGlossDraftResolve` keeps the id, zero for a row the store has not written.

## Stamp

Commit: `c2f335c331fe`
- `LExample.cs` `a4f2e873837bd96c`
- `LExampleDraft.cs` `bd2f4955c8899713`
- `LGloss.cs` `7589b256dc19f233`
- `LGlossDraft.cs` `116b34d159dbac58`
