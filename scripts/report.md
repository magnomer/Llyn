# Audit console report grammar

This file is the single layout every audit script in `scripts/` prints to the console.

## Order

The report runs from the widest view to the narrowest.
An audit prints the levels it has in this order and skips the ones it lacks.

| Level | Content |
|---|---|
| Title | `AUDITX GENERATION N` in cyan. |
| Scope | One `Scanned:` line in dark gray naming what the run read. |
| Counters | The `Counters` section with every gate number. |
| Group | Totals per folder, project, kind, reason, pair or verdict. |
| File | Rows per file. |
| Item | Single lines, names, members or hits. |
| Reports | One `Report: <path>` line per written file. |

## Sections

A blank line comes before every section heading and before the report lines.
A heading is underlined with dashes of its own length.
A list section heading ends with ` (N)`, where N counts every row, shown or not.
A summary or ranking section heading carries no count.
A group table that splits a whole ends with a `Total` row.
A list section with nothing to list is left out, since its counter already reads 0.
A section cut short ends with `... and N more in the report.`
Every line after the scope line prints in the default colour.

## Counters

Every counter is a gate, so a done run shows every counter at 0.
The audit exits 1 when any counter is above 0 and 0 otherwise.
A counter row is the label, padded to the widest label, two spaces, and the number.
A counter that lists findings carries the same label as its finding section.
A number that gates nothing belongs in the scope line or a group section.

## Tables

Columns are split by two spaces and nothing starts indented.
A dashed rule sits under the header and matches each column width.
Numbers align right and text aligns left.
Every count carries thousands separators, while a `path:line` location never does.

## Plain text

A warning prints as a plain line and never through `Write-Warning`.
A failure the audit cannot judge throws and exits with an error.
`check.ps1` reads only the `Counters` section, so its rows are the whole contract.
