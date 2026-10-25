# Mention

The console's menu of entries that a mention in a sentence may stand for.
It is drawn as the body of a parley box and knows no window.

## `BSMentionMenu`

One list of translation targets, each a headword and its language, with one lit row.
It is a parley body, so the box hands it keys while the body holds the focus.
Up and Down move the lit row and wrap round the list.
Enter on the lit row opens that entry through navigation and ends the menu.
The body is as wide as its widest row or the title, plus 6 cells.
The headword column is as wide as the widest headword, and the language follows it after 2 cells.
The list scrolls so the lit row stays in the last visible line.
A long row is clamped to the width.
The menu ends when navigation arrives somewhere, whatever caused it.

## `BSMentionMenuShow`

With no offer, or an offer with no entries, it opens nothing.
So no empty box flashes and no key is taken.
Otherwise it builds the title from the offer's key.
It opens a dialog box with no text and no buttons, placed at the given anchor.
The menu is handed to the docket with the task of its choice.
The call returns when the box is gone.
The arrival subscription is removed afterwards, even if the docket throws.

## Stamp

Commit: `8c403d1df768`
- `BSMentionMenu.cs` `ac35769e6d36c0b6`
