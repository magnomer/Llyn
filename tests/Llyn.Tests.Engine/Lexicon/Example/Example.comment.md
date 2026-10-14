# Example

This folder holds the tests of example sentences and what a card holds in them.
They cover examples, lines, sentences and their order, and mentions with their spans.

## `TExample`

Covers Examples as the entry editor quotes them.
An Example is created with its renditions and quoted from both card sides by a sentence citing it.
What a card quotes is read back through the entry's load, as the editor reads it.

The whole-table read fills every Example's Glosses and Mentions in one statement each.
Past 32 766 Examples that statement can no longer bind one parameter per id.
So the ids travel as one JSON value, and a corpus that deep must still open.

An Example quoted by a Meaning card and a Collocation card reads back as the same Example from each side.
A gloss given at creation is stored with its Example.

Editing a quoted Example through its card keeps the Example id and the Reference it points at.
Cards that hold the id would otherwise lose their quote.
Reordering or adding sentences must not mint new rows for the others.

An Example two cards quote is pool data, and a card edits only what it owns.
Editing the sentence through one card gives that card a new Example row carrying the new text.
The other card keeps the row it quoted, unchanged.
A card quoting an Example alone still edits it in place, since no one else can see the change.

The browsing panel reads every Example with its renditions, including one nothing quotes.
The usage counts answer for every Example at once, and an unquoted one is absent rather than zero.
The itemized usage says which side quotes, Meaning or Collocation.
The plain delete is refused while anything quotes the Example.
The detaching delete drops every reference and the row in one operation.

### `TExampleCiteCreate`

Builds a sentence citing a stored Example by id, with the text and source it already holds.
Anything else would edit the Example rather than quote it.

## `TExampleLine`

Covers the three texts a reading card shows for one sentence row.
The engine composes them by the portrait's rule, so the display keeps no copy of it.
The frame follows the language's order, and the sentence stands apart from it.
An unknown marker reads the mark inside the brackets.
A row without an Example has no sentence and no Source line.
A frame with one part keeps no stray space, and a row with no frame has no brackets.
A cited Source answers its ready line, and a missing line or no citation answers nothing.
The sentence arrives in pieces around its Mentions, the draft's span resolved to stored shape.

## `TMention`

Mentions persist ordered text spans linked to Entries and optional meanings.
Overlaps, out-of-range spans, and meanings without Entries are rejected.
Deleting linked records or shortening text preserves only valid data.
Sentence reads restore mention data for each embedded Example.

Creating an Example sorts mentions by text offset and assigns ids.
Archive reads preserve spans and links, including optional sense identity.
Overlapping spans are rejected without leaving partial mention rows behind.
A span extending beyond Example text is refused, while the same start with an in-bounds length can be saved.
A sense reference without its required Entry violates the database check.
An Entry-less mention without a sense remains valid.

Deleting an Entry removes mentions that point to it but preserves the containing Example.
Deleting a referenced meaning clears only the sense link, retaining the mention id and its Entry link.
When Example text shrinks, mentions extending beyond its end are removed and earlier spans remain.
Meaning-based sentence reads include mention data in each Example row, preserve non-Example rows, and retain sense links.
Resolving a draft preserves its temporary negative id, span, and Entry and meaning links for unsaved mentions.

## `TMentionFinding`

Covers the engine's answer for a clicked word.
A stored Mention wins over every candidate, even one standing for nothing.
Two homograph Entries of the language both come back, in headword order, and another language's Entry does not.
Three homographs come back the same way, which is the order the word menu lists them in.
An inflected form with no headword answers its span and no candidate.
Japanese text picks the longest headword covering the click over the letter run.
It falls back to the run where nothing matches.

## `TMentionSpan`

Span operations use code-point offsets and respect word and script boundaries.
Division preserves gaps and avoids empty pieces around mentions.
Selection normalization trims outer spaces.
Example lookup accepts only selections contained by one mention.
A draft reads a sentence example by its card and sentence ids.

A caret inside an English word or contraction resolves to the full word span.
A caret on a separator or trailing punctuation boundary produces an empty span rather than selecting adjacent text.
A caret at or beyond the end of text has no selected word.
A supplementary Unicode character contributes one offset unit when a word span is resolved.
Japanese runs stop when the script changes, and punctuation positions resolve to empty spans.

Out-of-order mentions divide into alternating text and mention pieces in source order.
Each piece retains its exact text and mention identity.
Mentions at the start and end leave only the intervening text piece, and no zero-length boundary pieces are emitted.
A mention after a surrogate pair is cut at its own characters, with the pair kept whole in one piece.
Nonempty text without mentions becomes one plain-text piece, and empty text yields no pieces.
Overlapping mention ranges are rejected instead of producing ambiguous pieces.
Offset-to-unit and unit-to-offset conversions round-trip every code-point position across a surrogate pair.
A unit inside the pair reads back as the pair's own offset.

A space-padded selection is normalized to the trimmed word span, and a selection of only spaces becomes an empty span.
Selection lengths count Unicode code points rather than UTF-16 code units.
A caret within a mention resolves to that draft mention, and so does an exact selection of it.
A wider selection or a later unrelated selection does not match a mention merely because it overlaps or follows it.
An Example draft answers a selection wholly inside one of its Mentions, and nothing for one running past its end.
A card id and a sentence id name the example a Mention request would edit.
A sentence id the card lacks reads nothing, and zero ids read nothing too.

## `TSentence`

Covers the association a Meaning or a Collocation holds over an Example.
That association carries data, so what is tested is the frame surviving a write and a read.
The whole row round-trips, because a frame the store drops is a frame the editor offered for nothing.
The stored id is not empty, since a row handed in without one is only identity once it is written.

A frame belongs to the owner, so it is storable before any sentence is.
Requiring an Example first would lose what the user wrote for no reason the data model gives.
A Collocation holds an Example on the same terms a Meaning does.
The editor already draws the frame fields on a Collocation card, so the store has to keep what they say.

Nothing ships a marker or a role.
What a store has already been given is the only list the editor can offer.
An untouched store offers nothing, which is what an empty dropdown on a fresh workspace means.
Each language is asked separately, because a Japanese particle is no offer to make on an English row.
A frame saved before any sentence was written still reaches the field that offers it.

A row the owner names again by its id is rewritten in place, so its id survives the save.
A row the owner stopped naming goes, and a row named by no id is new.
A caller holding its id must therefore find the same row after a save.
Rows come back in the order the owner gave them, not in the order the Examples were made.
That order is the sentence order the user arranged on the card.

Deleting a Meaning or a Collocation removes its sentence rows and keeps the Examples they cited.
An Example belongs to no owner, so another card may still quote it.
Two Meanings citing one Example must read it under their own frame.
A frame living on the Example instead would make the second write overwrite the first.
A Meaning and a Collocation keep separate tables, so neither write can reach the other's frame.

## `TSentenceOrder`

Covers the one thing a language pack says about an Example's frame.
That is which of its two fields is written first.
The packs on disk are read, not a fixture.
The ordering shipped with a language is the fact under test.
English writes the marker before the role, which is the order its pack declares.
Japanese declares the opposite order and must read back the opposite order.
A language whose pack reverses the two might still draw English's order.
That is why this is read from the pack.
A language nothing ships a pack for still draws its rows.
It falls back to the marker first rather than refusing to lay the row out.

## Stamp

Commit: `c6e60141a172`
- `TExample.cs` `a9f3021b68f07b9d`
- `TExampleLine.cs` `12fd9a19c0177a20`
- `TMention.cs` `cb740644a69ee697`
- `TMentionFinding.cs` `ab0e214226ad13fc`
- `TMentionSpan.cs` `fe6960d3e17c668e`
- `TSentence.cs` `6b1a3fcdaa01b3d5`
- `TSentenceOrder.cs` `8167c86b34858bbf`
