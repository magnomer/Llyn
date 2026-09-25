# LExample.cs

## `public sealed record LExample(`

One Example — the first independent shared entity in the lexicon.
An Example is owned by nothing: no Entry, Meaning, or Collocation contains it.
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
The Source reference is a pointer, not ownership: clearing it or deleting the Example never touches the Source.
Every field here that can stand empty carries `LStateValue`.
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

## `public bool Equals(LExample? other)`

Two Examples are equal when every field is equal and the Gloss and Mention lists match row by row.
A record compares a list by reference, which would make every read a change.

## `public override int GetHashCode()`

The hash that agrees with that equality.

## `public LExample LExampleNormalize()`

The same Example with every unreadable value dropped to unspecified.
Each Gloss is normalized the same way, and its Mentions are sorted by start.
Called only after the user agreed to lose what the store could not read.
