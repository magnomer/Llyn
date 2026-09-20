# LParadigmRow.cs

## `public sealed record LParadigmRow(`

One row of the paradigm table: the slots that share a part of speech and an inflection text.
The engine groups the slots, so the shell only draws the rows it is handed.

**Parameters**

- `LParadigmRowSlots` — The slots the row gathers, in the order the paradigm lists them.
- `LParadigmRowLead` — True when the row opens a new part of speech and the table has more than one.

## `public LParadigmSlot LParadigmRowFirst => LParadigmRowSlots[0];`

The slot the row's inflection and state are read from.

## `public string LParadigmRowName =>`

The morphology names of the row, joined for the label column.

## `public string LParadigmRowPart =>`

The part of speech printed at the head of the row, or empty when the row does not lead.

## `public static IReadOnlyList<LParadigmRow> LParadigmRowScan(IReadOnlyList<LParadigmSlot> slots)`

Cuts the slots into rows wherever the part of speech or the inflection text changes.

## `private static LParadigmRow LParadigmRowCreate(List<LParadigmSlot> row, int parts, long? previous)`

Marks the row as leading when the table holds several parts and this one differs from the last.
