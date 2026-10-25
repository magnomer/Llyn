# Livery

This folder holds the livery port and the platform-free models of the notes it renders.

## `LLivery`

The port for how an entry looks in Joplin, so Joplin shows an entry as view mode does.
Its adapter owns the theme, the rules, the page markup and the hashing.
The engine pushes the bodies and never learns how they are written.

## `LLiveryRead`

The whole body of the style note, as Markdown.
A line warns that Llyn writes the note and overwrites edits, so nobody tunes it by hand in vain.
The CSS follows in a fenced `css` block, which Joplin applies to the notes that import it.
The CSS never holds three backticks, so the fence cannot close early.

## `LLiveryFormat`

The entry overload writes one note body as Markdown from the stored data view mode shows.
The `note` function maps an entry id to its Joplin note id.
The `link` function maps a language, a kind and a key to the id of a reconstruction note.
It answers empty for a note this push does not write, so that chip stays plain.
The `lookup` function maps a localization key to the text view mode shows.
The body imports the style note by id, so a theme change touches one note instead of every entry.
Pictures leave the body as parcels, since Joplin shows images only as its own resources.
The series and rime-table overloads import the same style note and open with the same mark.
So the trash check proves those notes as Llyn's own too.

## `LLiveryMarkFormat`

The first line of every note body Llyn writes, the import of the style note.
A note in Llyn's notebook whose body starts with it is proven to be Llyn's own.
So only such a note may ever be trashed.

## `LLiveryIdFormat`

A fixed Joplin id from a seed, the first 32 hex characters of the SHA-256 of its UTF-8 bytes.
The same seed always yields the same id, so each push overwrites the same note.
The realm in an entry's or reconstruction note's seed separates independently created workspaces.
A folder copied outside Llyn keeps it and pushes onto the same notes.

## `LLiveryDigestFormat`

Hashes everything a note carries into Joplin, so any change to it pushes the note again.
The answer is the whole SHA-256 in lowercase hex, which the manifest keeps per note id.
Tags are sorted here, so their order never forces a push.

## `LLiveryNote`

One rendered Joplin note, with the pictures its body names.
The body and its parcels travel together, so no note links to a resource nobody uploads.
The body is HTML that Joplin keeps inside Markdown.
There is one parcel per distinct picture, each named in the body by its id.

## `LLiveryPage`

The stored data for one entry as view mode shows it.
`LLiveryClerk.LLiveryClerkBuild` constructs it from the answers `LLiveryFacade.LEngineLiveryRead` gathers.
`LCourierNote.LCourierEntrySend` hands one per entry to `LLivery.LLiveryFormat`.
The guises hold one guise per reflex of the draft in draft order.
`LLiveryPageFold` holds folded cards and `LLiveryPageTarget` holds translation targets, both keyed by kind and id, and `LLiveryPageFolded` holds reflex languages.
A fold toggle changes the note, so the next push re-sends it.
`LLiveryPageInflection` is null when the language has no inflection layout.
The paradigm rows drop the layout's part of speech, so only that table carries those forms.
A language with no stored flag file has no entry in the banner map.
`LLiveryPageShown` holds one entry per paradigm row, in the same order.
Each is the shown text and tip the view paints for that row, never held.
The frequency rows are the stored ones, read with fetch off.
`LLiveryPageLineage` holds the roots of the etymology tree, empty when the entry has no etymon.
`LLiveryPageRelation` lists the relations, so a box's relation id finds its key or name.

## `LLiveryStem`

One phonetic series of a language, as the Joplin push shows it in a series note.
The entries are for that series alone, read with an empty query.
`LLiveryStemKind` is kept apart from the `LDiwei` kinds so links never collide.

## `LLiveryDiwei`

One rime-table category of a language, as the Joplin push shows it in a category note.
The kind is an initial, a rime or a tone.
The entries are for that category alone, read with an empty query.

## `LLiveryLanguage`

The whole reconstruction side of one language, the platform-free model of its Joplin notes.
The Joplin push renders every series and category note from it.
Series come in name order and are empty without a series rule.
Categories come as initials, then rimes, then tones, and are empty without a fanqie book.

## Stamp

Commit: `36e31eb71c98`
- `LLivery.cs` `791b17789a18977b`
- `LLiveryDiwei.cs` `cead8ec76bdbd658`
- `LLiveryLanguage.cs` `8bfba9621043bb14`
- `LLiveryNote.cs` `82844d79aa7a10fe`
- `LLiveryPage.cs` `c7a3113cdbef0002`
- `LLiveryStem.cs` `94c0be6ac23776ec`
