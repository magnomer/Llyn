# PReflexCommand.cs

## `public static class PReflexCommand`

The routed commands the reflex rows of the input panel raise, bound once on the panel.
Each carries the row as its parameter.

## `public static RoutedCommand PReflexCommandAddition { get; }`

Adds a row after the one the plus was pressed on, in the same language and kind.

## `public static RoutedCommand PReflexCommandRemoval { get; }`

Drops the row the minus was pressed on.

## `public static RoutedCommand PReflexCommandMain { get; }`

Marks or unmarks the row as the reading in common use.

## `public static RoutedCommand PReflexCommandAnchor { get; }`

Opens the anchor dropdown under the row's placement label.
