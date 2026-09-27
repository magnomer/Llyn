# PNavigationTab.cs

## `public partial class PWindow`

The window's buttons and the jumps panels ask for.
`LNavigation` decides which panel shows, and this part marks the button and shows the panel.
The tabs are held here with their buttons and hooks, and the navigation knows each only by key.

## `private PTab PNavigationInput`

The tab buttons are read through the loaded window's name scope.

## `private void PNavigationHandle(object sender, RoutedEventArgs e)`

A tab button click, handed to the navigation as the key of the button pressed.
Answers nothing for an unknown button or when the open tab refuses to be left.

## `private void PNavigationAttach()`

Reads the tabs and builds the navigation over their keys.
It hooks the window's key and mouse previews to the voyage shortcuts.

## `private void PNavigationRestore()`

Shows every tab button unless its tab is not allowed.
Then it restores the tab the navigation answers, skipping the leave guard.
The restored tab's scribe takes the posture's split state.

## `private bool PNavigationShow(object button, long id)`

Jumps to a record in the tab of the given button.
The station is read before anything moves.
It is recorded only when the jump went, so a refused jump leaves no station.

## `private void PNavigationShow(object button, Action arrival)`

Jumps to the tab of the given button and records no station.
The arrival runs only when the switch succeeds.

## `private bool PNavigationArrivalShow(string mode, long id)`

Opens the tab of `mode` and hands it the record id.
Answers false for a tab without arrival or a refused switch.
It records no station, so the voyage walks through it freely.

## `private void PNavigationApply(string mode)`

Marks the chosen button and shows only its panel.

## `private bool PNavigationLeaveCheck(string mode)`

The leave seam: whether the tab of `mode` may be left.

## `private bool PNavigationAllowCheck(string mode)`

The allow seam: whether the tab of `mode` is offered.

## `private LTab? PNavigationFind(object? button)`

The tab owning the given button, or null.
The overload taking a key answers the tab of that key.

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

## `private (string PVoyageTab, long PVoyageId) PVoyageStationRead()`

The station of the open tab, which comes from the posture.
A tab without a station reader, or an id of zero, says there is nothing to come back to.

## `private void PVoyageUpdate()`

Enables each tab's pair only while the matching trail has somewhere to go.
Every tab is told, because any of them may be the one on screen.

## `private void PVoyageKeyHandle(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.

## `private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.

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

## `internal void PWindowMentionHandle(PMention anchor, CMentionResult result)`

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
