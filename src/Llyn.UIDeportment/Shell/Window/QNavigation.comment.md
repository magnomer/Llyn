# QNavigation.cs
Hash: `1d10c925a4904be0`

## `internal sealed class QNavigation`

The window's tab buttons and the jumps panels ask for.
Conduct's `CNavigation` decides which tab shows and keeps the voyage, and this driver paints what it raises.
The tabs are held here only with their buttons and panels.
Each panel's rail lights its own voyage buttons, so no tab hands a hook over.
It knows the loaded window and the navigation, and no window driver.

## `internal QNavigation(Window surface, CNavigation navigation)`

Keeps the loaded window and the atelier's navigation, then reads the tabs once.
It paints the tab icons and introduces itself.
The window builds it after every panel attaches, so every panel has registered its tab.

## `private Button QNavigationInput`

The tab buttons are read through the loaded window's name scope.
Each is a plain Veneer `Button`, whose chosen look is the look's `Chosen` cue.

## `private void QNavigationObserve(object sender, RoutedEventArgs e)`

A tab button click, handed to the navigation's select gate as the key of the button pressed.
An unknown button sends nothing.
The navigation raises the change it made, so nothing is painted here.

## `private void QNavigationIntroduce()`

Marks the tab whose panel the markup shows, before any change arrives.
It subscribes every tab button's click, then the navigation's changes this driver paints.
It hooks the window's key and mouse previews to the voyage shortcuts.

## `private void QNavigationIconRefine()`

Each tab button's icon is resolved here and set as the look's icon, which the tab style's icon part copies.

## `private void QNavigationRefine(CNavigationState state)`

Hides the tabs the session does not offer, then marks the open tab's button and shows only its panel.
A state naming no tab leaves the open tab as it is.

## `private static void QNavigationChosenRefine(QTab tab)`

Marks the tab's button chosen while its panel shows, and clears the mark otherwise.
The mark is the look's `Chosen` cue, so the tab style's accent rows apply.

## `private QTab? QNavigationFind(object? button)`

The tab owning the given button, or null.

## `private QTab[] QNavigationTabRead()`

The tabs of the window, each with the key the navigation names it by.
Each panel is pulled from the window by its contract ID.

## `private void QVoyageKeyObserve(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward, each through its one voyage gate.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.
The key binding is the GUI's own, so it stays here.

## `private void QVoyageMouseObserve(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.
Each button reaches its one voyage gate.
