# Rehearsal

These tests pin the rules and workflows of flashcard decks.
Each Core rule test drives one rule through a relay, with no workspace.
The stencil clerk tests save entries in a workspace, since a face reads stored rows.
The deck and cue clerk tests run over a real database.
Only SQLite enforces the deck place and the card grain checks.

## `TCue`

Covers when a card is orphaned and when it rests.
A meaning card whose meaning is gone is orphaned, and a free card never is.
A pair needs its target and one of its meaning or collocation.
An excluded card rests even when nothing is gone.

## `TCueClerk`

Covers the read, scan, add, face and cell edit of cards, and the stencil read, slot set, save and apply.
A deck's card read returns only that deck's cards.
A scan returns every meaning at any depth, and one pair per translation target.
A second add of the same entry adds nothing.
A free card reads back its saved cells, and a bound card refuses them.
A face read by card id resolves against the deck's own template.
An unknown card id refuses.
A new deck's template reads as its grain's default.
A slot set replaces a held slot in place and appends a new one.
A new deck owns an unsaved template and two bearings.
An applied stencil copies its slots, and a saved stencil keeps them through later edits.

## `TCueMarkup`

Covers the reduction of Markdown to the phone's markup.
Bold and italic become phone tags, and links and code keep their text.
Paragraphs and line breaks join with a bar, and a rule is dropped.
A literal bar is escaped in plain text and in Markdown alike.

## `TDeckCode`

Covers the codes that keep deck file names apart.
A taken name gains ` 2`, then ` 3`.
A match ignores case, non-ASCII letters included.

## `TDepotFolder`

These tests pin the phone file format the folder adapter writes.
Each writes a sheet into a fresh workspace folder and reads the bytes back.
The expected text is literal, so a quoting or order change shows at once.
The backup test plants a phone backup under `Backups/` and proves the read never reaches it.

## `TDeckManifestFile`

This test pins the manifest file store over a real keep file.
A save then a read gives back every deck's digest, media names and time written.
The media names read back sorted, since the store writes them in ordinal order.

## `TDepotLatch`

These tests pin the export latch each workspace holds.
Two engines over two workspaces hold two latches, so a run in one leaves the other idle.
A started latch refuses a second start until it finishes.

## `TDepotClerk`

These tests pin the deck export over a fake depot and a fake manifest.
A deck file the manifest never wrote is refused and left as it was.
Two sources with one content name are copied once.
Only a name the manifest owns is deleted, so a phone's own file stays.
An http picture is written as its address and copies nothing.
A started latch refuses a run, and another workspace's latch blocks nothing.
An orphaned cue never becomes a row.

## `TDeckClerk`

Covers the deck and folder workflows over a real database.
A folder delete appends its decks at the top level, even where places would clash.
A rename keeps the code, and a taken name suffixes a new deck's code.
An update refuses a code another deck holds in another case.
An update that keeps the deck's own code saves.

## `TStencilClerk`

Covers the build of one card face from a one-slot template.
The join rule joins every value with a bar, and an index picks that value.
A meaning's image fills the picture, and an entry's recording fills the sound.
Both come back as rooted paths.
A pair writes its target while the link stands.
A pair whose link is gone writes an empty target and stays unorphaned.
A video alone fills no picture.
The hand-built meaning and pair cards carry no entry id, as the schema allows only an entry card one.
So they reach their entry through the bound row.
One test per bound grain stores the cue through a new deck and resolves it against that deck's template.
So each proves the holder lookup over a real database and the default template together.

## `TStencilDefault`

Pins the default template of each grain, prompt on side 1 and answer on side 2.
A free grain gets no slots.

## `TStencilToken`

Covers the parse of a slot body.
Literal text and field tokens split apart, and an index or the all mark is read.
An unknown key, a zero index, two suffixes or a stray brace stay literal.

## Stamp

Commit: `aae5349dbb94`
- `TCue.cs` `fb984bcfc6fa43be`
- `TCueClerk.cs` `2482063704eefa59`
- `TCueMarkup.cs` `2c0c1a28489dafd0`
- `TDeckClerk.cs` `11940141f47aaa8e`
- `TDeckCode.cs` `97c6a001d711f683`
- `TDeckManifestFile.cs` `fe2b883882b6283f`
- `TDepotClerk.cs` `dbb9715cd0ab95d8`
- `TDepotFolder.cs` `cb913707dba4edfb`
- `TDepotLatch.cs` `3aad50a929a35ba7`
- `TStencilClerk.cs` `34eb8cb057ced205`
- `TStencilDefault.cs` `fad4ab50915daad1`
- `TStencilToken.cs` `b8ac3e335ed8a6b0`
