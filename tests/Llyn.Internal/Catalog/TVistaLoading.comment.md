# TVistaLoading.cs

## `public sealed class TVistaLoading`

Loading and deleting follow the selected row's kind.
Entry views open or delete the chosen Entry.
Catalog views load their own subject without opening an editing draft.
Deletion detaches the view before removing supported subjects.
Missing or cleared selections return no draft.
Structural views never mistake their selection for an Entry.

## `Delete_StructuralVista_DoesNotTreatItsSelectionAsAnEntry(string tab)`

Structural `yunjing` and `yunmu` selections have no Entry subject, draft, or deletion target.
The underlying Entry remains stored.

## `FileRead_NoVista_ReturnsEntry()`

Without a selected view, file-name generation uses the generic Entry name.

## `FileRead_ChosenEntry_ReturnsCleanedHeadword()`

A selected Entry supplies the file name.
Unsafe characters are replaced without changing the Entry.

## `Load_EntryVista_UsesChosenAndDeleteClearsIt(string tab)`

Entry views return a draft only for a valid choice.
Deleting it clears the choice and removes the Entry.
Stale or cleared choices load nothing.

## `Load_CatalogVistas_ReturnTheirOwnSubjectWithoutStartingAnEditingDraft()`

Each catalog view loads a draft for its own Example, Situation, Source, Author, Tag, or Register.
The draft has identity zero rather than an editing session.

## `Load_CatalogMissingOrCleared_ReturnsNull(string tab)`

An unknown catalog identity and a cleared selection both produce no draft across all catalog view kinds.

## `Delete_CatalogVistas_DeleteTheirOwnSubjectAndDetachFirst()`

Deletable catalog views remove their own Example, Situation, Source, or Author.
An attached Example mention and the unrelated Entry remain valid.

## `Delete_TagAndRegisterVistas_DeleteNothing()`

Tag and Register views do not delete their selected rows.
The choice and stored row remain available.

## `private static LRevision? TVistaDeleteRun(LEngine engine, string tab, long id)`

Selects a row and attempts deletion.
Checks that the view clears its choice afterward.

## `private static LDraft TVistaLoadRead(LEngine engine, string tab, long id)`

Loads a selected catalog row and verifies the result is a newly materialized, non-editing draft.
