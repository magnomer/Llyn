# Anatomy

This folder holds reflex anatomy rules and parts, IPA charts, and tone contours.
Its rules cut reflex readings into onset, vowel, coda and tone.
The engine holds no language-specific facts of its own.
Everything language-specific arrives through the pack records declared here.

## `LAnatomy`

The phonological anatomy of one reflex reading holds onset, vowel, coda and tone twice.
One set is cut from the reading as fetched, the IPA or script form, and one from its respelling.
Both sets are always held together, so a consumer never picks one and finds the other missing.
It is derived by the engine from the rules the entry's own pack declares, never by a view.
Each part is text, empty when the reading has no such part or no rule cuts the language.
The respelling set is cut from the reading when it has no respelling.

## `LAnatomyEmpty`

The anatomy of a reading no rule cuts, every part empty.

## `LAnatomyIpaRead`

The IPA part a Diwei kind tallies.
An initial takes the onset, a rime takes vowel and coda joined, and any other kind is empty.
The respelling reader does the same on the respelling set.

## `LAnatomyScan`

The anatomy of one reading under the first rule that names its language.
The rule cuts the reading for the IPA set and the respelling for the respelling set.
A language no rule names yields the empty anatomy.

## `LAnatomyPiece`

One cut of one reading holds the four parts a pattern reads off it.
Two pieces, one from the reading and one from its respelling, make one `LAnatomy`.
A reading the pattern does not match gives the empty piece.

## `LAnatomyPattern`

One way of cutting a reading into its parts.
The rewrites prepare the reading and the regex cuts it.
The regex names its parts with the groups `onset`, `vowel`, `coda` and `tone`, any of which it may omit.
An omitted group reads as empty, so a toneless language simply declares no `tone` group.
Rewrites run in order before the regex sees the reading.
A kana table maps each kana to its letters, and a superscript table turns tone marks into digits.
The decomposed flag splits the reading into base letters and marks before the rewrites.
A Hangul syllable then falls into its jamo, and a nasalized vowel into the vowel and its tilde.
Off, the reading is composed instead, so a precomposed letter stays one character.
The regex is compiled once when the record is built, so a broken pattern fails while the pack loads.
The compiled form is a private field, so it never enters the record's equality.

## `LAnatomyPatternResolve`

The piece the pattern cuts from the reading, after the decomposition and the rewrites.
A reading left blank by the rewrites, or one the regex does not match, gives the empty piece.
The prepared reading is normalized as the flag says, rewritten in order, then trimmed.

## `Equals` in `LAnatomyPattern`

Two patterns are equal when their regex text, decomposition flag and rewrite sequence match.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.

## `LAnatomyRule`

One rule of the Classical Chinese pack's `anatomy` file, saying how the readings of some reflex languages are cut.
A rule names the languages it serves and holds one pattern for the reading and one for the respelling.
The rules live in the Classical Chinese pack alone, since only its entries carry reflex rows.
The respelling pattern is the IPA pattern again when the file names none.

## `LAnatomyRuleMatch`

Whether the rule names this language, compared without regard to case.

## `LAnatomyRuleResolve`

The IPA pattern cuts the text and the respelling pattern cuts the respelling.
A blank respelling is cut from the text instead, so the respelling set is never empty for want of one.

## `LArticulation`

One IPA chart, the universal notation the input aid offers.
It is no language's fact, so it lives here and not in a language pack.
Rows are one axis of articulation and columns the other.
Cells hold the symbols for a row and column.

## `LArticulationConsonantRead`

The pulmonic consonant chart.
Rows are manner of articulation and columns are place of articulation.
A paired cell holds the voiceless consonant and then the voiced one.

## `LArticulationVowelRead`

The vowel chart.
Rows are height and columns are backness.
A paired cell holds the unrounded vowel and then the rounded one.

## `LArticulationConsonant`

An empty cell is a position judged impossible or one the IPA gives no symbol for.
It is kept as a cell so that the rows and columns stay aligned with their headers.

## `LArticulationRead`

Splits each written cell into its symbols, so an empty cell holds none.

## `LContour`

One syllable of an IPA reading with the pitch levels its tone marks spell.
It is derived from the reading text alone and never stored.
The parse knows only the IPA tone notations themselves.
The text keeps every tone mark, so a sandhi form such as `²¹⁴⁻²¹` stays readable.
The levels are Chao levels in speaking order.
A level tone carries one entry and a contour tone one per turning point.
The list is empty for a syllable without a tone or with a mark outside the five levels.

## `LContourScale`

Every Chao level from the highest down to the lowest, the rungs a contour is drawn on.
It is built from the floor and the ceiling, so the scale has one owner.

## `LContourRoles`

The colour role of each level, in the same order as the levels.
Conduct maps it into `CContour`, so the view picks no colour itself.

## `LContourParse`

Splits a reading into syllables and reads the tone of each.
Three notations count as tone marks.
Superscript digits are what Wiktionary writes for Mandarin and Cantonese.
Chao tone letters `˩˨˧˦˥` are what it writes for Thai and Vietnamese.
Plain digits serve a reading typed by hand.
A syllable ends where its tone marks end, or at a space, a dot or a foot bar.
Slashes, brackets and parentheses are fences and are dropped from the text.
A reading with no marks at all still yields its syllables, each without levels.

## `LContourToneCheck`

Whether any syllable carries a tone worth drawing.

## `LContourRoleRead`

Picks the colour role of one level by its depth on `LContourScale`.
Depth 0 is the top role, and each step down takes the next role.
A level outside the scale takes the bottom role.
The view in Deportment and the Joplin writer both draw from this one pick.

## `LContourLevelRead`

A joiner, `⁻` or `-`, separates a citation tone from its sandhi surface form.
Only the surface form after the last joiner is read, because that is what is spoken.
A mark outside the five levels empties the list rather than guessing.

## `LContourRole`

The colour role of one Chao level on a drawn contour.
Every drawing names a colour per role.
Top is level 5, then high, mid and low for levels 4 to 2.
Bottom is level 1, and any level outside the scale.

## Stamp

Commit: `c2f335c331fe`
- `LAnatomy.cs` `dcae18a4dc62383d`
- `LAnatomyPattern.cs` `09972e57107f7c73`
- `LAnatomyPiece.cs` `3014588afd3f4a9e`
- `LAnatomyRule.cs` `86e1d61d797f59c3`
- `LArticulation.cs` `37b8467d3b4304c6`
- `LContour.cs` `cc791d2f4cd99776`
- `LContourRole.cs` `599351c9174db7bb`
