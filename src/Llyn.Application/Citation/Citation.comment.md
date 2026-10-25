# Citation

The clerks over the independent facts an entry cites, and the held-draft protocol for each.
A citation is an Author, an Example, a Reference or a Situation.
Each is shared data owned by nothing, so its draft names the row and carries no entry.
A citation is a reference to a row, never ownership of it.
A row nobody cites is deliberate data and goes only when something says to delete it.
This folder stands apart from Card because it owns the pool rows and not the cards that quote them.

## `LCitationClerk`

It holds the draft calls that belong to a citation rather than to an entry.
Only starting, committing and checking differ by kind.
Each kind has its own owner for those three, built and held here.
`LAuthorCitation`, `LExampleCitation`, `LReferenceCitation` and `LSituationCitation` follow one protocol call for call.
So a fifth kind cannot invent a fifth protocol.
The checks across all kinds, and the start of an entry draft, live here.
Every edit in between is a request, applied by `LDraftClerk` for all kinds alike.
The claim, the sweep and the discard are one for all kinds, and live in `LClaimClerk`.
The rows themselves are written through the clerk of each kind.
`revisions` goes to every kind owner, so the history is one list whatever kind was stored.

## `LCitationClerk.LEntryStart`

It starts an entry draft, either blank in `language` or the entry loaded with its recordings resolved.
An entry that no longer stands is refused.

## `LCitationClerk.LCitationDraftCheck`

A citation draft is measured by the check of its kind owner.
An entry draft compares to the stored entry as loaded, or to the blank draft in its language.

## `LCitationClerk.LCitationLeftoverCheck`

It answers whether a draft of any kind still says exactly what its stored origin says.
A sweep may then drop it.
A draft whose origin is gone is not a leftover.

## The citation protocol

Every kind owner follows the same start, commit and check, so the rules are written once here.
Start mints a draft id, writes the first file, and returns the held row.
With no row the content is blank under an id minted for it.
With a row the content is that row read back, which is an edit.
A row that is gone is refused before a file is written, so no draft can point at nothing.

Commit turns the held row into a stored one and returns it.
A draft naming no row is a create, and one naming a row is a rewrite.
An id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The write and the revision recording it share one session, so the history never misses a stored row.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

Check answers whether the held row differs from the one it was started from.
A draft naming no row, or whose row has since gone, is measured against the same empty row.

## `LAuthorCitation`

A blank name throws `LRefusal.LRefusalName`, because an Author is its name and the panel guards it too.
The verdict is asked of `LClaimClerk.LDraftRefusalRead`, the one owner of the unnamed-author rule.
A start with no Author is a nameless one under id zero, which is an Author being named first.
A start with an Author is a rename.
The check compares the trimmed names.

## `LExampleCitation`

The sentence id is given at start rather than at the store, so a recovered draft names the same sentence.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
Recommitting a file left nameless would store the sentence twice, and the sweep would never collect it.
Nothing settles a court here, because a sentence links to no tentative record.
The language a new sentence opens in was chosen for it rather than typed, so it is not an edit.

## `LExampleCitation.LExampleCitationNormalize`

It mints an id when the sentence carries none, as a sentence written by an older launch arrives.
A sentence already named is returned itself.

## `LReferenceCitation`

The credits of a stored Reference are read into the draft, so the panel shows them from one place.
The Reference, its credits and the revision recording them are written in one session.
A kill between them therefore cannot leave a source with no authors or no history.
The check counts the credits as well as the stated fields, so adding an Author is a change to save.
`LReferenceCitationNormalize` mints an id when none is carried and otherwise returns the Reference itself.

## `LSituationCitation`

The issuer names a new Situation and its media rows.
`LSituationCitationNormalize` names the image and video rows the same way, and drops a row with no location.
It returns the Situation itself when it and its rows were already named.

## `LAuthorClerk`

The clerk over the Authors a Reference credits.
An Author holds only its name.
So the interesting reading of an Author is the Sources crediting it and the places citing those.
Both are read as catalogs, with the counts the rows and the orderings need already summed.
Several References may credit one Author.
Folding two Authors into one lives here too, because it is the one change an Author admits beyond its name.

## `LAuthorClerk.LAuthorClerkCreate`

A blank name is refused here rather than in the archive.
This is the door the shell knocks on, and a name left empty there is a slip.

## `LAuthorClerk.LAuthorClerkRead`

The owner form reads the Authors credited on a Reference, in that Reference's own author order.
Only a Reference has credits, and any other side is refused.

## `LAuthorClerk.LAuthorClerkUpdate`

A rename means every credit reads the new name.

## `LAuthorClerk.LAuthorClerkDelete`

`detach` drops every credit first.
Deleting an Author never deletes a Reference.

## `LAuthorClerk.LAuthorClerkAbsorb`

It folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Every Source crediting the dropped Author credits the kept one afterwards, once, at the earlier of the two places.

## `LAuthorClerk.LAuthorClerkFind`

The name find uses the catalog's own match, so wildcards work here as they do in every search box.
The browsed find carries each Author's Source count and citation count.
The credits of every Source are read once and turned round into the works of every Author.
An Author credited nowhere is still listed, because the workspace still holds it.
The limited find leaves one Author out, and the union section offers these as the Authors one may fold into.

## `LAuthorClerk.LBylineFind`

It offers the byline rows a typed credit may already name, at most the limit asked.
An unnamed Author and one the draft already credits are left out, since neither can be picked.
Each trimmed name comes split around the trimmed word, culture-aware and case-blind, by `LCatalog.LCatalogMarkFind`.
`LBylineRead` trims the typed word, and the byline highlights that same word.

## `LAuthorClerk.LFellowFind`

It reads every Author credited beside `authorId` on some Source, with how many Sources credit the two together.
The credits of every Source are read once, so the tally never queries per Source.
The rows come by shared count falling, then by name, case aside, so the closest co-author leads.

## `LAuthorClerk.LAuthorUnionRead`

It gives the two names a union question shows, the Author held by `draft` and then the stored Author `kept`.
Each name loses its outer spaces, and a missing Author reads as empty.

## `LAuthorClerk.LAuthorVitaRead`

With no Author it is the sheet of nobody, worded as unnamed through `localize`.
Otherwise it carries the fellows and the citing places the caller read.

## `LAuthorClerk.LAuthorReferenceSave`

It makes the stored credits of the Reference read as the draft's list.
A credit with a minted id is created by name first.
A credit naming an Author that is gone is refused.
Each is then attached at its place in the list, which also moves one already attached.
A stored credit the list no longer names is detached last.
A name credited twice is attached once, at its first place.

## `LAuthorClerk.LAuthorClerkMatch`

Only the ids count, because a renamed Author is still the same credit.

## `LAuthorClerk.LOeuvreMatch`

Null asks for every Source, zero for the uncredited ones, and any other id for the Sources crediting it.

## `LAuthorClerk.LAuthorWorkRead`

One pass over the credits serves every row, so the catalog never queries per Author.

## `LAuthorClerk.LAuthorOwnerRaise`

It builds the refusal for a side that carries no credits.

## `LExampleClerk`

The clerk over the Examples of the workspace, and over the Example a sentence row names.
An Example is pool data owned by nothing.
So an edit made through one card must not rewrite what other cards quote.
Both card sides may reference the same one, each holding its own order over the Examples it references.
Two sides is why the side arrives as an `LOwner` and not in the method's name.
The Mentions ride with the Example, matched back into the identity map by offset.
The card clerk calls in here for every sentence row it writes.
The corpus panel reads its rows and its page here, so screen and print show one thing.

## `LExampleClerk.LExampleClerkFind`

An Example is matched over its sentence, its translation and the name of the Source it cites.
The name is resolved here, because matching on an id the reader never sees would answer the wrong question.
The cited name follows `LReferenceClerk.LCitationFormat`, the rule every citation line shares.

## `LExampleClerk.LExampleClerkMatch`

It compares field by field and leaves identity out.
A held sentence is named before the Example it becomes exists.
The language counts, because the tongue a sentence is written in is part of the sentence.
The cited Source counts too, and so do the translations and the Mentions.

## `LExampleClerk.LExampleTextMatch`

A blank field reads as nothing recorded, as the engine would store it, so it matches an empty text.

## `LExampleClerk.LExampleLineRead`

It gives the frame, the sentence and the Source line a reading card shows for one sentence row.
The frame and the sentence follow the same rules the portrait prints by, with `mark` for an unknown part.
The sentence arrives divided around its Mentions, resolved from the draft row here and only here.
The division refuses overlapping Mentions, which no writer lets a draft hold.
The Source line is the cited Source's ready line, empty when the row cites none or the line is missing.

## `LExampleClerk.LExamplePageRead`

The sentence heads the page, its language is the chip, and its usage tally follows.
Each gloss is a line labelled with its language, under the translation heading.
An example citing nothing shows no source.

## `LExampleClerk.LExampleClerkResolve`

The row form answers the Example a row names, or `null` when it names none.
A row whose Example carries neither a stored id nor a sentence names none.
An Example carrying a stored id names a stored row, however little its sentence says.

The draft form answers the stored Example a row's positive id names, updated to what the row now says.
A positive id nothing is stored under is refused.
The card is never bound to a row the user never chose.
An Example other cards also quote is pool data.
A change of its text or Source made through this card gives this card a fresh row instead.
The other cards keep the row they quoted, unchanged, because nobody edited it there.
A row with a negative id gets a fresh Example, recorded in the map under the id it replaces.
No row is ever matched by its wording, so two new rows with one sentence stay two rows.
The language falls back to the entry's when the Example states none of its own.

The Mentions count as part of what the row says.
A row whose text and Source are unchanged but whose translations or Mentions differ rewrites those rows in place.
That holds whoever else quotes the Example.
A fork made for a text or Source edit is created with the row's Mentions, already shifted by the edit.
Every Mention written under a negative id is recorded in the map, matched by its offset.

## `LExampleClerk.LMentionResolve`

It keeps the Mentions of a row that still fit its text, sorted by offset.
A span past the end is dropped here rather than refused.
A text set without the shift therefore cannot block a commit.

## `LExampleClerk.LExampleShareCheck`

It asks whether any card other than `ownerId` quotes the Example.
The owner side matters, because a Meaning and a Collocation can carry one id each.

## `LExampleClerk.LGlossRecord`

It records the stored id each new gloss row received, matched by position after a save.
The identity map lets a later request that still names the negative id reach the stored row.
`LMentionRecord` does the same for Mentions, matched by offset.

## `LFavoriteClerk`

The clerk over the favorite marks.
A favorite is a marked citation of an entry.
The mark changes no lexical data and keeps the entry's identity.
The mark carries its own stamp, so ordering by the mark is not ordering by the entry.
The filtered find leaves out the marked entries in a hidden language.
Unmarking leaves the entry standing, still reachable through the entry catalog.

## `LReferenceClerk`

The clerk over the bibliographic Sources of the workspace.
A Reference is the work an Example is drawn from, cited from any number of places.
An Example cites at most one, a single column with nothing to order, so attaching there replaces the last.
The page the sources panel reads is composed here too, so screen and print show one thing.

## `LReferenceClerk.LReferenceClerkBlank`

Its author state is unspecified, because nobody has been credited or ruled unknown yet.

## `LReferenceClerk.LReferenceClerkCreate`

The fields carry the three-state distinction the model gives them.
Each is unspecified, deliberately unknown, or a value.
The title form creates a Reference carrying nothing but `title`, for a citation typed where no stored Source answered.
A blank line is refused as an argument.

## `LReferenceClerk.LReferenceClerkResolve`

It answers the stored Reference a typed citation names, zero for a blank line, or `null` when none answers.
Both sides fold the way `LCatalogTextNormalize` folds, so case and edge spaces never mint a twin.
The held Reference wins when its title or byline answers, so re-entering an unchanged line keeps it.
Bylines repeat across works by one author, and only this rule stops a silent switch to a sibling.
Otherwise a title match beats a byline match, since a byline leaves the title out once credits exist.
Ties go to the oldest id, so the same line always lands on the same Reference.
The draft form takes the Reference cited by the Example at the site as the held one.
The engine hands the draft over, so the shell never reads the held Reference itself.
A draft without a cited Reference holds zero, which no Reference carries.

## `LReferenceClerk.LReferenceClerkRead`

The page read counts the usage tally from the rows citing the Source, as the catalog counts it.

## `LReferenceClerk.LReferenceClerkFind`

The credits and the counts are read whole rather than one Source at a time.

## `LReferenceClerk.LReferenceCitationFind`

The site names a card sentence or the draft's own Example, which the corpus transcript writes.
Each byline is split around the trimmed word by `LCatalog.LCatalogMarkFind`.
The offer keeps at most `LCatalog.LCatalogOfferLimit` rows, the cap every chip dropdown shares.
Each row carries its usage count ready to show, as `LCatalog.LCatalogUsageFormat` writes it.
The text comes back unchanged, since typing a citation writes nothing.
The private find offers the most cited first, and a blank trimmed word offers none.
A word that already names the byline of the `held` Source offers none, because the citation then stands as typed.

## `LReferenceClerk.LReferenceHeldRead`

The offer and the resolve share it, so both hold the same Reference against a typed line.

## `LReferenceClerk.LReferenceOeuvreFind`

Passing `null` lists every Source, as the sources panel lists every Entry under no chosen Source.
Passing zero lists the Sources crediting nobody, so a missing credit can be found and written.
The kinds are matched by their stored word, which is what the filter remembers between sessions.

## `LReferenceClerk.LCitationRead`

The plain read gives the `Author (Year)` line every stored Source is cited under, by id.
One read serves a whole catalog fill, because resolving a name per row would be one query per row.
The shown-entry read gives every cited id a line, so the display only looks one up.
The draft read serves the corpus transcript and excerpt, which show it beside the Example.

## `LReferenceClerk.LCitationFormat`

A citation of nothing shows nothing.
A Source no byline names shows its bare id.
A citation is worth showing even when its Source is gone.
The catalog rows of `LExampleClerk` and every citation line share this one rule.

## `LReferenceClerk.LReferenceClerkMatch`

It compares field by field and leaves identity out.
A held Reference is named before the record it becomes exists.
The title, the year, the kind, the note and the address each count.
The author state counts too, because ruling the writers unknown is an edit nothing else records.

## `LReferenceClerk.LReferenceColophonRead`

A draft holding no Source is a caller's mistake and throws.

## `LReferenceClerk.LReferenceImprintRead`

With no draft it is the sheet of a blank Source, which the imprint shows while nothing is held.
A draft holding no Source is a caller's mistake and throws.

## `LReferenceClerk.LReferenceMenuRead`

The kind menu follows `LReference.LReferenceKindMenu` order.
Each option carries the tag a pick sends and the localization key it shows.

## `LReferenceClerk.LReferenceKindRead`

It is null when no tag was picked, or when the Source already has that kind, so nothing is sent.

## `LReferenceClerk.LReferencePageRead`

The title heads the page, and the kind and usage tally are the chips.
Credited authors are joined on one line, and an unknown credit shows the mark alone.
Year, address and note each take a section only when written or marked unknown.

## `LSituationClerk`

The clerk over the Situations of the workspace.
A Situation is the context a Meaning or Collocation is used in.
It is independent data on the same terms as a Tag.
Any number of cards reference it, each holding its own order.
Rewriting a Situation is a single seam, and every card referencing it sees the new wording at once.
The image and video rows a Situation shows are situation media, settled here as a card settles its own.
The page the repertoire panel reads is composed here too, so screen and print show one thing.

## `LSituationClerk.LSituationClerkCreate`

Its title is not identity, so the same words saved twice are two Situations.
The row and its media are written in one session, so a failed picture leaves no half Situation.

## `LSituationClerk.LSituationClerkFind`

A Situation is matched over its title, its description and its kind.
The field offer trims the text, and a blank one offers nothing.
A Situation the card already links by id is left out, and so is one with a blank title.
Each title is split around the typed word by the catalog's mark rule.
At most the catalog's offer limit of rows are kept, in usage order.
Each row carries its usage count ready to show, as `LCatalog.LCatalogUsageFormat` writes it.
No draft or no such card holds nothing, so every match is offered.
The card is found by its kind and id.

## `LSituationClerk.LSituationClerkDelete`

Without `detach` it is refused while any Meaning or Collocation still references the Situation.

## `LSituationClerk.LSituationClerkMatch`

It compares field by field and leaves identity out.
A held Situation is named before the record it becomes exists.
The title, the description and the kind each count, so any of them typed is an edit.
The image and video lists count as well, through `LCardEquality`, so an added picture is an edit.
A blank row is not, because the commit drops it.

## `LSituationClerk.LSituationPageRead`

The title heads the page, and the kind and usage tally are the chips.
The description is Markdown, carried as a note so it draws as blocks.
Pictures and videos share one unheaded section, as the vignette shows them without a heading.

## `LSituationClerk.LSituationMediaSync`

It settles the Images and Videos against what is attached, through the same field sync a card uses.
A row with an id is that record, its location or span rewritten when it changed.
One without becomes a fresh record.
What the list no longer names is detached and keeps its record.

## Stamp

Commit: `8c403d1df768`
- `LAuthorCitation.cs` `41c8fed28c3f1b8c`
- `LAuthorClerk.cs` `1e45c458df96b5fa`
- `LCitationClerk.cs` `719e7a45d58a0b0f`
- `LExampleCitation.cs` `f309cf4a2c952285`
- `LExampleClerk.cs` `2bdb72e5f03e7834`
- `LFavoriteClerk.cs` `b3e702513de3e864`
- `LReferenceCitation.cs` `22b01c63bf1fb4b6`
- `LReferenceClerk.cs` `9e40dc9160b9b513`
- `LSituationCitation.cs` `4c94190b8ce67d40`
- `LSituationClerk.cs` `e05ff68196f2e351`
