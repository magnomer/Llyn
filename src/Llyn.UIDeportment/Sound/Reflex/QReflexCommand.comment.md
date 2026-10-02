# QReflexCommand.cs
Hash: `d8942c5c953b1bcb`

## `public static class QReflexCommand`

The routed commands the reflex block of the input panel raises.
The row commands bind once on the panel, and each carries the row as its parameter.

## `public static RoutedCommand QReflexCommandAddition { get; }`

Adds a row after the one the plus was pressed on, in the same language and kind.

## `public static RoutedCommand QReflexCommandRemoval { get; }`

Drops the row the minus was pressed on.

## `public static RoutedCommand QReflexCommandMain { get; }`

Marks or unmarks the row as the reading in common use.

## `public static RoutedCommand QReflexCommandRenewal { get; }`

Fetches the reflexes again, raised by the renewal button rather than by a row.
It binds on that button alone and carries no parameter.
The size hold runs before the fetch, so the button stays under the pointer.

## `public static RoutedCommand QReflexCommandAnchor { get; }`

Opens the anchor dropdown under the row's placement label.
