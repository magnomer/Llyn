# Rafter

The roof menu bar along the window's top row, shared by every tab.
It stands apart from `Chamber`, because the window only builds the bar and hands it keys.

## `BRoof`

The bar over the `ARoof` part of `AWindow`, with its menus left to right.
A menu with no item is left out at construction.
At most one menu is open, and its items drop below the bar.

## `BRoofKeymap`

The bar's own key table.
F9 opens the first menu when the window has the focus, and closes an open one.
Escape closes, Left and Right change menu, and Up and Down skip to the next live item.
Enter and an item's hotkey run the item, after the bar closes.
Alt with a menu's letter opens that menu, unless an earlier row already holds the key.

## `BRoofKeymapFocus`

The roof focus while a menu is open, and otherwise the window focus.

## `BRoofKeyObserve`

Runs the row this table holds for the key and answers whether one ran.
An Alt key is also looked up at the roof focus, so a menu letter opens from the closed bar.

## `BRoofDraw`

Draws the bar, then each title that fits, with its hotkey letter marked while closed.
The open title is lit, and its menu draws below it with a frame, rules and a shadow.
A menu that would pass the right edge is moved left to fit.

## `BRoofMenu`

One menu: its title, the letter that opens it, and its items.
A null item is a rule between groups.

## `BRoofItem`

One menu line: its label, the key shown at its right, its hotkey, its action and whether it is live.
A dim item cannot be lit or run.

## Stamp

Commit: `c4a4eab0ba6c`
