# Workspace

This folder holds the ports for what belongs to the workspace as a whole.
It covers authors, references, settings, workspace state, language packs and interface catalogs.
Each interface lists only what the engine asks, and nothing about how it is kept.
Database ports have an `L<Name>Archive` adapter in Infrastructure.
The other ports have loader adapters named in their sections.
The folder stands apart from the Sound ports because none of this is tied to one entry's phonetics.

## `LAuthorVault`

Persistence for Author rows, with `LAuthorArchive` as adapter.

### `LAuthorCreate`

Inserts the author with a fresh opaque id and returns it with that id filled in.
The new Author is credited on no Reference until one attaches it.
The name may be empty, because section 6 of the format spec lets an author carry none.
An import writes such a row, and the engine refuses one typed by hand.

### `LAuthorRead`

Reads one Author by id, or null when none exists.

### `LAuthorReferenceRead`

The read by reference gives the Authors a Reference credits, in the order that Reference gives them.
The order lives on the association row, so another Reference may order the same Authors differently.
The read without an argument gives the credits of every Reference at once, each in its own order.
A catalog showing credits on every row would otherwise run one query per row.

### `LAuthorAllRead`

Reads every Author the workspace holds, by name.
The Sources panel offers them for crediting, so it asks for the whole shelf at once.

### `LAuthorUpdate`

Rewrites the name of the Author with the given id.
The id and every Reference crediting it are untouched, so a rename never changes where the Author appears.
It throws when no Author carries that id.

### `LAuthorAbsorb`

Folds the dropped Author into the kept Author and deletes the dropped row.
Every Reference crediting the dropped Author credits the kept one afterwards, at the same position.
A Reference already crediting both keeps the kept credit where it stood and loses the duplicate.
The remaining credits of each touched Reference are renumbered, because the order is a unique index.
The kept Author is inserted past every live position first, so the renumbering never collides.
It throws when either id names no Author or both name the same one.
The whole fold is one transaction, so no Reference is left crediting a deleted Author.

### `LAuthorDelete`

The same delete, with the detach flag dropping every credit first.
Dropping a credit renumbers that Reference's remaining credits, because the order is a unique index.
Deleting an Author never deletes a Reference.

### `LAuthorUsageRead`

Reads the Meanings, Collocations and Examples citing any Source the Author is credited on.
A card names its own id, the Entry it belongs to, that Entry's headword, and its title.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card is listed once however many credited Examples it holds, because the row leads to the card.
The count of an Author is the Example rows, which are the citations themselves.
The card rows are the way up to those citations rather than additions to them.
Counting cards and Examples together would count one citation twice, as `LReferenceUsage.comment.md` records.

## `LLanguageVault`

The port for language packs, the per-language definitions the build ships beside the program.
The engine asks by language name and never learns where a pack sits or how it is written.
`LLanguageLoader` is its adapter over the `languages` folder.

### `LLanguageScan`

Gives the names of every listed pack, the primary language first and the rest in ordinal order.

### `LLanguageRead`

Gives the pack of the language, or a blank pack of that name when none is readable.

### `LLanguageNameValidate`

Tells whether a name could name a pack at all.
A name that would escape the pack folder, or name no folder, is refused before any pack is looked for.

### `LLanguageFlagRead`

Gives the local path of the flag image for an ISO 3166-1 alpha-2 country code.
The adapter fetches it into the workspace on first use and serves the cached copy after.
It answers null when the fetch fails or the flag file cannot be written or read.
So a missing flag never blocks the UI.

### `LLanguageFlagFind`

Gives the local path when the flag file is already stored, else null.
It never fetches and never writes, so a caller that must not touch the network can ask it.

## `LLocalizationVault`

The port for interface-language catalogs, the texts the program shows in one language.
`LLocalizationLoader` is its adapter over the catalogs embedded in the build.
The engine and the bootstrap read a catalog through it and never learn where it is kept.

### `LLocalizationScan`

Gives the languages the build carries a catalog for, in ordinal order.

### `LLocalizationRead`

Gives the raw pairs of the catalog, `terms.*` keys beside the text keys, references still unresolved.
The adapter owns the file format, so the ring above resolves the pairs and never sees the JSON.
A language the build carries no catalog for raises an exception of the adapter's own.

## `LReferenceVault`

Persistence for Reference rows, with `LReferenceArchive` as adapter.

### `LReferenceCreate`

Inserts the reference and returns it with its id filled in.
A Reference carrying no id is given a fresh opaque one.
A Reference already carrying one keeps it, because a held source is named before the record it becomes exists.
Each field is written as its state plus, for a specified field alone, its value.
The new Reference is cited by nothing and credits no Author until one is attached.

### `LReferenceRead`

Reads one Reference by id, or null when none exists.

### `LReferenceAllRead`

Reads every Reference the workspace holds, by title.
The Sources panel browses the whole shelf, so it asks for it at once.

### `LReferenceUpdate`

Rewrites every field of the Reference with the given id, states and values alike.
The id, the Authors credited, and every citation pointing at it are untouched.
So an update never changes where the Reference appears or in what order.
It throws when no Reference carries that id.

### `LReferenceDelete`

The same delete, with the detach flag clearing every citation first.
The detaching, the count, and the delete stand in one session.
Between any two steps the answer to whether anything still cites it can change.
The guard and the delete share one transaction, so nothing can cite the Reference between them.
An Example is not deleted with the Reference, so its citation is cleared back to Unspecified.
Only the links between them and this Reference go.

### `LReferenceAuthorAttach`

Credits an existing Author on this Reference at a position in the Reference's own author order.
The Author row itself is untouched and stays available to every other Reference.

### `LReferenceAuthorDetach`

Removes this Reference's credit for an Author.
The Author and its other credits survive.

### `LReferenceUsageRead`

The read without an argument counts the citations of every Source at once.
It serves the catalog, the ordering, and the delete.
A Source nothing cites is absent from the map rather than present at zero.
The unit is the citing Example, which is the only thing that cites at all.
It is not the number of rows the itemized read returns.
That list also names each citing card as the way up to the citation.
Counting cards and Examples together would count one citation twice, from two directions.
So the badge showing this number says what it counts, through `Source.Tally`.
The Example rows of the list are what it agrees with.

The read by id itemizes the citing Meanings, Collocations and Examples of one Source.
Each row carries the id it leads to.
A card names its own id, the Entry it belongs to, and that Entry's headword.
It also names its title with the definition or expression behind it.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card holding two Examples of one Source is listed once.
The row leads to the card rather than the citation.

## `LSettingsVault`

The port for the user's engine settings, with `LSettingsLoader` as adapter over `settings.json` inside the workspace.
The engine reads once per workspace and saves on every change.

### `LSettingsExist`

Tells whether the workspace has stored settings of its own.
A workspace without them inherits the settings of the one left behind.

### `LSettingsRead`

Gives the stored settings, or the defaults when none are stored or the stored ones are unreadable.

### `LSettingsSave`

Stores the settings, replacing whatever was there.
A store that cannot be written raises `LVaultFault`.

## `LWorkspaceVault`

The port for the single workspace state row, with `LWorkspaceArchive` as adapter.

### `LWorkspaceStateRead`

Reads the workspace state, creating a blank one when none is stored yet.

### `LWorkspaceStateSave`

Stores the state as the workspace state.

### `LWorkspaceFloorAdjust`

Issues the next provisional id by lowering the identity floor by one and answering the new floor.

### `LWorkspaceSizeRead`

Gives the bytes the workspace store occupies, or zero when nothing is stored yet.
The establishment panel shows it, and only the adapter knows what file that is.

### `LWorkspaceRealmRead`

Gives the realm the workspace was created under, fixed for the life of its store.
The realm separates independently created workspaces.
A folder copied outside Llyn keeps it and pushes onto the same notes.

## Stamp

Commit: `c2f335c331fe`
- `LAuthorVault.cs` `58991b28a7c9254c`
- `LLanguageVault.cs` `dccc78c74f94d252`
- `LLocalizationVault.cs` `e00de5956dc8719e`
- `LReferenceVault.cs` `875f0706a5b64d06`
- `LSettingsVault.cs` `ba91401d0d4265e4`
- `LWorkspaceVault.cs` `ef9ab7fcfdfee9d2`
