# PNavigationTab.cs

## `public partial class PWindow`

The window's buttons and the jumps panels ask for.
`LNavigation` decides which panel shows, so each member only hands the request over.

## `private PTab PNavigationInput`

The tab buttons are read through the loaded window's name scope.

## `private void PNavigationHandle(object sender, RoutedEventArgs e)`

A tab button click, handed to the navigation as the button pressed.

## `private LTab[] PNavigationTabRead()`

The tabs of the window, each with the name it is stored under.
The first tab is the one the window opens on.
Input, dual panel and settings hold no station and ask nothing.
Xiesheng and yunjing are offered only while a pack carries their tables.

## `internal void PVoyageRecord()`

Forwards a panel's own row click to the voyage, so the place left is recorded.

## `internal void PVoyageRetreatRun()`

Forwards a panel's retreat button to the voyage.

## `internal void PVoyageAdvanceRun()`

Forwards a panel's advance button to the voyage.

## `internal bool PWindowEntryShow(long id)`

Switches to the library panel and opens one Entry there.
Any open mention menu is closed first.
The answer says whether the jump went.

## `internal bool PWindowSituationShow(long id)`

Switches to the repertoire panel and opens one Situation there.

## `internal bool PWindowTagShow(long id)`

Switches to the taxonomy panel and browses by one tag, named by its id.

## `internal bool PWindowExampleShow(long id)`

Switches to the corpus panel and opens one Example there.

## `internal bool PWindowRegisterShow(long id)`

Switches to the tenor panel and opens one Register there.

## `internal void PWindowDiweiShow(string language, string kind, string key)`

Switches to the rime table and chooses the category a fanqie link names.
An unsaved rime table draft is confirmed before the switch.
It leaves no voyage station.

## `internal void PWindowStemShow(string language, string? key)`

Switches to the xiesheng panel and opens the series a chip names.
An unsaved draft in the panel stops the switch, as it does for a rime-table cell.
It leaves no voyage station.

## `internal void PWindowMentionHandle(PMention anchor, LMentionResult result)`

What a click on a word of a shown sentence opens, decided once for every panel that draws one.
Any menu already open is closed first, so a second click never stacks two.
A word stored as standing for an Entry opens that Entry.
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
