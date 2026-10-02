# QFanqieCommand.cs
Hash: `998f6295db3e54af`

## `public static class QFanqieCommand`

The routed commands the fanqie line's links raise, bound by the fanqie box itself.
Each carries its line or text as its parameter, because the template has no code of its own.

## `public static RoutedCommand QFanqieCommandInitial { get; }`

Opens the rime table on the line's initial.

## `public static RoutedCommand QFanqieCommandStem { get; }`

Opens the stem a block's key names.
It carries the raw key text rather than a line, since the key heads a block of lines.

## `public static RoutedCommand QFanqieCommandRime { get; }`

Opens the rime table on the line's rime.

## `public static RoutedCommand QFanqieCommandRepresentative { get; }`

Moves the line's representative rank when its star is pressed.
The command carries only the line, so the box reads the Ctrl state at the moment it hears it.
