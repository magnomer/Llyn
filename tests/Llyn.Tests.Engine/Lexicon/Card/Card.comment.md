# Card

This folder holds the tests of what a card carries.
They cover meanings, registers, tags, translations, situations with their media, images and videos.

## `TImage`

Covers the Images a card carries end to end, through the one path the form uses.
That is a draft saved, loaded back, and updated.
The order a card gives its Images is the order they come back in.
Dropping one from a card detaches it.
A location left blank is written nowhere, not stored as an empty picture.
One card may hold a file path and a web address at once.
The store keeps the text and never asks whether the picture can be reached.

## `TMeaning`

Covers how the Meanings of an entry read back.
Sub-senses saved under a parent read back with that parent and their own positions.
The engine walks them into reading order for the word menu, each with its depth.
A Meaning with no title or definition takes the worded fallback.
Meanings hang from an Entry and from nothing else.

## `TRegister`

Covers the engine's Register seam, which mirrors the Situation one on a card.
The shelf holds two kinds of row, the ones a language pack ships and the ones the user writes.

A card marked with names a language pack ships must reference that pack's rows rather than write new ones.
A shipped row is found by its name, since the store mints its id.
The card's order is kept, because a mark is ordered on the reference and not on the row.
Two language packs shipping the same name must share one built-in row, and both entries are found through it.
A name the user wrote before any pack shipped it keeps its row when a pack later ships it.
The row turns built-in in place, so every mark made on it survives.
A name no pack ships is stored as a written Register, belonging to no language.

Two cards typing the same wording must reference one row.
Otherwise the shelf would grow a duplicate every time a name was typed instead of chosen.
This holds even when the wording was never looked up.
The engine folds case and edge spaces when it looks the wording up.
The panel therefore needs no matching rule of its own.
A client that never consults the shelf, such as an import, gets the same shelf as the form.

The panel browsing the shelf must see a Register nothing is marked with, which no card read can reach.
It must also see the count on every row without asking once per row.
Under the usage ordering the most-marked row leads, which is the whole reason that ordering exists.
Registers marked equally often are listed by name, whatever their case, so storage order never shows.
Two Registers whose names differ only in case stand equal under the name and under the usage.
The lower id leads as the last tie-break, so the result stays stable whatever order the rows arrive in.
A query naming a language finds no row, because registers are found by their own wording.

The middle column of the panel lists the Entries whose cards carry the chosen Register.
A Register carrying no id stands for the whole workspace, which is what the panel shows before one is chosen.
The tenor panel's New makes a written Register no card marks yet, with its wording trimmed.
It is listed with no marks counted, so the panel can select it at once.
A wording the shelf already holds, whatever its case, answers the stored row rather than a second one.

A Register row wears the icon its name owns, read without the name's outer spaces and whatever its case.
A name with no icon of its own, or no name at all, wears the plain register icon.

A card's register offer keeps only the first rows, so its order decides which Registers appear at all.
The marked Register was stored last, yet it leads the offer.
The unmarked ones follow by name.
Those cut by the limit are the last by name, not the last stored.

## `TSituation`

Covers the engine's Situation seam, which mirrors the Tag one.
A Situation is created, read, rewritten and referenced from both card kinds.
Situations on a card are read back through the entry's load, as the editor reads them.
The Images and Videos a Situation shows are covered by `TSituationMedia`.

Rewriting a Situation shows its new title on every card that references it, Meaning and Collocation alike.
A Situation is one shared row, so no card may keep the old wording.
A request names its card by kind and id.
So a pick aimed at the Collocation of a fresh entry lands on the Collocation.
A Meaning is number 1 as well.
The Meaning then keeps no Situation.
A card reads its Situations in the order they sit on it, and a pick at position 0 goes first.
The order is the one the user arranged, so a read must not fall back to creation order.
Editing, reordering and adding Situations through a card keeps the id of each edited one.
The edited row is rewritten in place and a new chip becomes a new row.
So no card loses a reference.

The plain delete is refused while a card references the Situation.
Detaching it from the card empties that card and leaves the Situation row standing.
The detaching delete is the one the user confirms after being told how far it reaches.
Every reference goes with the row, on both association tables.
What each card has left is renumbered.
A gap in a card's positions is the next attach failing on the unique index.

The panel browsing the shelf must see a Situation nothing references, which no card read can reach.
It must also see the count on every row without asking once per row.
A Situation nothing references is absent from the counts rather than present as zero.
The itemized usage says which Meaning or Collocation carries the Situation, and under which Entry.
So the relationship is never flattened into the Entry alone.
A Meaning is named by its definition and a Collocation by its expression when neither carries a title.

### `TSituationDraftRead`

Builds the chips a form holds for stored Situations, so the save picks them rather than minting new ones.

## `TSituationMedia`

Covers the Images and Videos a Situation shows, the way a Meaning shows its own.
Media rows are created with the Situation, kept or dropped on update, and unlinked on delete.
The Image and Video records themselves outlive every link.
The rows come back with their ids and in the order given.
Each Image and Video counts the Situation as a referrer.
An update names the rows to keep and the rows to add.
What it leaves out is detached and renumbered around, and its record stays for whatever else shows it.
The catalog read fills media in two grouped queries rather than two per Situation.
Three Situations with different media prove each row lands under its own owner and none leaks to a neighbour.

## `TTag`

Covers Tags as the entry editor writes them, through the cards of a saved entry.
A Tag line is saved on either card kind and read back through the entry's load.
The catalog's own Tag create is covered too.

A Meaning and a Collocation each read back the Tag line they were saved with.
The catalog lists each wording once, however many cards carry it.
A Tag keeps its inner spacing and wording, loses its edge spaces, and a blank item is dropped.
The stored wording is what the catalog shows, so the save must not alter it further.
A wording repeated on one card is one Tag, and the positions left are contiguous.
A repeat would otherwise show twice on the card and take a gap in the order.

Two cards carrying the same wording link one Tag row, not one each.
The catalog count of a Tag would otherwise be split across duplicates.
A chip naming a stored Tag links that row, so the stored wording wins over the chip's text.
A Tag no card carries any more stays in the catalog and browses to no entry.
The taxonomy panel's New makes a Tag no card carries yet, trimmed, listed at once and browsing to no entry.
A wording already stored answers the stored row rather than a second one.

## `TTranslation`

Covers the Translation link store, which mirrors the Tag one on an id instead of a text.
A card writes its whole line at once, so most cases are a write followed by a read.
It also covers resolving a typed word to one Entry, and finding the Entries a word could name.

A Translation line is ordered by the card, not by the target headword.
So the order given going in is the order coming back, numbered from zero.
A card links an Entry or it does not, so a repeat is one link rather than two.
A blank id names no Entry and is dropped the same way.
The positions left behind are contiguous, because the row that folded away never took one.
Editing a card is the same whole write as making it.
So no caller works out which links were added and removed.
An update with one link fewer drops that row and rewrites nothing else.

A link is the target Entry, so a deleted target leaves no link behind.
The card that pointed at it is not a part of that Entry and stays, now carrying one link fewer.
Links are stored one way, so the target finds them by looking back rather than by holding them.
Both card kinds are found, each naming the Entry it belongs to.
An Entry nothing points at finds nothing, which is not an error.

A link is part of the card draft, so a save writes it and a load brings it back.
A blank card is skipped, and a link is not blankness.
So a card whose only content is a link is written and comes back holding it.
Without a row the link would have nothing to hang from and would be lost on save.

SQLite binds at most 32 766 parameters to one statement.
A card asking for more ids than that must still read in one statement.
So the ids travel as one JSON value.
A card shows headwords and the rows hold only ids, so the two are joined once for the whole card.
The order asked for is the order returned.
An id no Entry answers is passed over rather than raised, because a caller wants what it can show.

A typed word resolves only to an Entry whose whole headword reads as it, ignoring case and padding.
A prefix finds candidates but settles nothing, so the card never links a half-typed word.
Two Entries sharing one headword resolve to nothing, and the find still lists both.
Picking either one silently would link the wrong Entry, so the user must choose.
Given the id of the Entry being edited, the resolve and the find leave that Entry out.
An Entry may not translate itself, and the same word still resolves for any other card.

## `TVideo`

Covers Core's rule that names the hosted film inside a video address.
It also covers the rule that reads a written span into a start and an optional end.
Every address shape the host hands out yields the same eleven-character id.
The host name is read without case and without its `www.` label.
Another host, an unknown path and a local file answer null.
An id of the wrong length or with a stray character answers null too.
The character check matters because the id is written into a page's script.
A written span reads as a start and an optional end, each in `m:ss` or `h:mm:ss` form.
A lone moment gives no end, and an unreadable start gives zero.
The player is told these moments, so a bad span must play the film rather than refuse.

## Stamp

Commit: `c37ca1837ded`
- `TImage.cs` `a00f8c19ff29160b`
- `TMeaning.cs` `e0969500a3c587ad`
- `TRegister.cs` `d96edc5045420c8b`
- `TSituation.cs` `330635d9363c2e16`
- `TSituationMedia.cs` `a05133ab1254cfa8`
- `TTag.cs` `0200895230f0ca8f`
- `TTranslation.cs` `f7b6241d9396bff9`
- `TVideo.cs` `2884e3c748bec951`
