# PGlossCommand.cs

## `public static class PGlossCommand`

The routed command a Gloss row raises, bound by whichever surface hosts the row.
The card editor and the corpus panel bind it to their own handlers, so one template serves both.

## `public static RoutedCommand PGlossCommandRemoval { get; }`

Drops the Gloss carried as the command parameter.
