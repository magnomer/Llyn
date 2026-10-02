# QReflexCommand.cs

## `public static class QReflexCommand`

The routed commands the reflex rows of the input panel raise, bound once on the panel.
Each carries the row as its parameter.

## `public static RoutedCommand QReflexCommandAddition { get; }`

Adds a row after the one the plus was pressed on, in the same language and kind.

## `public static RoutedCommand QReflexCommandRemoval { get; }`

Drops the row the minus was pressed on.

## `public static RoutedCommand QReflexCommandMain { get; }`

Marks or unmarks the row as the reading in common use.

## `public static RoutedCommand QReflexCommandAnchor { get; }`

Opens the anchor dropdown under the row's placement label.
