# LExample.cs
Hash: `a4f2e873837bd96c`

## `public sealed record LExample(long LExampleId, string LExampleLanguage, LStateValue LExampleText, LStateAnchor LExampleSource, IReadOnlyList<LGloss>? LExampleGloss = null, IReadOnlyList<LMention>? LExampleMention = null)`

One Example, the first independent shared entity in the lexicon.
An Example is owned by nothing.
No Entry, Meaning, or Collocation contains it.
Any number of them *reference* it instead.
The order an Example appears in lives on each reference rather than here.
So the same Example can sit first under one Entry and third under a Meaning.
`LExampleId` is the identity, an opaque and program-generated stable id.
`LExampleText` is display text and never identity.
Two Examples with identical text remain distinct rows.

An Example carries its own Glosses in `LExampleGloss` and references *at most one* Source through `LExampleSource`.
A Gloss is the sentence rendered as plain text in one language.
One Example may carry any number of them.
The Translation a Meaning carries is instead a link to another Entry.
The Source reference is a pointer, not ownership.
Clearing it or deleting the Example never touches the Source.
Every text and citation field here carries a state, in `LStateValue` or `LStateAnchor`.
A field holding nothing says whether nothing was ever recorded.
It says instead when the user marked the value as not known.

**Parameters**

- `LExampleId` — Opaque, program-generated stable id.
- `LExampleLanguage` — Language code the example text is written in.
- `LExampleText` — The example text and what is known about it, display text and never identity.
- `LExampleSource` — The single referenced Source, by its id when one is cited.
  Nothing was recorded when none is cited.
  It is unknown when the user marked the citation as not known.
- `LExampleGloss` — The sentence rendered in other languages, as [LGloss](LGloss.comment.md) rows in the order the user keeps them.
  The list is the Example's own, so every card quoting the sentence reads the same renderings.
  It stands empty until one is written.
- `LExampleMention` — The words of the sentence that stand for an Entry, as [LMention](LMention.comment.md) rows ordered by start.
  The list is the Example's own, so every card quoting the sentence reads the same words the same way.
  It stands empty until one is drawn.

## `public LStateValue LExampleText { get; init; }`

A null handed in becomes unspecified, so no reader checks for null.
`LExampleSource` falls back to the unspecified anchor the same way.

## `public IReadOnlyList<LGloss> LExampleGloss { get; init; }`

A null handed in becomes the empty list, so a reader walks it without a check.
`LExampleMention` does the same.

## `public IReadOnlyList<LMention> LExampleExcerpt`

The Mentions a reading of the text may link, which are none unless the text reads soundly.
A Mention's offsets point into the stated text, so an unknown or unreadable text links nothing.

## `public bool Equals(LExample? other)`

Two Examples are equal when every field is equal and the Gloss and Mention lists match row by row.
A record compares a list by reference, which would make every read a change.

## `public override int GetHashCode()`

The hash that agrees with that equality.

## `public LExample LExampleNormalize()`

The same Example with every unreadable value dropped to unspecified.
Each Gloss is normalized the same way, and its Mentions are sorted by start.
Called only after the user agreed to lose what the store could not read.
