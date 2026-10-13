# Diwei

This folder holds the diwei cell lists and the page of one cell.
It also holds the per-division tally of reflex parts on that page.

## `CDiwei`, `CDiweiLine`, `CDiweiPage`, `CDiweiSection`, `CTally`, `CTallyMark`

The diwei cell lists and the page of one cell.
The blank page carries empty text and no section.
The engine orders the sections and sorts the lines, and judges whether the page is empty.
Both fonts are ready to paint, the headword font on the key and the glyph font on the character lists.
A section's tallies are in the notation it chose.

## Stamp

Commit: `c2f335c331fe`
- `CDiwei.cs` `c9423f175ee0f891`
- `CDiweiLine.cs` `0b229f53dcd37ff6`
- `CDiweiPage.cs` `91d969e915b886d8`
- `CDiweiSection.cs` `758ca18d8d4174ac`
- `CTally.cs` `656e717bb7b8c142`
- `CTallyMark.cs` `ed651055b37d10f5`
