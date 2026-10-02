# QAccentCommand.cs

## `public static class QAccentCommand`

The routed commands a pronunciation row raises, bound by the editor or the reading view that hosts the rows.
Each carries the row as its parameter, because the row template has no code of its own.

## `public static RoutedCommand QAccentCommandAddition { get; }`

Adds a blank pronunciation row after the row.

## `public static RoutedCommand QAccentCommandRemoval { get; }`

Drops the pronunciation row.

## `public static RoutedCommand QAccentCommandNotation { get; }`

Opens the pronunciation menu for the row.

## `public static RoutedCommand QAccentCommandClip { get; }`

Opens the audio menu for the row.

## `public static RoutedCommand QAccentCommandPlayback { get; }`

Plays the row's recording.
