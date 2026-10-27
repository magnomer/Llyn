# Entry

Entry holds the headword record, its draft form, and the grammar that hangs off it.
That grammar covers forms, unit and freedom.
It also names the folding boxes of the entry editor.

## `LEntry`

A lexical entry, the root that owns its forms, parts of speech and every other subordinate lexical structure.
The id is an opaque program-generated identity.
The headword is never identity, so two entries with the same headword stay distinct records.

## `LEntryDraft`

The whole input form as one immutable value.
The shell reads the visual tree once, builds this, and hands it to the engine.
The engine never learns that controls exist and the save never walks the tree.
Both card lists arrive in the order they are shown, and a card's position is its place in its list.
The entry-level detail travels as the shapes the archives store, never as flattened text.
`LEntryDraftAncestors` holds the etymology map's boxes, and an entry with no etymology carries an empty list.
`LEntryDraftNames` leaves out a part stored with no name, since only its stored value would show.
`LEntryDraftCitations` lists every Source an Example cites across meanings, collocations and child cards, for one display query.
`LEntryDraftPronunciation` is the first of the list, or null, and the surfaces that show one reading read it.
`LEntryDraftAccents` holds every pronunciation after the primary one.
`LEntryDraftNormalize` walks every card and is called only after the user agreed to lose what the store could not read.
`LEntryDraftSettle` rewrites every translation link that names a tentative draft id, through child cards too.
A real id takes its place, once and only when the card does not already link it.
A real id of zero drops the link, which is how a cancelled draft leaves no dangling chip.

## `LEntryChip`

One chip of an entry's chip row and its kind.
`LEntryChipRead` owns the row's order, the one the view shows.
The unit comes first, then the freedom, then each named part of speech.
It takes the draft's unit, freedom and speeches, so each caller names the draft fields it shows.
An unchosen or unknown unit or freedom adds no chip, and a draft with nothing marked reads as no chip.
A unit or freedom chip carries its localization key from `LUnitKeyRead` or `LFreedomKeyRead`.
A speech chip carries its name.
The print sheet, the Joplin note and the DOCX and Markdown portrait all read the row here.
Each of them only words and styles it.

## `LEntryChipKind`

Whether a chip is the unit, the freedom or a part of speech.
A writer styles the first two as outlined chips.

## `LFoldBox`

Which folding box of the entry editor a fold request means, Fanqie or Script.
Each box keeps its opened state per entry, and no entry shares it with another.
The "More readings" fold of a reflex list is not a box and keeps its own members.

## `LForm`

A written form of an entry, identified by entry and position.
Reordering changes the position only.

## `LFreedom`

Whether the entry's unit can be uttered alone as a response, kept apart from the unit.
The user chooses it, and any freedom goes with any unit.
The numbers are stored in `entry.freedom`, so a member never changes its number.

## `LFreedomKey`

The text key of each freedom, written in markup, and its localization key.
One map keeps a freedom written by an export readable by the import.
Parse ignores case and surrounding space.
An unknown or missing key reads as `LFreedomEmpty`, so a foreign file never fails on it.
The editor, the reading view and the exports name a freedom through `LFreedomKeyRead`, so they never disagree.

## `LUnit`

The lexical unit an entry stands for, kept apart from its Freedom and its parts of speech.
A part of speech names a role, and the unit names what kind of piece carries it.
The user chooses it, and every language offers the same three.
The numbers are stored in `entry.unit`, so a member never changes its number.
Numbers 1 and 2 held the retired content and function units, and a rebuild clears them.

## `LUnitKey`

The text key of each unit, shared by markup and language packs, and its localization key.
Parse ignores case and surrounding space, and an unknown or missing key reads as `LUnitEmpty`.
The retired keys `content` and `function` read as unknown, so an older file leaves the unit unchosen.
The editor, the reading view and the exports name a unit through `LUnitKeyRead`, so they never disagree.

## Stamp

Commit: `1f3467bbe5c6`
- `LEntry.cs` `33570d75bb87f462`
- `LEntryChip.cs` `4e5cafe96cf4c9f2`
- `LEntryChipKind.cs` `9368a296b1bee07f`
- `LEntryDraft.cs` `7945d3dc4742680a`
- `LFoldBox.cs` `1cc4cf7e5a366082`
- `LForm.cs` `08608a643507cab0`
- `LFreedom.cs` `a9233b2e06da7c36`
- `LFreedomKey.cs` `8e12d1a272990917`
- `LUnit.cs` `c99a5412bc633ebf`
- `LUnitKey.cs` `82b5114c87f826cb`
