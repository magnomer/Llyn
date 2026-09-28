# CCitationRow.cs

## `public sealed record CCitationRow(`

One Source the corpus citation field offers for the typed text, already split around the match.
The driver draws the three parts as they come and decides nothing about the match.

**Parameters**

- `CCitationRowId`: the stored reference.
- `CCitationRowLead`: the byline before the match, or the whole byline when nothing matches.
- `CCitationRowMark`: the matched part of the byline, empty when nothing matches.
- `CCitationRowTail`: the byline after the match.
- `CCitationRowCount`: how many examples cite the reference, empty when none do.

## `private const int CCitationRowLimit = 8;`

The most Sources the drawer offers, so a short word never floods the popup.

## `public static IReadOnlyList<CCitationRow> CCitationRowFind(IReadOnlyList<CCatalogReference> found, string word)`

The rows the citation field offers, at most eight, in the order the engine found the references.
The engine already dropped a blank word and a word naming the cited Source, so only the split remains.
The mark is found for the word without its surrounding blanks, which the engine searched for too.
The split sits in Conduct, since a controller that computes over engine rows reads as a driver.

## `private static CCitationRow CCitationRowRead(CCatalogReference row, string word)`

Splits one byline around the first match of the word, ignoring case in the current culture.
A byline that does not hold the word reads whole as its lead.
A reference no example cites shows no count.
