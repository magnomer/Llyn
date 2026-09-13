# PTranscriptionCommand.cs

## `public static class PTranscriptionCommand`

The routed commands a transcription row raises, bound by the editor that hosts the rows.
Each carries the row as its parameter, because the row template has no code of its own.

## `public static RoutedCommand PTranscriptionCommandAddition { get; }`

Adds a transcription row in the next scheme the draft does not yet hold, after the row.

## `public static RoutedCommand PTranscriptionCommandRemoval { get; }`

Drops the transcription row.

## `public static RoutedCommand PTranscriptionCommandNotation { get; }`

Opens the lookup menu for the row, searching in the row's own scheme.
