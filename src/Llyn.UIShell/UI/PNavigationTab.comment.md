# PNavigationTab.cs

## `public partial class PWindow`

Which panel the window shows.
The navigation buttons pick exactly one.
The chosen button wears the selected style.
Every other panel is collapsed out of the layout.
The tab chosen is pushed downstream as it changes, so the window opens on it next time.

## Inline notes

### `if (PLibrary.IsVisible && selectedButton != PNavigationLibrary && !PWindowDiscardConfirm(PLibrary.PLibraryChangeCheck()))`

Leaving the library panel while it is being written in is leaving the editing state.
So it is asked about here.
A tab that is switched away from keeps its editor.
But the correction the user typed would sit out of sight until they came back.

## `internal void PNavigationRestore(LWorkspaceState state)`

Puts the window back on the tab `state` names.
That tab goes back on the side it was left standing on.
It goes through the ordinary tab switch, so the restored tab is selected exactly as a click selects it.
A state naming no tab leaves the window on the tab it opens with.
So does one naming a tab this build no longer offers.

## `private (string Mode, Button Button, FrameworkElement Panel, Action<bool>? Scribe)[] PNavigationTabRead()`

The tabs of the window, each with the name it is stored under.
Each also carries the way to put it back on its editor.
The switch and the restore read the same table.
A tab cannot be stored under one name and restored under another.
A tab holding no editor offers no way to restore one.

## `internal void PWindowEntryShow(long id)`

Switches to the library panel and opens one Entry there.
It is the way a referring side listed under a shared record reaches the Entry holding it.
Panel switching is the window's to do, so a panel asks for it rather than reaching into `PNavigation` itself.
The library is asked first, because the tab guard above passes over a library that is already the target.
Opening the entry would otherwise cancel the draft being written in that panel without a word.

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

## `internal void PWindowSituationShow(long id)`

Switches to the repertoire panel and opens one Situation there.
It is the way a situation chip read on a card reaches the record it names.
The repertoire is asked first, for the reason the library is asked above.

## `internal void PWindowTagShow(long id)`

Switches to the taxonomy panel and browses by one tag, named by its id.
The taxonomy is asked first, for the reason the library is asked above.

## `internal void PWindowExampleShow(long id)`

Switches to the corpus panel and opens one Example there.
A Source citing row names an Example rather than an Entry, so it needs the counterpart of the entry switch.
The corpus panel is asked first, for the same reason the library is.

Every browse-style panel that holds an editor is asked the same question before the tab changes.
The list stays one guard per panel rather than one loop, because each panel names its own check.
