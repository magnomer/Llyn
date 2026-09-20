# PMentionMenu.cs

## `public partial class PWindow`

The menu that opens at a clicked word when the word could mean more than one thing.
It is one popup the window owns, because every panel that draws a sentence answers a click through the window.
It has two modes and one shape.
Entry mode lists the Entries an unlinked word may mean, and picking one opens it.
Sense mode lists the Meanings of one Entry, and picking one hands its id to whoever asked.
The menu never writes and never reads the engine.
It shows the rows it is given and reports the one picked.

## `internal void PMentionMenuShow(FrameworkElement anchor, Rect place, LMentionResult result)`

Entry mode.
A picked row opens its Entry through `PWindowEntryShow`, so the menu hides the way any navigation hides it.

## `internal void PMentionMenuShow(FrameworkElement anchor, Rect place, long entryId, IReadOnlyList<LMeaning> meanings, Action<long> chosen)`

Sense mode.
The window reads the Meanings before calling, so the menu stays free of the engine.
A picked row calls `chosen` with its sense id, and zero means the whole Entry.
The linking gesture calls it from the card row and from the corpus scribe.

## `internal void PMentionMenuHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer released on and picks it.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `internal void PMentionMenuHide()`

Closes the menu and forgets what it offered and whom it would tell.
It is safe to call when the menu is already closed, which is how every navigation calls it.

## `private void PMentionMenuShow(FrameworkElement anchor, Rect place, string key, IReadOnlyList<PMentionItem> rows, Action<PMentionItem> chosen)`

The one path both modes go through.
The popup hangs under the anchor, moved right to the clicked word and up to the line it sits on.
So a wrapped sentence opens the menu under the word and not under the block's last line.
The header is bound to a localization key rather than a string, so a language change renames it in place.
The first row starts selected, so Enter alone picks the likeliest answer.
An empty list opens nothing.

## `private void PMentionKeyHandle(object sender, KeyEventArgs e)`

Hooked into the window's preview key event, so the keys reach the menu wherever focus sits.
The rows are not focusable, and the menu takes no focus of its own.

## `private bool PMentionMenuHandle(Key key)`

Up and Down cycle through the rows, Enter picks the selected one and Escape closes.
Every other key passes through, so a shortcut still works while the menu is open.
It is the same shape as the dropdown the editor owns.

## `private void PMentionMenuSelect(PMentionItem item)`

Hides before it tells, so the callback sees a closed menu.
A callback that opens another panel does not then close a menu that has already moved on.
