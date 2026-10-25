# Catalog

This folder holds the slices a browsing panel lists and creates its stored records through.
They cover entries, Authors, Sources, Examples, favourites, Registers, Situations and Tags.

## `LAuthorPort`

The slice the authors panel and the source editor see when they read or fold Authors.
`LAuthorFacade` implements it.
`LEngineRollFind` lists the Authors of the roll, and none while the vista is missing.
With no query typed, the uncredited row heads them when some Source has no Author.
`LEngineUnionFind` lists the Authors the roll's Author may be folded into, matched by the typed text and capped.
`LEngineUnionRead` gives the held draft's written Author name and the kept Author's name for the union question.
`LEngineAuthorAbsorb` folds the dropped Author into the kept one and deletes the dropped row.
The credits of the dropped Author move to the kept one, and observers hear both ids.
`LEngineOeuvreFind` lists the Sources of the oeuvre, and none while either vista is missing.
`LEngineCreditRead` gives the credit rows of the held Source draft, after its deferred requests are applied.

## `LExamplePort`

The slice a deportment sees when it lists Examples or shows a sentence line.
`LExampleFacade` implements it.
`LEngineExampleFind` lists the Examples the vista shows, named distinctly, with wording for blank fields.
`LEngineTextMatch` says whether a field already shows the text, a blank field reading as nothing recorded.
`LEngineLineRead` gives the frame, the sentence divided around its Mentions and the Source line of one sentence row.
It also answers the row's line key against the shown entry, which the caller passes in.
The clerk composes them, so the display and the portrait share one rule.

## `LFavoritePort`

The slice a deportment sees when it lists or marks favourite entries.
`LCatalogFacade` implements it.
The find applies the vista's query, order and filter.

## `LReferencePort`

The slice a deportment sees when it lists Sources or reads their sheets and citations.
`LReferenceFacade` implements it.
The vista find applies the shelf's query and order, and the plain find lists every Source ordered by author.
`LEngineCitationRead` gives the citation line of every Source the shown entry cites, child cards included.
A Source that is gone reads as its bare id.
The draft overload gives the line of the Source the draft's own Example cites.
It is empty for no draft or no citation.
`LEngineColophonRead` gives the read sheet of a Source draft, with the usage tally of the Source it holds.
A draft that holds no Source is a caller mistake and throws.
`LEngineImprintRead` gives the edit sheet of a Source draft, or of a blank Source with no draft.
`LEngineKindRead` gives the kind menu of the source editor, each option's tag and key, in menu order.

## `LRegisterPort`

The slice the tenor panel sees when it lists or creates registers.
`LCatalogFacade` implements it.
`LEngineRegisterCreate` answers the register of the typed name, creating it when none matches.

## `LSituationPort`

The slice the atlas panel sees when it lists situations.
`LSituationFacade` implements it.
The find names the situations distinctly, with wording for blank fields.

## `LTagPort`

The slice the taxonomy panel sees when it lists or creates Tags.
`LCatalogFacade` implements it.
The find does not apply the vista's language filter, since it hides entries, not Tags.
A vista whose chosen Tag no longer answers is deselected.
`LEngineTagCreate` answers the Tag of the typed text, creating it when none stands.

## `LVistaPort`

The slice a deportment sees when it lists entry rows.
`LVistaFacade` implements it, since every entry list runs through a vista.
The port starts no vista, so the shell's posture starts each one through the facade.
`LEngineEntryFind` applies the vista's query, order and filter, and a blank vista with nothing typed answers no rows.
The parent and child form narrows a child list by the parent catalog's choice.
The footnote, cohort, membership, occurrence and quotation lists are read this way.
`LEngineNameResolve` makes labels distinct in their given order, numbered where two share a name.
`LEngineVistaLoad` gives the chosen record as a read-only snapshot, or nothing once the choice no longer loads.
A stored choice that no longer loads is deselected.
The id overload selects the row and loads it, keeping the prior choice when the load throws.
`LEngineFileRead` gives the file name an export of the vista's entry is offered under.
`LEngineSideSave` stores the chosen entry as the duplex side its tab names.
`LEngineUsageRead` counts the places a delete of the chosen record would drop.
`LEngineTallyRead` words the usage tally of the chosen Example, Situation or Source.
Any other subject is a caller mistake and throws.
`LEngineVistaDelete` deletes the chosen record and clears the choice.
It answers the revision of an entry delete, and nothing for the other subjects.
A vista with no choice, or a subject it cannot delete, deletes nothing.

## Stamp

Commit: `44d0e95bcfcc`
- `LAuthorPort.cs` `6f38609535b6a579`
- `LExamplePort.cs` `b6c72ca28cde3957`
- `LFavoritePort.cs` `3c243051779a80d9`
- `LReferencePort.cs` `516f5f4bb2486985`
- `LRegisterPort.cs` `9935bb59c37ef592`
- `LSituationPort.cs` `240c79bb9942082b`
- `LTagPort.cs` `da3c61686b209828`
- `LVistaPort.cs` `d5a47c43ab2b4aea`
