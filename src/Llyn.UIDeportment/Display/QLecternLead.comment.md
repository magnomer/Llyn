# QLecternLead.cs
Hash: `f744babc1ea40def`

## `public sealed class QLecternLead`

A lectern section's shown state together with its label column.
The label column joins the shared `PReadingLabel` width only while its section shows.
A collapsed section left in the group would still widen every other section's labels.

## `public QLecternLead(UIElement section, ColumnDefinition column)`

Holds the section and the label column the section that builds it pulled from the page.

## `public void QLecternLeadRefine(bool shown)`

Shows or collapses the section from the ready verdict `shown`.
The label column joins or leaves the shared width group in the same step.
