# QMentionMenu.cs
Hash: `598a7eecf234f899`

## `public partial class QWindow`

The menu that opens at a clicked word when the word could mean more than one thing.
It is one popup the window owns, because every panel that draws a sentence answers a click through the window.
It has two modes and one shape.
Entry mode lists the Entries an unlinked word may mean, and picking one opens it.
Sense mode lists the Meanings of one Entry, and picking one hands its id to whoever asked.
The menu never writes and never reads the engine.
It shows the rows it is given and reports the one picked.

## `private Popup QMentionMenu`

The popup, its title and its list are read through the loaded window's name scope.

## `internal void QMentionOfferRefine(FrameworkElement anchor, Rect place, CMentionOffer offer)`

Entry mode, on the candidates and the title key a find gate's `CMentionOffer` carries.
An empty list only closes the menu.
A picked row opens its Entry through the navigation's gate.
The menu is already hidden by the pick, and a landing hides it too.

## `internal void QMentionMeaningRefine(FrameworkElement anchor, Rect place, CMentionSense sense, Action<FrameworkElement, long> chosen)`

Sense mode.
The caller hands the ready sense menu a Conduct read answered, its title key and its rows.
A picked row calls `chosen` with the anchor and its sense id, and zero means the whole Entry.
The card row and the corpus scribe call it directly, each with its own area's ready read.

## `private void QMentionMenuObserve(object sender, MouseButtonEventArgs e)`

Takes the row the pointer released on and picks it.
The row fill subscribes it on each realized row directly.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `internal void QMentionMenuHide()`

Closes the menu and forgets the rows it offered and the row it lit.
It is safe to call when the menu is already closed, which is how every navigation calls it.
The popup's own closing calls it too, so a closed menu never keeps rows for a key to reach.

## `private void QMentionMenuRefine(FrameworkElement anchor, Rect place, string key, IReadOnlyList<PMentionItem> rows, Action<PMentionItem> chosen)`

The one path both modes go through.
The popup hangs under the anchor, moved right to the clicked word and up to the line it sits on.
So a wrapped sentence opens the menu under the word and not under the block's last line.
The header is bound to a localization key rather than a string, so a language change renames it in place.
The first row starts selected, so Enter alone picks the likeliest answer.
An empty list opens nothing.

## `private void QMentionKeyObserve(object sender, KeyEventArgs e)`

Hooked into the window's preview key event, so the keys reach the menu wherever focus sits.
The rows are not focusable, and the menu takes no focus of its own.
The arrows ask `CLanternMove` which row to light, sending the lit row, the row count and the direction.
A closed menu holds no rows, so the gate answers null and the arrow passes through.
Escape closes the menu only while it stands open.
Enter picks the selected row, and a closed menu has none.
Every other key passes through, so a shortcut still works while the menu is open.
It is the same shape as the dropdown the editor owns.

## `private void QMentionMenuSelect(PMentionItem item)`

Hides before it tells, so the callback sees a closed menu.
A callback that opens another panel does not then close a menu that has already moved on.

## `private Action<PMentionItem> _qMentionChosen`

Whom the open menu tells about a pick, set by each opening to its mode's answer.
It starts as a no-op and is never empty, since only rows an opening set can be picked.

## `private void QMentionRowRefine(FrameworkElement container, object item, string? _)`

Fills one menu row, indents it by its depth and subscribes its click.
The flag and language fold away when the row carries no language.
