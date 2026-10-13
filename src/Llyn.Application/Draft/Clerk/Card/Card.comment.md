# Card

This folder holds the card arithmetic of a draft and the links a card holds.
They are tag, translation, register and situation chips and the collocation-to-meaning links.

## `LDraftClerkCard`

Card edits share the recursive search, so requests reach nested cards without a second addressing rule.
Card-specific list edits share `LCardChange`, while shared-row renames visit every card through `LDraftRowChange`.
Only a card addition mints an id, through the issuer the clerk hands in.
Every other routine is pure and works on cards already named.

## `LDraftClerkCard.LCardApply`

It routes every card request to its handler, and answers null for any other request.
Link additions and removals go to `LDraftClerkSense`.
An expression request goes to `LCardExpressionChange`.

## `LDraftClerkCard.LCardExpressionChange`

Expression edits refuse nonempty values only for top-level meanings, so nested meanings use the ordinary card change.
An empty value sent to a top-level meaning hands the draft back unchanged.
Nothing typed is lost that way, and a request that writes every field of a meaning still applies.
An expression for any other card goes through `LCardChange`.

## `LDraftClerkCard.LCardInsert`

The request form mints the id of the new card here rather than leaving it to the normalize.
The answer must name the card, and a card named at birth cannot be confused with one named later.
The clerk also asks here when a request naming card zero finds the draft without a Meaning.
A collocation takes no parent, because only a meaning names a parent in the store.
A parent the meanings do not hold is a missing card, and is refused as one.
A place beyond either end of the list is read as that end.

## `LDraftClerkCard.LCardRemove`

It drops the card named from the meanings, their children or the collocations, and refuses when none carries it.
A removed meaning takes every collocation link to it and to its descendants with it.
The list form hands the removed card back, so a move can put it somewhere else.

## `LDraftClerkCard.LCardMove`

The card is taken out of wherever it sits, then put back under the parent named.
Its kind is remembered from where it was found, so a card never changes list by moving.
A parent inside the card being moved is gone by the time it is looked for.
So the move refuses rather than loops.

## `LDraftClerkCard.LCardChange`

It applies one change to the card named, wherever it nests, and refuses when no card carries the id.
Only the lists on the way down to the changed card are copied, so the rest keep their reference.

## `LDraftClerkCard.LCardEndRead`

A card the user adds goes last, whoever asks for it.

## `LDraftClerkCard.LCardLoneCheck`

It asks whether the card is the only one in the meanings or in the collocations.
Removing it would leave the list with nothing to type into, so a user edit keeps it.
The removal request itself stays free to empty a list, as a replay or an import needs.

## `LDraftClerkCard.LCardShiftRead`

It answers the place a card moves to in its own list, counting from zero, or null when nothing moves.
A card in neither top-level list, a place outside its list, or the place it holds already answers null.
So a list of one never moves, and the caller sends nothing.

## `LDraftClerkCard.LCardFind`

The id find asks the match find for the card carrying that id.
The match find tries each card before its children, so a parent wins over a child that also matches.
The id find and the media finds share it, so no finder forgets the child cards.

## `LDraftClerkCard.LCardOwnerFind`

Only the meanings and the collocations are scanned, since a child card has no owner list of its own.

## `LDraftClerkCard.LCardPositionUpdate`

It renumbers one list from one, which every insert, removal and move leaves to do.
A card already carrying its number is kept as it is, so an untouched card stays the same reference.

## `LDraftClerkChip`

The tag and translation chips of a card, each a link to a shared row.
A card adds a new tag by its text or picks a stored one by id.
A tag's text is edited by its id and reaches every card holding it.
A translation is only ever picked, because an entry is never written from inside another entry's card.
Situation chips live in `LSituationChip` and Register chips in `LRegisterChip`, each over its own shelf.

## `LDraftClerkChip.LTagApply`

It routes every tag chip request to its handler, and answers null for any other request.
The clerk then hands that request on to the translation chips, and `LTranslationApply` hands on to the picture rows.

## `LDraftClerkChip.LTagAdd`

The new tag takes the typed text trimmed and a minted id, at the place asked for.
Blank text, or text the card already holds, leaves the draft unchanged.
Every driver meets the same rule here, not in a view.

## `LDraftClerkChip.LTagInsert`

It copies the stored tag under its own id into the card, unless the card already holds it.
A stored tag whose text the card already holds is skipped too.
`LDraftClerkList.LDraftListInsert` holds the id test.
`LDraftClerkList.LDraftHeldCheck` holds the text test, and is the one owner of the held-text rule.

## `LDraftClerkChip.LTagChange`

It rewrites the tag in every card holding it, and refuses when none does.
The text is trimmed, so every tag a draft holds reads ready to show.
Removing and moving a tag refuse when the card holds none.

## `LDraftClerkChip.LTranslationInsert`

It links the entry, or the court's target draft, to the card, unless the card already links it.
Nothing is read from the store, because a target draft is not stored yet and the commit settles ids anyway.
An id of zero is refused with the target reason.
Removing and moving a link refuse when the card holds none.

## `LDraftClerkSense`

The links of a draft from its collocations to its meanings.
A collocation holds meaning ids whose homes and stubs are resolved by `LCollocation`.
Links never leave their entry, so every check reads the draft alone.

## `LDraftClerkSense.LCardSenseAttach`

It appends one link at the end of the collocation's list.
The card must be a top-level collocation, and the sense a meaning of the same draft at any depth.
Anything else is refused as an unknown card.
A sense already linked returns the same draft, so no history step records a void edit.

## `LDraftClerkSense.LCardSenseDetach`

It drops one link and keeps the order of the rest.
An unknown collocation is refused, but an absent link returns the same draft.

## `LDraftClerkSense.LCardSenseSweep`

It drops every link to a removed meaning and to each of its descendants.
Only the public card removal calls it.
A move goes through the private removal, so a moved meaning keeps its links.
A draft with nothing to drop is returned as it came.

## `LDraftClerkSense.LCollocationLayoutRead`

It applies the home, stub and other rules of `LCollocation` to one draft for the reading view.
The view shows collocations under top-level meanings, so homes and stubs are lifted to their root.
A stub line still names the path of the nested home, so the reader learns which sub-meaning holds it.
Collocations keep draft order within each meaning.
It also applies the phrase and marker rules to every collocation, keyed by its id.
The flags come from `targets`, the translations the caller already read.
The markers stay facts, a count and a list of paths, so the reader words them.
The stub line carries the phrase, so a collocation with no expression is named by its title.
It also carries the collocation's id, so the reading view can open that row and scroll to its home.

## `LDraftClerkSense.LCardSenseRead`

It yields the id of every card in the list and below it, parents before children.

## `LRegisterChip`

The Register chips of a card, each a link to a shared Register row.
A card adds a new row by its name or picks a stored one by id.
A rename is made by the Register's id and reaches every card holding it.

## `LRegisterChip.LRegisterChipApply`

It routes every Register chip request to its handler, and answers null for any other request.
The clerk then hands that request on to the tag chips.

## `LRegisterChip.LRegisterChipAdd`

The name is looked up on the whole shelf.
A stored Register reading the same way is picked under its own id.
Only a name nothing matches becomes a new register with a minted id.
A name the card already shows is nothing to add, so a typed duplicate leaves the draft unchanged.

## `LRegisterChip.LRegisterChipInsert`

It copies the stored register under its own id into the card, unless the card already holds it.
Removing, moving and renaming refuse when the card, or for a rename every card, holds none.

## `LRegisterChip.LRegisterChipResolve`

An uncertain name is resolved separately, so it never becomes an ordinary blank-name lookup.
An unknown name matches the first stored Register whose name is also unknown.
Each import of an unknown register would otherwise add one more nameless row to the shelf.
An unknown name never matches a named Register.
`LRegisterClerk` asks the same question when it creates a Register and when it settles a card's row.
The rule is written here once.

## `LSituationChip`

The Situation chips of a card, each a link to a shared Situation row.
A card adds a new row by its title or picks a stored one by id.
A field of a shared Situation is edited by its id and reaches every card holding it.
Its change also reaches the situation panel's own draft, which holds no cards.

## `LSituationChip.LSituationChipApply`

The draft form routes every situation field request to the change by id, and answers null for any other.
A field request may mean the panel's situation or a chip, so it is routed by id.
The clerk then hands any other request on to the entry content.
The content form routes every situation chip request on a card to its handler.
The clerk then hands that request on to the Register chips.
The private form rewrites the situation list of the card named through the list routine given.
A card the draft does not hold is refused by the card change.

## `LSituationChip.LSituationChipAdd`

The title is looked up first, and a stored Situation reading the same way is picked under its own id.
Only a title that no single stored Situation matches becomes a new situation with a minted id.
A title the card already shows is nothing to add, so a typed duplicate leaves the draft unchanged.
The chip holds this rule so the form, an import and a test all get one row for one wording.

## `LSituationChip.LSituationChipInsert`

It copies the stored situation under its own id into the card, unless the card already holds it.
An id naming no stored situation is refused.
Removing and moving refuse when the card holds none.

## `LSituationChip.LSituationChipChange`

It applies one change to the situation named, wherever the draft holds it.
The situation panel's own situation is looked at first, since a panel draft holds no cards.
Otherwise every card is offered the change, and a draft holding no such situation is refused.
The change is written against the stored shape and mapped onto the chip shape, so one lambda serves both.
`LSituationChipRead` converts between the two shapes.

## `LSituationChip.LSituationChipResolve`

It finds the one stored Situation whose folded title is the folded title given, or null.
Two stored Situations reading the same way answer null too, since neither is the one meant.
`LCardClerkField.LSituationResolve` asks the same question when it settles a chip, so the rule is written here once.

## Stamp

Commit: `c2f335c331fe`
- `LDraftClerkCard.cs` `b03badb861b0a014`
- `LDraftClerkChip.cs` `b96e58b192f9a9ea`
- `LDraftClerkSense.cs` `3cd3d97f842cc243`
- `LRegisterChip.cs` `57d69f23b8552a78`
- `LSituationChip.cs` `b7017af410e8ef14`
