# LVistaPort.cs
Hash: `d5a47c43ab2b4aea`

## `public interface LVistaPort`

The slice of the engine a deportment sees when it lists entry rows.
`LVistaFacade` implements it, since every entry list runs through a vista.

## `IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista);`

The entry rows the vista lists, with its query, order and filter applied.
A blank vista with nothing typed answers no rows.

## `IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);`

The entry rows of a child list narrowed by the parent catalog's choice, as the footnote and cohort lists read.

## `IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);`

The labels made distinct in their given order, numbered where two share a name.

## `LDraft? LEngineVistaLoad(LVista vista);`

The vista's chosen record as a read-only snapshot, or nothing once the choice no longer loads.
A stored choice that no longer loads is deselected.

## `LDraft? LEngineVistaLoad(LVista vista, long? id);`

Selects the row and loads it, keeping the prior choice when the load throws.

## `string LEngineFileRead(LVista? vista);`

The file name an export of the vista's entry is offered under.

## `void LEngineSideSave(LVista vista);`

Stores the vista's chosen entry as the duplex side its tab names.

## `int LEngineUsageRead(LVista vista);`

How many places a delete of the vista's chosen record would drop.

## `string LEngineTallyRead(LVista vista);`

The usage tally line of the vista's chosen Example, Situation or Source.
Any other subject is a caller mistake and throws.

## `LRevision? LEngineVistaDelete(LVista vista);`

Deletes the vista's chosen record and clears the choice.
It answers the revision of an entry delete, and nothing for the other subjects.
A vista with no choice, or a subject it cannot delete, deletes nothing.
