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

## `internal void PWindowMentionHandle(PMention anchor, CMentionResult result)`

What a click on a word of a shown sentence opens, decided once for every panel that draws one.
Any menu already open is closed first, so a second click never stacks two.
A word stored as standing for an Entry opens that Entry through the navigation's gate.
The card it is narrowed to is scrolled into view.
A word stored as standing for nothing opens nothing.
An unlinked word with exactly one candidate Entry opens it.
An unlinked word with several opens the menu under that word, and the reader picks.
The word's place is read off the control that drew it, at the offset the engine settled on.
An unlinked word with no candidate opens nothing.

## `internal void PWindowSenseShow(FrameworkElement anchor, Rect place, long entryId, Action<long> chosen)`

Opens the menu in Sense mode on the Meanings of one Entry.
The window reads the Meanings, because the menu holds no engine.
A read that fails is reported the way a failed word lookup is, and nothing opens.
The linking gesture is the caller, and it passes what to do with the chosen sense.
Both the card row and the corpus scribe call it.

## `internal void PWindowProspectShow(FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)`

Passes the corpus scribe's ask for the Entry picker through to the editor that owns it.
The popup is declared once, in the editor, and the window is the only path between the two panels.

## `private void PMentionLeaveHandle(object? sender, EventArgs e)`

The menu closes when the window loses activation.
A popup that stays open over another window would answer keys meant for that window.
