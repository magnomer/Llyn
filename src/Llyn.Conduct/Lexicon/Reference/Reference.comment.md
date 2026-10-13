# Reference

This folder holds the Source draft as the source editor and the colophon show it.
It carries the stated fields, kind options and credit rows.

## `CAuthorRow`

One credit of the Source draft, as the imprint lists its authors.
The position counts from zero in the draft's order.
The earlier and later flags say whether the credit can move one place.
`CAuthorRowBlank` marks the imprint's blank row, so no id value has to mean blank.

## `CColophon`

The sheet of a reference, as the colophon panel shows it.
The engine decides every faint and shown flag, so the panel only places them.
A faint flag means the field shows as a placeholder.
A shown flag means the chip or section shows.
The tally is how many examples cite the reference.

## `CReference`

The stated fields of the Source draft, as the source editor writes them into its fields.
It copies the engine's edit sheet, so every text and key is already decided.
Each hint is a resource key.
The kind tag marks its entry in the kind menu.

## `CReferenceKind`

One option of the source editor's kind menu, ready to show.
The tag is what a pick hands to `CImprint.CImprintKindSet`.

## Stamp

Commit: `c2f335c331fe`
- `CAuthorRow.cs` `bf9488280af627f5`
- `CColophon.cs` `50235a38c806148e`
- `CReference.cs` `00e2cdde24ed090c`
- `CReferenceKind.cs` `499ece224dfddca4`
