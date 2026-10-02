# QNavigationTab.cs

## `public partial class QWindow`

The window's buttons and the jumps panels ask for.
Conduct's `CNavigation` decides which tab shows and keeps the voyage, and this part paints what it raises.
The tabs are held here only with their buttons, panels and the two Veneer hooks each panel hands over.

## `private Button QNavigationInput`

The tab buttons are read through the loaded window's name scope.
Each is a plain Veneer `Button`, whose chosen look is the look's `Chosen` cue.

## `private void QNavigationObserve(object sender, RoutedEventArgs e)`

A tab button click, handed to the navigation's select gate as the key of the button pressed.
An unknown button sends nothing.
The navigation raises the change it made, so nothing is painted here.

## `private void QNavigationIntroduce()`

Reads the tabs and marks the one whose panel the markup shows, before any change arrives.
It then takes the atelier's navigation, whose changes this part paints.
A landing only closes the mention menu, since the tab's area opens the record itself.
A stored Mention's sense, raised by the mention gate, scrolls into view.
It hooks the window's key and mouse previews to the voyage shortcuts.

## `private void QNavigationRefine(CNavigationState state)`

Hides the tabs the session does not offer, then marks the open tab's button and shows only its panel.
A state naming no tab leaves the open tab as it is.
The voyage buttons are lit last.

## `private static void QNavigationChosenRefine(QTab tab)`

Marks the tab's button chosen while its panel shows, and clears the mark otherwise.
The mark is the look's `Chosen` cue, so the tab style's accent rows apply.

## `private QTab? QNavigationFind(object? button)`

The tab owning the given button, or null.

## `private QTab[] QNavigationTabRead()`

The tabs of the window, each with the key the navigation names it by.
The panels with a record list hand their voyage buttons.
Input, dual panel and settings hand none.

## `private void QVoyageRefine(CVoyageState state)`

Enables each tab's pair only while the matching trail has somewhere to go.
Every tab is told, because any of them may be the one on screen.

## `private void QVoyageKeyObserve(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward, each through its one voyage gate.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.
The key binding is the GUI's own, so it stays here.

## `private void QVoyageMouseObserve(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.
Each button reaches its one voyage gate.

## `internal void QWindowMentionRefine(PMention anchor, CMentionOffer? offer)`

Shows the menu a word click's gate left, under the found word of `anchor`.
Every panel that draws a sentence takes this one path, the reading display and the corpus excerpt alike.
The word's place is read off the control that drew it, at the offset the engine settled on.
The gate already asked, found and opened, so the window only paints the offer.
A null offer shows nothing, and an empty one only closes any open menu.

## `private void QMentionSenseRefine(long sense)`

Answers `CMentionSenseChosen` by asking the library lectern's card to scroll the sense card into view.
The navigation's entry open lands in the library, so that display is the one showing the entry.

## `private void QMentionLeaveRefine(object? sender, EventArgs e)`

The menu closes when the window loses activation.
A popup that stays open over another window would answer keys meant for that window.
