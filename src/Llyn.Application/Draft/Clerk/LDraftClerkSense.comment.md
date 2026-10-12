# LDraftClerkSense.cs
Hash: `3cd3d97f842cc243`

## `public static class LDraftClerkSense`

The links of a draft from its collocations to its meanings.
A collocation holds an ordered list of meaning ids, and its first link is its home.
Links never leave their entry, so every check reads the draft alone.

## `public static LEntryDraft LCardSenseAttach(LEntryDraft content, long cardId, long senseId)`

Appends one link at the end of the collocation's list.
The card must be a top-level collocation, and the sense a meaning of the same draft at any depth.
Anything else is refused as an unknown card.
A sense already linked returns the same draft, so no history step records a void edit.

## `public static LEntryDraft LCardSenseDetach(LEntryDraft content, long cardId, long senseId)`

Drops one link and keeps the order of the rest.
An unknown collocation is refused, but an absent link returns the same draft.

## `public static LEntryDraft LCardSenseSweep(LEntryDraft content, LCardDraft removed)`

Drops every link to a removed meaning and to each of its descendants.
Only the public card removal calls it.
A move goes through the private removal, so a moved meaning keeps its links.
A draft with nothing to drop is returned as it came.

## `public static LCollocationLayout LCollocationLayoutRead(LEntryDraft content, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets)`

Applies the home, stub and other rules of `LCollocation` to one draft for the reading view.
The view shows collocations under top-level meanings, so homes and stubs are lifted to their root.
A stub line still names the path of the nested home, so the reader learns which sub-meaning holds it.
Collocations keep draft order within each meaning, and the reader sorts them by position.
It also applies the phrase and marker rules to every collocation, keyed by its id.
The flags come from `targets`, the translations the caller already read.
The markers stay facts, a count and a list of paths, so the reader words them.
The stub line carries the phrase, so a collocation with no expression is named by its title.
It also carries the collocation's id, so the reading view can open that row and scroll to its home.

## `private static IEnumerable<long> LCardSenseRead(IReadOnlyList<LCardDraft> cards)`

Yields the id of every card in the list and below it, parents before children.
