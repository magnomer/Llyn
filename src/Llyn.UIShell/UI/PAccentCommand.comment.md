# PAccentCommand.cs

## `public static class PAccentCommand`

The routed commands a pronunciation row raises, bound by the editor or the reading view that hosts the rows.
Each carries the row as its parameter, because the row template has no code of its own.

## `public static RoutedCommand PAccentCommandAddition { get; }`

Adds a blank pronunciation row after the row.

## `public static RoutedCommand PAccentCommandRemoval { get; }`

Drops the pronunciation row.

## `public static RoutedCommand PAccentCommandNotation { get; }`

Opens the pronunciation menu for the row.

## `public static RoutedCommand PAccentCommandClip { get; }`

Opens the audio menu for the row.

## `public static RoutedCommand PAccentCommandPlayback { get; }`

Plays the row's recording.
