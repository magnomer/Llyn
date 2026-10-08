# QMentionMenu.cs
Hash: `4e16ce165e70c6d0`

## `internal sealed class QMentionMenu`

The menu that opens at a clicked word when the word could mean more than one thing.
It is one popup of the loaded window, because every panel that draws a sentence answers a click through it.
It has two modes and one shape.
Entry mode lists the Entries an unlinked word may mean, and picking one opens it.
Sense mode lists the Meanings of one Entry, and picking one settles the `QMentionAsk` that opened it.
The menu never writes and never reads the engine.
It shows the rows it is given and reports the one picked.
It knows the loaded window and the navigation, and no window driver.

## `internal QMentionMenu(Window surface, CNavigation navigation)`

Reads the popup, its title and its list off the loaded window, keeps the atelier's navigation, then wires the menu.
It sets itself as the mention attached property, so every mention block below inherits its host.
The list is fed from the menu's own collection, and each realized row is filled and subscribed here.
However the popup closes, the menu hides, so a closed menu keeps no rows for the keys.
The window builds it before its own wiring, so its preview key runs ahead of the chronicle's.
Deactivation and every navigation landing close it too.
A landing only closes the menu, since the tab's area opens the record itself.

## `private Popup QMentionPopup { get; }`

The popup, its title and its list are read once through the loaded window's name scope.
The popup itself is declared in the window markup beside the panels, once.

## `internal void QMentionOfferRefine(PMention anchor, CMentionOffer? offer)`

Shows the menu a word click's gate left, under the found word of `anchor`.
Every panel that draws a sentence takes this one path, the reading display and the corpus excerpt alike.
The word's place is read off the control that drew it, from the offer's unit in the whole text.
That is a ready surface value, so the menu calls no gate here.
The gate already asked, found and opened, so the menu only paints the offer.
A null offer shows nothing, and an empty one only closes any open menu.

## `private void QMentionEntryRefine(FrameworkElement anchor, Rect place, CMentionOffer offer)`

Entry mode, on the candidates and the title key a find gate's `CMentionOffer` carries.
A picked row opens its Entry through the navigation's gate.
The menu is already hidden by the pick, and a landing hides it too.

## `internal QMentionAsk QMentionMeaningRefine(FrameworkElement anchor, Rect place, CMentionSense sense)`

Sense mode.
The caller hands the ready sense menu a Conduct read answered, its title key and its rows.
It answers a fresh `QMentionAsk` over the anchor, and the caller subscribes that ask alone.
So only the asker hears its pick, and no area compares anchors.
The card row and the corpus scribe call it directly, each with its own area's ready read.

## `private void QMentionMenuObserve(object sender, MouseButtonEventArgs e)`

Takes the row the pointer released on and picks it.
The row fill subscribes it on each realized row directly.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `private void QMentionMenuHide()`

Closes the menu and forgets the rows it offered, the row it lit and the sense ask.
It is safe to call when the menu is already closed, which is how every navigation calls it.
The popup's own closing calls it too, so a closed menu never keeps rows for a key to reach.

## `private void QMentionMenuRefine(FrameworkElement anchor, Rect place, string key, IReadOnlyList<PMentionItem> rows)`

The one path both modes go through.
It hides first, so entry mode always opens with no sense ask.
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

Captures the sense ask, then hides before it settles, so a subscriber sees a closed menu.
A subscriber that opens another panel does not then close a menu that has already moved on.
With no ask the pick is an Entry, opened through the navigation's gate.
Zero as the settled sense means the whole Entry.

## `private QMentionAsk? _qMentionMeaningAsk;`

The ask of the open sense menu, which tells sense mode from entry mode.
It is null in entry mode, and every hide clears it.

## `private void QMentionRowRefine(FrameworkElement container, object item, string? _)`

Fills one menu row, indents it by its depth and subscribes its click.
The flag and language fold away when the row carries no language.

## `private void QMentionLeaveRefine(object? sender, EventArgs e)`

The menu closes when the window loses activation.
A popup that stays open over another window would answer keys meant for that window.
