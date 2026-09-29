# PNavigationTab.cs

## `public partial class PWindow`

The window's buttons and the jumps panels ask for.
Conduct's `CNavigation` decides which tab shows and keeps the voyage, and this part paints what it raises.
The tabs are held here only with their buttons, panels and the two Veneer hooks each panel hands over.

## `private PTab PNavigationInput`

The tab buttons are read through the loaded window's name scope.

## `private void PNavigationObserve(object sender, RoutedEventArgs e)`

A tab button click, handed to the navigation's select gate as the key of the button pressed.
An unknown button sends nothing.
The navigation raises the change it made, so nothing is painted here.

## `private void PNavigationIntroduce()`

Reads the tabs and takes the atelier's navigation, whose changes this part paints.
A landing only closes the mention menu, since the tab's area opens the record itself.
A stored Mention's sense, raised by the mention gate, scrolls into view.
It hooks the window's key and mouse previews to the voyage shortcuts.

## `private void PNavigationRefine(CNavigationState state)`

Hides the tabs the session does not offer, then marks the open tab's button and shows only its panel.
A state naming no tab leaves the open tab as it is.
The voyage buttons are lit last.

## `private QTab? PNavigationFind(object? button)`

The tab owning the given button, or null.

## `private QTab[] PNavigationTabRead()`

The tabs of the window, each with the key the navigation names it by.
The panels with a record list hand their voyage buttons.
Input, dual panel and settings hand none.

## `private void PVoyageRefine(CVoyageState state)`

Enables each tab's pair only while the matching trail has somewhere to go.
Every tab is told, because any of them may be the one on screen.

## `private void PVoyageKeyObserve(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward, each through its one voyage gate.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.
The key binding is the GUI's own, so it stays here.

## `private void PVoyageMouseObserve(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.
Each button reaches its one voyage gate.

## `internal void PWindowMentionObserve(PMention anchor, CMentionResult result)`

What a click on a word of a shown sentence found, handed whole to `CMentionResultOpen`.
Every panel that draws a sentence takes this one path.
The gate decides what opens, and the candidates it answers go to the menu under the clicked word.
The word's place is read off the control that drew it, at the offset the engine settled on.
An empty answer only closes any menu already open, so a second click never stacks two.
The reading display still takes this path until its click becomes one gate.

## `internal void PWindowMentionRefine(PMention anchor, CMentionOffer? offer)`

Shows the menu a word click's gate left, under the found word of `anchor`.
The gate already asked, found and opened, so the window only paints the offer.
A null offer shows nothing, and an empty one only closes any open menu.

## `private void PMentionSenseRefine(long sense)`

Answers `CMentionSenseChosen` by scrolling the stored Mention's sense card into view in the library's display.
The navigation's entry open lands in the library, so that display is the one showing the entry.

## `private void PMentionLeaveRefine(object? sender, EventArgs e)`

The menu closes when the window loses activation.
A popup that stays open over another window would answer keys meant for that window.
