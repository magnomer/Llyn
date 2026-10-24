# Rafter

The roof menu bar along the window's top row, shared by every tab.
It also holds the product and options menus, and the help box and tab chooser those menus open.
It stands apart from `Chamber`, because the window only builds the bar and hands it keys.

## `BRoof`

The bar over the `ARoof` part of `AWindow`, with its menus left to right.
A menu with no item besides rules is left out, at construction and at each apply.
At most one menu is open, and its items drop below the bar.

## `BRoofApply`

Replaces the menus whole and rebuilds the key table from them, so the bar lives as long as the window.
The construction applies the first menus, so the menus have one writer.
An open menu stays open: the one with the same title, or else the one at the same place.
Its lit item stays lit when it is live.
That is the item with the same label, or else the one at the same place.
An open submenu stays open under that item, its lit item kept the same way.
A menu or item that is gone closes the menu, or leaves the first live item lit.

## `BRoofKeymap`

The bar's own key table.
F9 opens the first menu when the window has the focus, and closes an open one.
Escape closes, Left and Right change menu, and Up and Down skip to the next live item.
Enter and an item's hotkey run the item, after the bar closes.
Hotkeys of submenu items are bound too, and they run only while that submenu is the level shown.
On an item with a submenu, Enter, Right and its hotkey open the submenu instead.
While a submenu is open, Up, Down, Enter and hotkeys act in it.
Left and Escape close only the submenu, and Right changes menu.
Alt with a menu's letter opens that menu, unless an earlier row already holds the key.

## `BRoofKeymapFocus`

The roof focus while a menu is open, and otherwise the window focus.

## `BRoofKeyObserve`

Runs the row this table holds for the key and answers whether one ran.
An Alt key is also looked up at the roof focus, so a menu letter opens from the closed bar.

## `BRoofDraw`

Draws the bar, then each title that fits, with its hotkey letter marked unless its menu is open.
The open title is lit, and its menu draws below it with a frame, rules and a shadow.
A menu that would pass the right edge is moved left to fit.
An open submenu draws right of its menu, with its first item level with the lit item.
It moves left the same way.
All widths use the measure passed in, so the window's ambiguous-width choice applies.
An item with a submenu shows the `Submenu` glyph in its key column.

## `BRoofMenu`

One menu: its title, the letter that opens it, and its items.
A null item is a rule between groups.

## `BRoofItem`

One menu line: its label, the key shown at its right, its hotkey, its action and whether it is live.
A dim item cannot be lit or run.
An item may hold a submenu, one level deep.
Its own action then never runs.

## `BCabinet`

Builds the `Options` menu.
Theme, glyph set and ambiguous width are submenus of radio items.
The item for the current choice carries the `Radio` mark.
The others carry blanks of the same cell width.
The blank is measured, so the labels line up under a wide mark.
Language lists the ledger's languages the same way, and is dim while no ledger state has been heard.
A language choice is saved through the ledger, whose change the window then applies to the catalog.
`Settings…` selects the Settings tab.
Hotkeys come from the catalog, and only a single ASCII letter becomes a hotkey.

## `BHeadquarter`

Builds the product menu, `Llyn`.
The Joplin items come first, only when there is a courier and its state has been heard.
Attach shows only while not attached, and Send always shows.
Both are dim until the courier state allows a request.
The `Workspace…` item follows them, only when a workspace port is given, and it asks `CWorkspaceChange` through the envoy.
Its hotkey comes from the catalog and it shows no shortcut.
A rule follows the group above, and there is no rule when the group is empty.
About, Help, Keys and Quit follow, with F1 and F10 as the shortcuts shown for Help and Quit.
Keys opens help at the help focus, so it starts on the keys everywhere page.
About shows the atelier's about lines in a `dialog` box with OK.

## `BSNavigation`

The tab chooser body, in two columns.
It lists the visible tabs with a hotkey each.
The hotkeys are the digits 1 to 9 and then the letters, so at most 35 tabs show.
Arrows move the lit tab, and Enter or a hotkey chooses.
The width follows the widest label, and a drawn column is clamped to the room given.
Its key table is the body's own, so help lists these keys beside the box's.

## `BSHelp`

The body of the help box: four pages of prose and keys, linked to each other.
The pages are keys everywhere, the window, a box, and tabs.
Each page has a key focus, and the window page uses the help focus.
The tabs page reads the roof focus and keeps only the Alt rows.
The other roof rows are not about tabs.
The keys of a page come from a function, so help lists only what is bound.
The window page also lists the pager's own rows.
Rows with the same key and modifiers are listed once.
A digit key is listed as its digit.
Pages are built on every read, so a key's live state shows at once.
Help opens on the box page for a box focus and on the keys everywhere page for the help focus.
Otherwise it opens on the window page.
Each page lists its keys as `keys   meaning`, joining keys with one meaning and leaving out hidden rows.
A dim row draws in the `disabled` role.
Each page ends with links to the other pages, and the lit link takes the pointer from the `History` glyphs.
A `BPager` moves through the pages and the links, and F1 inside help goes to the keys everywhere page.
The page position shows the lit link of the links on the page, and nothing on a page without links.
The width is 78 and the height 14.
Prose and key columns wrap by measured cells, so the window's width setting applies.
The console minimum fills the small-console sentence.

## Stamp

Commit: `78fc0ebfff7c`
- `BCabinet.cs` `9c7873781402935c`
- `BHeadquarter.cs` `34abc335bfdcbfb1`
- `BRoof.cs` `0315392a65140e16`
- `BRoofItem.cs` `e2a42521c4ed1f78`
- `BRoofMenu.cs` `7ab739815c3a5874`
- `BSHelp.cs` `70e5b0723e3a20e4`
- `BSNavigation.cs` `ca9085841e92ebd5`
