# TDiweiArchive.cs

## `public sealed class TDiweiArchive`

Covers the diwei store over stored fanqie rows and a small hypothesis.
A character with two placements links every part of each, and counts entries by the characters they hold.
Two categories together pick the entries of one cell, and two that share no row pick none.
Applying stores the derived reading and tone class on each row, blank where the hypothesis has no answer.
A rebuild without a hypothesis drops the tone classes and the categories nothing links to, and blanks the readings.
A refetch drops the old row's links by cascade and the category left orphaned.
