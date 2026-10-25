# Card

The clerks that read and write what a card carries.
A card is a Meaning or a Collocation, and a card holds references to shared data rather than copies.
This folder stands apart from Citation and Draft because it owns the card rows and their fields.
Every clerk runs over the ports of one rig, and the engine calls it under its own gate.
No clerk here raises a bulletin, because the engine keeps those.

## `LCardClerk`

The clerk over Collocations and over what any card references.
It reconciles an entry's stored Collocations to the cards a draft lists.
It also writes the rows, chips, marks and links one card of either kind carries.
It composes the tag, register, translation and example clerks.

Which stored row a card is comes from `LCardDraft.LCardDraftId`, never from its place in the list.
A card naming a stored row updates that row in place, so its id survives.
A card naming nothing is created, and a stored row the draft no longer names is deleted.
A card names a stored row once, so a second card carrying that id becomes a new card.
Otherwise both cards would write the same row.

## `LCardClerk.LCollocationSave`

A row the card left as it was is neither rewritten nor recorded, so a no-op save leaves no history.
An unreadable value is still sent, because the store is what refuses it.
A blank card is skipped, so its stored row counts as no longer named.
So clearing a card removes its row, exactly as adding text to a blank one adds one.

Deletions run first, so the creates that follow append onto a set free of the going rows.
The whole surviving set is renumbered afterwards in one pass.
The unique (owner, position) index rejects a swap done row by row.
That is what `LCollocationOrderSet` on the collocation vault exists for.

## `LCardClerk.LCollocationSenseSync`

It writes the meaning links of one Collocation to match the card's ordered link list.
A negative link id is settled through the map, so a meaning created in the same save is linked.
A link naming no meaning of this entry is dropped, and a repeated link keeps its first place.
The first link left is the home of the Collocation.
It answers whether the stored links differed, so a relink alone counts as a change to record.

## `LCardClerk.LCardClerkSync`

It re-attaches the rows and Situations one card references and rewrites the Tags it carries.
A row named by a positive id is kept and moved to its new place.
A row named by a negative id gets a row of its own.
A Situation or Register whose wording is already stored is shared instead, and the map records which one.
A row the card dropped is detached only.
Those rows are independent data the card references, so the last reference going does not take the row.

Tags are not reconciled that way, because a Tag is its own text and lives on the card.
So the card's whole Tag line is written over.
Translations are written over the same way, because a link is an id the card holds.
A name that is no Entry is dropped rather than written.
A newly created card has nothing attached yet, so the same path attaches its whole set.
A stale positive id anywhere in the card is refused rather than rebound.
The card therefore never points where the user did not.

## `LCardClerk.LSentenceSync`

It writes the whole row list of one card over what the store holds for it.
A negative row id is mapped to the row the store wrote.
A row states which Example it quotes, and the example clerk resolves that Example.
A row naming no Example is written with none, which the store's check allows for a bare frame.

## `LCardClerk.LSituationSync`

The card's Situation chips are reconciled against the rows it already references, by the shared field sync.

## `LCardClerkField`

The stateless helpers over the fields a card carries.
They decide which draft rows are worth writing.
They also resolve a row into the stored Situation, Image or Video it names.
The card, meaning, entry, register and situation clerks call them, and so does `LCardEquality`.
Nothing here holds a vault, so every resolver takes the vault it writes through.

## `LCardClerkField.LCardValidate`

A Collocation carrying a card inside it is refused before anything is written.
Only a Meaning nests, because only a sense names a parent in the store.
Saving it with the children dropped would lose data the caller believes it handed over.

## `LCardClerkField.LCardRead`

A card with every field blank is neither a Meaning nor a Collocation, so it is skipped.
A card carrying nothing but a Translation is a card, because a link is something the user chose.
The form seeds one card of each kind and refuses to remove a list's last card.
So this is where an entry with nothing typed stops becoming rows.
The callers number the remaining cards in order, so skipping a middle card still leaves 0, 1, 2.

## `LCardClerkField.LSentenceRead`

The rows worth writing keep the order the card holds them.
A row stating a frame and no sentence is kept, because the frame is the card's own.
The positions stay a gapless 0, 1, 2 over what is stored.

## `LCardClerkField.LImageRead`

Rows naming no picture are dropped here rather than written.
So a list the shell built out of a control cannot leave an empty row behind.
The positions stay a gapless 0, 1, 2 over what is stored.
`LVideoRead` applies the same terms to Videos.

## `LCardClerkField.LSituationRead`

It keeps the Situation chips of one card that carry a title, in card order.

## `LCardClerkField.LSituationResolve`

A positive id nothing is stored under is refused rather than rebound.
The stored Situation is updated to what the chip now says.
A chip with a negative id is looked up by its title first, through `LSituationChip.LSituationChipResolve`.
A wording the workspace already holds is shared rather than doubled.
Only then is a fresh Situation made, recorded in the map under the negative id it replaces.

## `LCardClerkField.LImageResolve`

A positive id nothing is stored under is refused rather than rebound to a fresh row.
The stored Image is updated to the location the row now says.
A negative id gets a fresh Image, recorded in the map under the id it replaces.
Two cards naming one id therefore reference one row, which is what sharing a picture means.
`LVideoResolve` follows the same rule and updates location and span when either changed.

## `LCardClerkField.LCardFieldSync`

One card field is reconciled against the rows it already references.
Situation, Register, Image and Video differ only in the resolver and the attach and detach pair.
So those are handed in and the reconciliation is written once.

A value names its row by id and nothing else.
A positive id keeps that row, and a negative id creates one.
A card naming one id twice keeps one association.
Attaching a row the card already references moves it.
The association goes in at the end and the set is renumbered around the requested position.
So draft order becomes stored order.

## `LCardEquality`

It decides whether two card lists, or the fields inside a card, say the same thing.
Every call is static and answers by content, because records compare lists by reference.
`LDraftClerkEquality` asks it for an entry's cards.
`LSituationClerk` asks it for a Situation's own pictures and clips.
Every comparer is one overload of `LCardEqualityMatch`, told apart by the row type it compares.

## `LCardEquality.LCardEqualityMatch`

Card lists match when they carry the same cards in the same order.
Order counts, because the order cards are in is the order they are stored in.
The meaning links of a card count too, in order, since the first is its home.
Position is compared with it, so a reorder is caught by what it changed rather than by chance.

Video and image lists compare the same way, with blank rows left out first.
A Situation compares its own clips and pictures through them.

Sentence rows leave out blank rows first, so a row the form added and never filled is not a change.
The frame counts, so writing a marker or a role and nothing else is a change and is saved.
The stored row each names counts too, because a row that changed id is a different row.

Two Example rows match when they quote the same Example on the same terms.
A row quoting none matches only another quoting none.
The citation counts, so retagging a sentence is a change.
The Mentions count too, so a word linked or unlinked marks the draft dirty.

Situation lists compare title, description, kind and id, not only the title the card shows.
Id lists compare both translation targets and meaning links, in order.
Tag lists compare by id and by text, in order.

## `LCardEquality.LCardEqualityScan`

It keeps the cards that carry something, in their original order.

## `LCardEquality.LCardEqualityCheck`

It answers whether a card holds nothing at all.
An open form always shows one such card, and offering it is not an edit.
A blank sentence, image or video row counts as nothing, since the form offers those too.
A card holding a meaning link is not blank.

## `LFoldClerk`

Card folds, "More readings" and editor boxes are persistent view state, not lexical edits.
These operations neither change a draft nor record revisions.
The rig supplies the persistence port, keeping database details outside the clerk.
Nonpositive ids never reach storage, because fresh cards and entries have no stored fold marks.
A read for a nonpositive entry id answers an empty set or false.
A write for a nonpositive entry id writes nothing.

## `LFoldClerk.LFoldClerkRead`

One entry read covers folded cards of both kinds.
Each folded card is a kind and an id pair, since a Meaning and a Collocation may share an id.

## `LFoldClerk.LFoldClerkSave`

Only positive card ids can acquire or lose persistent fold marks, through save and delete.
Both take the card kind with the id.

## `LFoldClerk.LFoldClerkSpread`

Each stored entry keeps its own "More readings" state, independent of other entries.
An editor box is keyed by entry and box together, and views of one entry read the same state.
A series member's "More readings" is keyed by its entry and the series key.
It is separate from the entry page's opening, so folding a member leaves the entry page as it was.
`LStemClerk` reads the state back into each member.

## `LMeaningClerk`

The clerk over Meanings, the card kind that has a shape.
A Collocation list is flat and is reconciled by the card clerk.
A Meaning list is a tree, because `sense.sense_parent` names a Meaning in the same entry.
So three questions are asked once per sibling group rather than once per entry.
Those are which stored row each card is.
They also ask what to do with the rows no card names, and what order they sit in.
It hands each card's lines to the card clerk.

## `LMeaningClerk.LMeaningClerkSort`

The store returns Meanings grouped by parent, and a reader wants them in reading order.
So the children of each parent are sorted by position before their own children follow.
The walk keeps its own path, so a deep tree never grows the call stack.
A Meaning is named by its title, or by its definition when it has no title.
One with neither takes `unknown`, the worded fallback the caller hands in.
A row naming itself as its parent is skipped rather than followed, so it cannot loop the walk.

## `LMeaningClerk.LMeaningClerkScan`

The stored scan reads the Meanings of an Entry in stored order, roots and children together.
The draft scan flattens the draft tree into rows first, so the store's sort serves an unsaved draft too.
A path is the dotted rank such as "1.2", as `LMeaning` numbers it.
`entryId` is 0 for a draft never stored.
The private scan finds every stored row the draft still names, wherever in the tree.
The deletion pass needs that whole answer before it removes anything.
A card that has gone blank names nothing, so clearing a card removes its row and its children.

## `LMeaningClerk.LMeaningClerkSave`

It reconciles the whole Meaning tree of one entry to the cards the draft holds.
Every stored sense of the entry is read once, roots and children together.
The draft is walked whole before anything is deleted, so a card nested three deep still names its row.
Deleting a parent takes its children with it, because the store cascades on `sense_parent`.
So a child of a dropped parent is marked gone even when the draft still names it.
That card is then written as a new row rather than onto a row that no longer exists.
The store orders rows by parent id with roots last, and a moved row may sit under a larger id.
So the pass walks each row's parents up to the root to learn whether any was dropped.
Every row is marked before anything is deleted.
Only a dropped row with no dropped ancestor is deleted and reported.
A new entry is saved through here too, so every card is a created and reported row.

## `LMeaningClerk.LMeaningClerkApply`

It writes one sibling group and then, for each card, the group nested under that card.
A card naming a stored row updates that row and keeps its id.
A card that moved to another parent moves its stored row with it.
The row is not deleted and written again, because every example and link hangs from its id.
A card naming nothing, or a row already gone, creates one under this parent.
A card names a stored row once across the whole tree, not merely within one group.
The whole group is renumbered afterwards in one pass.
The unique sibling index rejects a swap done row by row.
That is what `LMeaningOrderSet` on the meaning vault exists for.

## `LMentionClerk`

It answers "what may this word mean" for a clicked position of a sentence.
It also reads what a chip line shows for the Mentions a sentence already holds.
Nothing here writes, and linking goes through `LRequestMentionAddition` on an open draft.
It uses the language cache to learn whether a language separates words.

## `LMentionClerk.LMentionClerkReach`

It bounds how many code points before the click a headword may begin in a language without word separators.

## `LMentionClerk.LMentionClerkFind`

The stored find reads the Example and asks the text find with its language and Mentions.
An id naming no Example is refused.
The reading-view find names only a line key, so text, language and Mentions come from the shown draft.
`LMentionLineFind` mints the line key, the position counted from one among the shown entry's sentences.
Those sentences run over the meaning cards first and then the collocation cards.
Only the top-level cards of each list are walked, not their child cards.
A key outside that range finds no row, so the answer is the bare offset.
A sentence the shown entry does not hold gets key zero.
The row's Example text is the text the line shows whenever the line shows words.
An Example that names no language is read in the language of the shown entry.
A row that is gone or quotes no Example answers the bare offset with no word.

The text find is the one both ask.
A Mention covering the offset wins.
The form navigates on it, or does nothing for a word standing for nothing.
Otherwise the word is resolved.
In a language without separators the longest stored headword covering the offset is the word.
Only when no headword matches, or the language separates words, does the letter run of `LMentionSpan` decide.
The candidates are every Entry of the language whose headword is that word, case folded, in headword order.

## `LMentionClerk.LMentionClerkDivide`

It divides a sentence into the pieces around its Mentions, as `LMentionSpan` divides them.

## `LMentionClerk.LMentionUnitRead`

Offsets count code points, and this maps a code-point offset to a UTF-16 index.
`LMentionOffsetRead` maps the other way.
`LMentionSpanRead` reads a UTF-16 selection as a code-point span without its outer whitespace.

## `LMentionClerk.LMentionSpanFind`

The span find is static.
So the tenure can call it under its own gate and never take the engine gate.
It answers the Mention the span lies inside, in the Example the site names.
The site is a card sentence or the draft's own Example.
It answers none without a draft.

## `LMentionClerk.LMentionClerkResolve`

It reads the span text, headword and sense of every Mention in one pass, in the order given.
The headwords are read in one batched call, each Meaning once however many Mentions narrow to it.
A Mention standing for nothing gets an empty name, for the line to substitute its own word.
The offsets count code points, so the span is cut at the UTF-16 units the sentence maps them to.

The draft form answers the mention line of every sentence row on every card, keyed by its line key.
A row with no Example yet answers an empty line, so its chips are cleared.
A missing draft answers no rows.
The site form answers one Example's line, for a card sentence or the draft's own Example.
A missing draft or Example answers an empty line.

## `LMentionClerk.LMentionSenseRead`

It answers a Meaning's title, its definition when the title is empty, and empty when the Meaning is gone.

## `LMentionClerk.LMentionClerkScan`

It finds the longest headword of the language that begins at or before the offset and covers it.
A window around the offset is read, and every headword the window contains is fetched in one query.
Each is tried at every start that could cover the click.
Longer headwords come first, so the first hit is the longest.
`LMentionRuneMatch` compares with letter case folded.

## `LMentionClerk.LMentionRuneRead`

It lists the text as code points, so offsets count what the store counts.
The example clerk and the markup link count a sentence through it too.

## `LRegisterClerk`

The clerk over Registers, which mirrors the Situation seam on a card.
A Register is shared data, so a card holds a reference rather than the wording.
The clerk resolves what a card holds into stored rows before any reference is written.
A language's shipped Registers are written on the way in rather than at workspace creation.
So a language pack added after a workspace was made still offers its Registers.

## `LRegisterClerk.LRegisterClerkFind`

The card shelf holds every Register, because a Register belongs to no language.
`language` only names the pack whose rows are seeded before the shelf is read, so a first card sees them.
An empty query offers the whole shelf.
The shelf is ordered by `LCatalogRegister.LCatalogRegisterSort` under usage, most marked first, then by name.
Storage order never decides which Registers come first.

The field offer trims the text.
A blank text offers nothing, since an empty query would list the whole shelf.
A Register whose trimmed name is blank is skipped.
A Register the card already links by id is left out.
No draft or no such card holds nothing, so every match is offered.
The card is found by its kind and id.
Each row is split around the word by `LCatalog.LCatalogMarkFind`.
At most `LCatalog.LCatalogOfferLimit` rows are kept, in the shelf's usage order.
So the most marked Registers are offered, never the earliest stored.
The offer shows only when some Register is left.

The tenor panel browses every Register counted and ordered by the same sort, the one owner of the Register order.

## `LRegisterClerk.LRegisterClerkCreate`

It makes a written Register for the tenor panel's New with no card marked by it yet.
A row already reading the same is returned rather than doubled.
The lookup is `LRegisterChip.LRegisterChipResolve`, the same rule a card's typed register meets.

## `LRegisterClerk.LRegisterClerkPrepare`

It writes the rows the named language pack names, marking every name already stored as built in.

## `LRegisterClerk.LRegisterClerkSync`

It makes the stored marks match the card, for a card that already exists.
Marks the card no longer carries are detached, and what it carries is attached in the card's order.
A name written twice on one card is kept once, because a mark is a reference and not a row.

## `LRegisterClerk.LRegisterClerkMatch`

The generated record equality compares lists by reference, so a held draft needs this instead.
It compares the marks in order.

## `LRegisterClerk.LRegisterClerkResolve`

It turns one row of the field into the id of a stored Register.
A row that names a stored Register keeps it.
A renamed written row rewrites that row's name unless the Register is built in.
A positive id nothing is stored under is refused.
The card is never bound to a row the user never chose.
A row with a negative id is looked up by its wording first.
A name the shelf already holds is shared rather than doubled.
An unknown name shares the shelf's first unknown Register the same way.
Only then is a new written Register created, recorded in the map under that id.

## `LTagClerk`

The clerk over the Tags a card carries and the catalog they are drawn from.
It holds no gate, no observer and no bulletin.
The engine calls it under its own gate and raises the tag bulletin afterwards.

A Tag is a shared row a card links by id.
A card still hands its Tags over as a whole line rather than as an attach and a detach.
The shell holds what the card should say, not a diff against what it said before.
A Tag carrying an id links that row.
A Tag carrying only text is resolved to the row reading the same, created if absent.
A row no card links any longer stays in the catalog until the taxonomy panel deletes it.

Which side an id names arrives as a `collocation` flag rather than in the method's name.
The engine turns its `LOwner` into that flag, since a Tag hangs from a Meaning or a Collocation only.

## `LTagClerk.LTagClerkRead`

It reads every Tag the workspace holds, once each, in alphabetical order.
That is what a Tag-based lookup lists.

## `LTagClerk.LTagClerkFind`

The tag catalog is held whole rather than searched in the store.
So the match is made over what was read.
The offer trims the text, and a blank text offers nothing, since an empty query would list the whole catalog.
The Tags come in usage order, and a Tag whose trimmed text is blank is skipped.
A Tag is held when a chip reads its trimmed text, the rule `LDraftHeldCheck` owns, and is left out.
At most `LCatalog.LCatalogOfferLimit` rows are kept.
No draft or no such card holds nothing, so every match is offered.
The card is found by its kind and id.
Each row is split around the word by `LCatalog.LCatalogMarkFind`.
The offer shows only when some Tag is left.

## `LTagClerk.LTagClerkSave`

It writes the line from the draft rows a card commit carries.
The store resolves each Tag to its row, by id when it has one and by wording otherwise.
The wording is the Tag's identity in the store, so a written Tag can only ever name one row.
A negative Tag id is recorded in `identity` against the row the store answered with.

## `LTagClerk.LTagClerkCreate`

It makes a Tag for the taxonomy panel's New with no card carrying it yet.
A Tag already reading the same is returned rather than doubled.

## `LTranslationClerk`

The clerk over Translations.
A Translation is a link from one card to another Entry, held as that Entry's id.
It crosses languages, so `break` in English links to `부수다` in Korean.
The link is stored one way, from the card that wrote it.
A target finds what points at it by looking back rather than by holding a list.

A card writes its whole link line at once, the way it writes its Tag line.
So no caller works out which links were added and which were taken away.
Which side an id names arrives as a `collocation` flag, which the engine derives from its `LOwner`.
It starts no fetch, which the engine keeps.
`revisions` records the revision a new stub leaves and moves the workspace row onto it.

## `LTranslationClerk.LTranslationClerkSave`

It writes that card's whole link line from the Entry ids given.
Blank ids, repeats and ids no Entry answers are dropped, and the surviving order is the order given.
An import points a card at an Entry the same file declares, and that Entry may not exist yet.

## `LTranslationClerk.LTranslationClerkFind`

It finds every Entry whose headword contains `query`, across every language the workspace holds.
A link crosses languages, so the search cannot be narrowed to the card's own one.
The Entries whose whole headword reads as `query` come first, then the rest in search order.
An empty query names the whole workspace, which is what a just-opened dropdown shows.
The Entry named by `entryId` is left out, because a card may not translate its own Entry.
A null `entryId` leaves nothing out, which is what an unsaved Entry needs.

## `LTranslationClerk.LTranslationClerkResolve`

It answers the one Entry whose whole headword reads as `word`, or nothing when none or several do.
The search behind it matches on containment, so a hit is not proof the user named it.
Resolving asks for the headword itself, so `brea` never silently becomes `breakfast`.
Both sides are folded the way `LCatalog.LCatalogTextNormalize` folds.
Several Entries sharing a headword are a question, so this answers nothing and the caller asks.

## `LTranslationClerk.LTranslationMentionFind`

A mention names a word of the draft's own language, so other languages are left out.
A draft naming no language, or no draft at all, keeps every language.
The order is the order the link search gives.

## `LTranslationClerk.LTranslationWordRead`

The word a typed translation stands for is the text trimmed, or null when nothing is left.
A blank word names no Entry, so it neither resolves nor offers prospects.
Resolving and offering both read the word here, so the rule has one owner.

## `LTranslationClerk.LTranslationLanguageRead`

It lists the languages a fresh Entry for a typed translation is offered in, one create row each.
Every language the workspace holds is offered, so the user picks rather than accepts.
The draft's own `language` comes last, because a link usually crosses into another one.
It is offered even when the workspace does not hold it, so a fresh Entry is always reachable.

## `LTranslationClerk.LTranslationClerkCreate`

It creates the bare Entry a typed word no Entry answers needs.
The row carries a headword and a language and nothing else.
A stub is a real Entry and is opened and filled in like any other later.
So its making is recorded as a revision, exactly as any other new Entry is.
The workspace row is moved onto that revision in the same session.

## `LTranslationClerk.LTranslationTargetRead`

It reads the headword and language behind each link id, in the order asked for.
An id no Entry answers is passed over rather than raised.
The draft form also looks for such an id among `links`, the draft's court links the engine reads.
Such a target shows the word and language the court recorded, until the target commits and the id settles.

## `LTranslationClerk.LTranslationIncomingRead`

It reads every card of either kind that links to the Entry.
Links are stored one way, so an Entry's incoming list is read rather than held.
The rows are twinned and given their epithet when `epithet` asks for it.

## Stamp

Commit: `8c403d1df768`
- `LCardClerk.cs` `bdb3cac0e6820774`
- `LCardClerkField.cs` `9bb3e1536c7cc7cd`
- `LCardEquality.cs` `7b2fe6cd813afde8`
- `LFoldClerk.cs` `95e87b3b392cb557`
- `LMeaningClerk.cs` `598fe8f393d8d254`
- `LMentionClerk.cs` `b38b6a236f108ba6`
- `LRegisterClerk.cs` `7244ddf600540ddd`
- `LTagClerk.cs` `2bc32514c1332d84`
- `LTranslationClerk.cs` `b6bfa9297962f513`
