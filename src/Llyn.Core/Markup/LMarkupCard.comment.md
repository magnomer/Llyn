# LMarkupCard.cs

## `public static class LMarkupCard`

Writes one meaning or collocation block, and the sub-senses below it.

## `public static void LMarkupCardAppend(StringBuilder text, int depth, string block, LCardDraft card, IReadOnlyDictionary<string, string> keys)`

The block name is passed in because a sense and a collocation differ mostly by their tag.
Where they differ in content the flag decides.
A sense writes `<gloss>`, `<labels>` and a `<meaning lang="...">`, because only a sense records those.
A collocation writes `<expression>` and a `<meaning>` with no language.
A collocation does not nest, so its children are not written even if a draft carries some.

The element order follows the reader's own switch, so a written file reads back in one pass.
Uses, then citations, then tags, then translations, then sub-senses.
Within each of those the draft's own order is kept, which section 8 makes meaningful.

Depth is passed rather than fixed, because a sub-sense stands one level deeper than its parent.
The writer indents for a person reading the file, and the reader ignores whitespace entirely.

A citation is written only when the row it names is in this document.
Dropping the rest is what keeps every key in the file resolvable, as section 9 demands.

**Parameters**

- `text` — The document being written.
- `depth` — How far the block is indented, counted in two-space steps.
- `block` — `sense` or `collocation`.
- `card` — The card to write.
- `keys` — Every stored row id, and the document key it is written under.

## `private static void LMarkupCardAppend(StringBuilder text, int depth, string name, string row, IReadOnlyDictionary<string, string> keys)`

Writes one citation, such as `<situation ref="around-a-hearth"/>`.
Every kind of citation has that one shape, so one helper writes them all.
A row the document does not declare is written as nothing rather than as a broken key.
