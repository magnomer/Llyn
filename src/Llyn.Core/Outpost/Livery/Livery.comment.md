# Livery

This folder holds the platform-free models of the notes the Joplin push renders.
The writers live in Infrastructure behind `LEngraverEntrance`, so Core names no markup.

## `LLiveryNote`

One rendered Joplin note, with the pictures its body names.
The body and its parcels travel together, so no note links to a resource nobody uploads.
The body is HTML that Joplin keeps inside Markdown.
There is one parcel per distinct picture, each named in the body by its id.

## `LLiveryPage`

The stored data for one entry as view mode shows it.
`LLiveryClerk.LLiveryClerkBuild` constructs it from the answers `LLiveryFacade.LEngineLiveryRead` gathers.
The outpost batch hands one per entry to `LEngraverEntrance.LEngraverEntranceFormat`.
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
The engraver renders every series and category note from it.
Series come in name order and are empty without a series rule.
Categories come as initials, then rimes, then tones, and are empty without a fanqie book.

## Stamp

Commit: `1f3467bbe5c6`
- `LLiveryDiwei.cs` `cead8ec76bdbd658`
- `LLiveryLanguage.cs` `8bfba9621043bb14`
- `LLiveryNote.cs` `82844d79aa7a10fe`
- `LLiveryPage.cs` `c7a3113cdbef0002`
- `LLiveryStem.cs` `94c0be6ac23776ec`
