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
With no entries the menu is already answered, so the box closes at once and the keys stay hidden.
The menu ends when navigation arrives somewhere, whatever caused it.

## `BSMentionMenuShow`

Builds the title from the offer's key, or from the default mention title when there is no offer.
It opens a dialog box with no text and no buttons, placed at the given anchor.
The menu is handed to the docket with the task of its choice.
The call returns when the box is gone.
The arrival subscription is removed afterwards, even if the docket throws.

## Stamp

Commit: `cb8ab7e8feb4`
- `BSMentionMenu.cs` `0cdde20a39582bc0`
