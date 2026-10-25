# Entry

This folder holds the slices that load and show one stored entry.
They cover its etymology map, card links and folds, grasp, markdown note, Mentions and example sentence.

## `LCardPort`

The slice for a shown entry's card links and stored card folds.
`LCardFacade` implements it.
Incoming links include both card kinds, with epithets chosen by settings.
Top-level meanings and collocations each receive a translation key, even without resolved targets.
Missing targets are omitted rather than rendered as blank chips.
`LEngineCollocationRead` places each collocation of the shown draft for the reading view and its compass.
Homes, stubs, the other list and each row's phrase and markers come in one answer.
So no reader applies the link rules itself.
The caller hands in the translations it already read, so the flags cost no second read.
The compass shows no flags, so it hands in an empty map.
One entry-scoped fold read covers folded Meanings and Collocations together.
A fold is keyed by the card's kind and id together.
`LEngineCardFind` says which card list of the draft holds the card of a kind and id.
It also says at which place, or null when it is in neither.
It reads only the draft the caller holds.
Card folds remain outside lexical edits and undo.
Save and delete announce a fold bulletin for the entry on normal completion.
This holds even when a nonpositive card id writes nothing.
Views of that entry can then re-read their folds.

## `LAncestorPort`

The slice a deportment sees for an entry's etymology map.
`LAncestorFacade` implements it.
The Lineage read takes the shown draft's boxes, so the tree follows edits not yet saved.
Relations and user tongues carry usage counts, and the app's languages come before the user's tongues.
Creating a relation or tongue answers the held value when the name exists, ignoring case.
A delete answers the count of boxes that held the value, or null when nothing was deleted.
A builtin relation is never deleted.
The caller asks for confirmation beforehand, since the count is only known by reading the list.

## `LEntryPort`

The slice a deportment sees when it loads one stored entry.
It loads the entry, reads its stamps, its Source tally and its written reflexes.
It also opens the entry a glyph cell names and words a lexical unit.
`LEntryFacade` implements it itself, so no outlet forwards between Conduct and the engine.
Member names keep the engine's `LEngine*` form.
`LEngineEntryLoad` gives the stored entry as a draft, or null when it no longer stands.
Its recordings come resolved to absolute paths.
`LEngineStampRead` says whether an entry is stored, with its worded creation and update times, empty once it is gone.
The caller passes the culture, read from the interface-language owner `LEngineCultureRead`, never the machine's.
The engine parses and words the stored stamps, so Conduct only copies them.
`LEngineTallyRead` words the usage tally of one Source, as none when no Source is given.
`LEngineGlyphResolve` gives the id of the entry a glyph cell opens, made when none exists yet.
`LEngineUnitFormat` and `LEngineFreedomFormat` give the localization key that names a unit or freedom, empty when none is chosen.
They are static, since the key depends on no workspace, and the unit and freedom clerks own the wording.

## `LGraspPort`

The slice a deportment sees when it rates how well an entry is known.
`LEntryFacade` implements it, since the grasp is stored on the entry.
`LEngineGraspStep` is the last grasp step, the limit a star control draws to.
`LEngineGraspRead` runs from zero to the last step, and a missing entry reads zero like one never rated.
`LEngineGraspSave` writes the grasp, then raises a Grasp bulletin for the entry.
`LEngineStarScan` passes `LGraspClerk.LGraspClerkScan` on unchanged.
Application builds the star answers, so the port calls no Core method.

## `LMarkdownPort`

The slice a deportment sees when it shows a note written in markdown.
`LDraftFacade` implements it.
A missing note parses to no blocks.

## `LMentionPort`

The slice a deportment sees when it opens a Mention or a link.
`LMentionFacade` implements it.
`LEngineMentionFind` gives the Mention under an offset of a stored Example.
It also gives the Mention under an offset of one sentence of the shown entry, named by its line key.
A line key outside the shown entry's sentences answers the bare offset.
`LEngineLinkRead` gives the entry a clicked link names, or null when it names none.
A link names its entry only through a stored id, so an empty id names nothing.

## `LSentencePort`

The slice a deportment sees when it builds an example sentence in a language.
`LVocabularyFacade` implements it.
The word order comes from the language's pack.
The dependence labels and particles are those already saved in the language's example sentences.

## Stamp

Commit: `8c403d1df768`
- `LAncestorPort.cs` `7e9777eb958ef99c`
- `LCardPort.cs` `4754481e29621576`
- `LEntryPort.cs` `45ee82ac8d86cff5`
- `LGraspPort.cs` `9099a47b67179e94`
- `LMarkdownPort.cs` `d966d5c1e2a4e15c`
- `LMentionPort.cs` `7a0a2aadff6233b5`
- `LSentencePort.cs` `8284e49bd11eb19f`
