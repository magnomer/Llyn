# LDiweiLine.cs

## `public sealed record LDiweiLine(`

One line of a Diwei page section: a label, its reconstructed reading, and the characters placed there.
An initial page lines by rime and roundedness, a rime page by initial ranked under the Hypothesis.
`LDiweiSection.LDiweiSectionScan` builds the lines and fills their characters before the page is handed out.

**Parameters**
- `LDiweiLineReading`: the reconstructed reading between slashes, or empty when no Hypothesis gives one.
- `LDiweiLineLabel`: the rime on an initial page, the initial on a rime page.
- `LDiweiLineRounded`: true for the rounded line of an initial page.
- `LDiweiLineRank`: the Hypothesis rank of the initial on a rime page, minus one when unranked.
- `LDiweiLineCharacters`: the placed characters in the order they were met, blanks and repeats dropped.
