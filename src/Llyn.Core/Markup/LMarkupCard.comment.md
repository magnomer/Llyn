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
Uses, then citations, then tags, then translations, then relations or synonyms, then sub-senses.
Within each of those the draft's own order is kept, which section 8 makes meaningful.

Depth is passed rather than fixed, because a sub-sense stands one level deeper than its parent.
The writer indents for a person reading the file, and the reader ignores whitespace entirely.

A citation is written only when the row it names is in this document.
Dropping the rest is what keeps every key in the file resolvable, as section 9 demands.

A sense declares its own key whenever the document names that stored row.
A relation in another entry points at a sense by that key, so the key has to be there to be named.
A collocation declares none, because nothing in the format points at a collocation.

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

## `private static void LMarkupCardAppend(StringBuilder text, int depth, LRelationDraft relation, IReadOnlyDictionary<string, string> keys)`

Writes one `<relation>` and the `<target>` inside it.

The target is written first, into a builder of its own.
A relation whose target the document does not declare is not written at all.
Writing the opening tag before knowing that would leave a relation with an empty body.

`type` is always written, because a relation without one does not import.
`label` and `labels` are written only when the relation carries them.

## `private static bool LMarkupCardAppend(StringBuilder text, int depth, string name, LSynonymDraft target, IReadOnlyDictionary<string, string> keys)`

Writes one `<target>` or one `<synonym>`, naming an entry or a sense.

The two elements differ in their tag alone, so the tag is passed in.
Which attribute is written follows the draft: an entry key or a sense key, never both.
The answer says whether anything was written, which is how a relation learns to drop itself.
A row the document does not declare is written as nothing rather than as a broken key.
