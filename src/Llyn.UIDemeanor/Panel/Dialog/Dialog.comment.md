# Dialog

The bodies of the console's sublevel boxes that carry a panel's own content.
Each is a `BSParleyBody`, so the box owns the frame and buttons and the body owns the middle.

## `BSCustoms`

The body of the import dialog, a table of the file's entries drawn from a `CSCustoms`.
It is 118 cells wide and 11 rows high, and shows 7 entries at a time.
A row shows its number, headword, language, mode and stored target.
The mode cell and the target cell each open a drop that shows 5 labels at a time.
The modes are create, append and replace, and the labels come from the catalog.
A choice goes to the `CSCustoms` and the import button follows what it answers.
So the button is enabled only while every row is ready.
A target cell in a create row draws as `disabled` and opens no drop.
At 112 cells or more the loss text sits at the end of each row.
Under 112 cells the lit row's loss text sits on one line under the table.
Only the lit row shows lit cells, and only while the body has focus.
The position text reads `n of m` for the lit row.
The state is read from the `CSCustoms` on every draw, so the body keeps no copy.
Cell widths use the measure the box hands in, so a wide character takes the cells the terminal gives it.
The body keeps its own fixed height and ignores the height the box offers.

## `BSCustomsOmission`

The body of the omission list, a heading and one row per omission.
Each row shows the line number right-aligned in 6 cells, then the text.
It has no cells, so Up and Down only scroll its page of 9 rows.
It takes the size of `BSCustoms`, so both boxes look alike.
It has no position text.
Its rows are measured with the box's measure too.

## Stamp

Commit: `8c403d1df768`
- `BSCustoms.cs` `f3b0200db4c70517`
- `BSCustomsOmission.cs` `61d6a3e15512125a`
