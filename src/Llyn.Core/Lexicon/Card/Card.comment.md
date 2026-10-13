# Card

Card holds the lexical content cards of an entry, which are Meanings and Collocations.
It also holds the grasp score and the pure rules that place collocations among meanings.
Every text field carries a state, in `LStateValue` or `LStateAnchor`.
It says whether nothing was recorded or the user marked it not known.
A null handed to such a field becomes unspecified, so no reader checks for null.
A null list becomes an empty list for the same reason.
Normalize methods drop unreadable values to unspecified.
They are called only after the user agreed to lose what the store could not read.

## `LCardDraft`

Meanings and collocations share one immutable card shape, so their common fields have one definition.
The record carries lexical content and row identity, but no fold state.
Collection order belongs to the supplied draft, rather than being reconstructed by this value.
Generated equality compares collection references, so equal row contents alone do not guarantee equal cards.
The meaning field is labelled Definition for meanings and Meaning for collocations.
Sentence holds can carry a frame without an example.
Translations are target entry ids, rather than copies of target headwords.
The position is the supplied display number, independent of the row id.
Negative editor ids and zero both remain unsaved according to `LCardDraftStored`.
The sense list holds the ids of the meanings a collocation links.
The first link present in the meaning tree is the home, and a meaning card carries none.
`LCardDraftNormalize` leaves identity, position, translation ids and tags untouched, since it walks state-bearing values.
`LCardDraftExemplified` counts any sentence hold, even when that hold is empty.
`LCardDraftStored` is true only for a positive id, so views decide fold eligibility without deriving it from ids.
`LCardDraftTally` counts this card and all descendants at every depth.
`LCardDraftEmpty` covers the scalar fields, supporting rows and translations, but not identity or position.
Any child or meaning link prevents emptiness, even when that child's own fields are empty.
Only zero translation ids are empty, and blank tag text contributes nothing.

## `LCardKind`

Which of the two card lists a request means.
A Meaning card and a Collocation card share one shape.
So a request that adds one has to say which list.
An existing card is named by id instead, because the engine knows which list holds it.
A meaning card may nest cards under it, and a collocation card holds none.

## `LCollocation`

One Collocation owned by an entry, a stable-id node mirroring the Meaning construction.
It carries the phrase itself as `LCollocationExpression`, and `LCollocationMeaning` explains it.
The title heads the card.
Reordering collocation cards rewrites the position only, and the id never changes.

## `LCollocationHomeRead`

The meaning a collocation shows under in full, which is its first link present in the tree.
A view that shows top-level meanings only passes `nested` false, which lifts the home to its root.
It is null when no link is present, and the collocation then belongs to the other collocations.
This is the one place the home rule lives, so every view asks it.

## `LCollocationStubRead`

The meanings a collocation shows a one-line stub under.
They are its other present links in link order, lifted alike.
The home and repeats are left out, so a lift never yields two stubs under one root.

## `LCollocationOtherRead`

The collocations with no home, in their own order, shown under Other collocations.
A link to a meaning absent from the tree does not give a home.

## `LCollocationPhraseRead`

The phrase a collocation is shown by is its expression, or its title when the expression is empty.
An expression marked unknown still counts, so the reader prints its unknown mark.
The read view, its stubs and every export ask this one rule.

## `LCollocationLanguageRead`

The languages of a collocation's translations, one per language in first-seen order.
A closed row shows one flag for each, and a target with no language is left out.

## `LCollocationExampleRead`

How many filled examples a collocation holds, behind a closed row's example marker.
Only filled sentences count, so a zero means the marker does not show.
It answers the fact only, and each renderer words it with its own text.

## `LCollocationAlsoRead`

The nested paths of the further meanings a collocation links to, behind a closed row's "also N" marker.
They are the meanings `LCollocationStubRead` gives in link order, so the home is left out.
Each renderer words the list itself.

## `LCollocationLayout`

Where the reading view places each collocation of one entry draft.
It carries values only.
The home, stub and other rules stay on `LCollocation`, and an Application workflow applies them.
The home and stub keys are top-level meaning ids, because the reading view shows collocations under top-level meanings only.
The phrase and marker keys are collocation ids, one for every collocation of the draft.
A meaning with no homed collocation has no home key.
Each stub line names the collocation's id, its phrase and the path of its nested home.
The id lets a click open the collocation's row.

## `LGrasp`

The user's own judgment of how well they know an entry.
It is a half-step score from zero to five stars, stored as an integer count of half steps.
Zero means the user has not rated the entry yet.
Nothing fetches it and no language pack declares it, so it belongs to the user alone.
`LGraspStep` is the highest stored value, ten.
`LGraspNormalize` clamps into the stored range.
Both entry writers clamp through it, so the Livery and the export sheet never differ on an out-of-range grasp.
`LGraspKeyRead` gives the localization key `Grasp.Level0` through `Grasp.Level10`.
`LGraspStarScan` gives the fill of each star as 0 empty, 1 half or 2 full.
It draws one star per two steps of the limit, rounded up, so five stars.
The view's star control and the Livery both read it, so neither keeps its own fill rule.

## `LGraspStar`

Everything a star control draws and hit-tests for one limit, answered before any pointer moves.
The view only looks answers up by step or star, so it computes no grasp rule.
The unrated look is true only at step zero.
`LGraspStarResolve` counts a negative limit as zero and never draws more than Core's five stars.
So a limit of zero draws no star and answers no hit.
A half never picks a step above the limit, so an odd limit's last star tops out at its half.
The fills come from `LGrasp.LGraspStarScan`, the only copy of the fill rule.

## `LMeaning`

One Meaning owned by an entry, a node in the entry's self-referential Meaning tree.
Its id is the base every later tag, situation and example association targets.
It nests under another through the parent id, which always names a Meaning in the same entry.
A root Meaning has no parent.
There is no separate definition entity, and each Meaning holds exactly one definition, empty when unset.
`LMeaningName` is the title, then the definition, then empty.
A list that finds it empty prints its own unknown mark.
`LMeaningPathRead` gives the dotted path in the draft tree, such as "1.2", ranked among siblings from one.
It is the format a mention's `<sense>` uses in markup.
It is null for id 0 or an id absent from the tree.
`LMeaningPathFind` is the inverse and answers 0 when a step is not a number or the path leaves the tree.
`LMeaningRootRead` answers 0 for an id absent from the tree.
`LMeaningDraftRead` gives flat rows holding the cards' own ids, so a collocation link can be checked against them.
Order across levels is not guaranteed.

## `LUsage`

One referring side of a shared object, named rather than counted.
A shared object is referenced by Meanings, Collocations or Examples, never by an Entry as a whole.
A count alone says how many, not which side, and not under which Entry.
It is a read result rather than stored data, so nothing here is written back.
The Entry it carries is the way from the referring side to the panel holding it.
A Meaning is named by its title, and by its definition when it carries no title.
A Collocation is named by its title, and by its expression when it carries no title.
`LUsageName` is the headword until the clerk tells twin headwords apart.
`LUsageEpithet` is filled by the engine when the setting asks for it, else empty.

## Stamp

Commit: `c2f335c331fe`
- `LCardDraft.cs` `f129077a016df2cf`
- `LCardKind.cs` `a3b69b9ff90b4039`
- `LCollocation.cs` `1192775e33212be9`
- `LCollocationLayout.cs` `be1752976a8509f9`
- `LGrasp.cs` `b41c99735ab6371c`
- `LGraspStar.cs` `9cd58857f207f8db`
- `LMeaning.cs` `a467044e1d4d18bc`
- `LUsage.cs` `c1cc5edfa39740bb`
