# Source

Source holds bibliographic References, their read and edit sheets, their citation offers, and the workspace's revision history.
References are independent data owned by nothing, and citing rows reach them by id.
The order a Reference takes lives on each referencing row rather than on the record.

## `LColophon`

The read sheet of one Source with every text the colophon page prints and every look it takes already decided.
The page writes each value into one control and branches on nothing.
A field's text is its value, the unknown mark when recorded unknown, or empty when never written.
A faint field is dressed as the placeholder the edit side shows, so the two sides read alike.
A shown field has a heading to stand over, so a blank line never stands for two facts.
The title alone is never empty, and a Source without one reads as untitled.
`LColophonCreate` takes localized texts through `localize`, because this record knows no language file.
The kind reads inside its chip when one was recorded, an unknown kind included.
The authorship reads the credited names in order, or the unknown mark when recorded unknown with no credit.

## `LImprint`

The edit sheet of one Source, with every text and key the imprint fields show already decided.
The fields write each value into one control and branch on nothing.
A value recorded unknown shows no text, and its hint turns to the unknown mark instead.

## `LReference`

One bibliographic Reference, the material an Entry or an Example cites.
No Entry, Example or Author owns it, and it owns none of them.
Entries reach it through ordered reference rows and an Example through its single source column.
Deleting a citing row never touches the Reference, and deleting the Reference never touches its citers.
One shape describes an article, a television episode, a video or a picture alike.
The fields that mean something for the material are specified, and the rest stay unspecified.
No code branches on the kind to choose which fields a Reference may carry.
A kind that decided the shape would rebuild one record per kind, which is what this record exists to avoid.
Whatever a kind alone cannot say goes in the note, in the user's own words.
Each text field is an `LStateValue` so never entered, recorded as unknown and this value stay distinct.
`LReferenceAuthorState` is that same distinction for the authorship as a whole.
The Authors themselves are separate rows attached in order.
`LReferenceTitleHint` is the unknown mark while the title is unknown, else the field's own key.
The year, url and note hints read the same way.
`LReferenceKindCheck` counts an unknown kind as stated, since the user chose it.
`LReferenceNameRead` is the first field the Reference states, title then url.
The note is left out because a memo is not a name.
A Reference that names itself nowhere is shown under its id, never as an indistinguishable blank.
The rule lives on the record because the catalog, the row and the citation list all need the same answer.
`LReferenceCreditRead` never falls back to the title or adds the year, unlike the byline.
`LReferenceAuthorCheck` is answered by attached Authors or an authorship the user marked unknown.
`LReferenceBylineRead` writes `Author (Year)` with every credited name in order, joined by a comma.
A Reference crediting nobody is written under its name instead, so the line is never blank.
The year follows only when it is stated.
The example line, the citation field and its dropdown all write the same line, so the rule lives here.
`LReferenceColophonRead` is composed on the record so the page passes the Source nowhere.
`LReferenceKindMenu` puts unspecified first and unknown last, unlike the enum order, so every medium lists the kinds alike.
`LReferenceKindResolve` is the one localization key for a kind across the read sheet, the print legend and the filter.
`LReferenceKindFormat` writes the stored word rather than a number, which would tie the file to member declaration order.
`LReferenceKindShow` writes no leaf for unspecified and an empty leaf for unknown.
`LReferenceKindParse` reads an unknown word as unspecified rather than failing, so a workspace from a later build still opens.
`LReferenceNormalize` is called only after the user agreed to lose what the store could not read.

## `LReferenceKind`

What kind of material a Reference stands for, a label the user reads, sorts and filters by.
Every Reference holds the same seven fields whatever kind it names.
Unspecified and Unknown are members here rather than sitting behind an `LStateValue`.
The set of kinds is closed, so the states fit as two more members.
That keeps one column in the database where a state and a value would need two.
`LReferenceKindOther` is the home for material no named kind fits, and the note says what it was.

## `LReferenceOffer`

The stored Sources a sentence's citation field offers for the text typed into it.

## `LReferenceRow`

One stored Source a citation field offers, its byline already split around the typed word.

## `LRevision`

A stamped point in the workspace's history under which a batch of changes was recorded.
It owns its `LRevisionDelta` rows and the tombstones written under it.
It owns no lexical data, and deleting lexical data never deletes a revision.

## `LRevisionDelta`

One recorded change inside a revision, with no id of its own.
The store keeps its order, so the changes read back in the sequence they were recorded.
The target and subject name what was touched, as plain recorded values rather than a foreign key.
A change routinely describes a row that no longer exists.
So a deletion is still readable after the row is gone.

## Stamp

Commit: `c2f335c331fe`
- `LColophon.cs` `3907302f9a77bfca`
- `LImprint.cs` `d4089ba314089697`
- `LReference.cs` `cff39ef01b5f86a1`
- `LReferenceKind.cs` `152a58f71b7908e1`
- `LReferenceOffer.cs` `732025f4c3e88fe3`
- `LReferenceRow.cs` `1d28a386f96d681f`
- `LRevision.cs` `ba4120f37cbc1893`
- `LRevisionDelta.cs` `43e38e48b2297841`
