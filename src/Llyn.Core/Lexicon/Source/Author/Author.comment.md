# Author

This folder holds the Author record and the credit and byline rows the source editor shows.
Authors are independent data owned by nothing, and citing rows reach them by id.
The order an Author takes lives on each referencing row rather than on the record.

## `LAuthor`

One Author, display name only, never identity.
Renaming an Author leaves every Reference to it untouched, and two Authors reading alike remain distinct rows.
`Anonymous` is an ordinary specified Author, never a stand-in for an unspecified or unknown authorship.
Those are states on the Reference itself, in `LReferenceAuthorState`.
`LAuthorNamed` is true when the name holds more than whitespace, and an author draft without one is refused with `LRefusal.LRefusalName`.
`LAuthorStored` is false for the id zero, which names the uncredited row.

## `LAuthorRow`

One credit row of the source editor, already decided for the veneer to copy.
`LAuthorRowCreate` gives the credits in order, each with its place and its two move verdicts.
The blank row the user types a new credit into is the editor's own and is not among these.

## `LBylineRow`

One stored Author the byline dropdown offers, its name already split around the typed word.

## Stamp

Commit: `c2f335c331fe`
- `LAuthor.cs` `d04f32b02160720e`
- `LAuthorRow.cs` `5aa095a0df758003`
- `LBylineRow.cs` `6b68dcd53dc298d0`
