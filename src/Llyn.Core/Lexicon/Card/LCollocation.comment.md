# LCollocation.cs
Hash: `1192775e33212be9`

## `public sealed record LCollocation(long LCollocationId, long LCollocationEntryId, int LCollocationPosition, LStateValue LCollocationTitle, LStateValue LCollocationExpression, LStateValue LCollocationMeaning)`

One Collocation owned by an entry, a stable-id node mirroring the Meaning construction.
It carries an `LCollocationExpression`, the phrase itself.
Beside it stands the `LCollocationMeaning` that explains it, the definition analogue of a Meaning.
The card has a field for each, so a collocation keeps both, beside the `LCollocationTitle` the card is headed with.
`LCollocationId` is the identity, an opaque and program-generated stable id.
Reordering collocation cards rewrites `LCollocationPosition` only.
The id never changes.

**Parameters**

- `LCollocationId`: opaque, program-generated stable id.
- `LCollocationEntryId`: owning entry id.
- `LCollocationPosition`: order among the entry's collocations.
- `LCollocationTitle`: title typed on the Collocation card, and what is known about it.
  Nothing was recorded when none was typed.
  It is unknown when the user marked it as not known.
- `LCollocationExpression`: the collocation expression text, and what is known about it.
- `LCollocationMeaning`: what the expression means, and what is known about it.

## `public LStateValue LCollocationTitle { get; init; }`

A null handed in becomes unspecified, so no reader checks for null.
`LCollocationExpression` and `LCollocationMeaning` do the same.

## `public static long? LCollocationHomeRead(IReadOnlyList<LCardDraft> meanings, LCardDraft collocation, bool nested)`

The meaning a collocation shows under in full, which is its first link present in the tree.
A view that shows top-level meanings only passes `nested` false, which lifts the home to its root.
It is null when no link is present, and the collocation then belongs to the other collocations.
This is the one place the home rule lives, so every view asks it.

## `public static IReadOnlyList<long> LCollocationStubRead(IReadOnlyList<LCardDraft> meanings, LCardDraft collocation, bool nested)`

The meanings a collocation shows a one-line stub under.
They are its other present links in link order, lifted alike.
The home and repeats are left out, so a lift never yields two stubs under one root.

## `public static IReadOnlyList<LCardDraft> LCollocationOtherRead(IReadOnlyList<LCardDraft> meanings, IReadOnlyList<LCardDraft> collocations)`

The collocations with no home, in their own order.
These are the ones the read view and exports show under Other collocations.
A link to a meaning absent from the tree does not give a home.

## `public static LStateValue LCollocationPhraseRead(LCardDraft collocation)`

The phrase a collocation is shown by: its expression, or its title when the expression is empty.
An expression marked unknown still counts, so the reader prints its unknown mark.
The read view, its stubs and every export ask this one rule.

## `public static IReadOnlyList<string> LCollocationLanguageRead(IReadOnlyList<LTranslationTarget> targets)`

The languages of a collocation's translations, one per language in first-seen order.
A closed row shows one flag for each, and a target with no language is left out.

## `public static int LCollocationExampleRead(LCardDraft collocation)`

How many filled examples a collocation holds, the count behind a closed row's example marker.
Only filled sentences count, so a zero means the marker does not show.
It answers the fact only, and each renderer words it with its own text.

## `public static IReadOnlyList<string> LCollocationAlsoRead(IReadOnlyList<LCardDraft> meanings, LCardDraft collocation)`

The nested paths of the further meanings a collocation links to, behind a closed row's "also N" marker.
They are the meanings `LCollocationStubRead` gives in link order, so the home is left out.
It is empty when the collocation links to no further meaning, and each renderer words the list itself.
