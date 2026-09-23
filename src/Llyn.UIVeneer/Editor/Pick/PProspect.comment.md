# PProspect.cs

## `public partial class PEditor`

The dropdown of Entries a typed translation may link to, and the rows it offers.
It is one popup the editor owns rather than one per card.
Only one card is being typed into at a time.
So it is retargeted at the caret it was opened from and remembers which card asked for it.
A card built at runtime could not declare a popup of its own anyway.

It sits beside the other candidate popups the editor owns rather than under the field that opens it.
The pronunciation lookup, the recording search and the part-of-speech menu are the same shape.

It has two callers.
A typed translation opens it at a card's link caret, offering matches and then a create row per language.
A selected word opens it at the selection in a sentence field, offering matches only, and hands back the pick.
The corpus scribe reaches the second through the window, so the popup is still declared once.

## `internal void PProspectShow(FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)`

Opens the dropdown over a selected word, listing the Entries the word matches in the given language.
No create row is offered, because a Mention may only point at an Entry that already exists.
A blank word opens nothing, since every Entry would match it.
The match is the translation lookup, exact headwords first, and an empty language filters nothing.
Twin numbers are given before the language filter runs, so a row keeps the number the catalog shows.
The dropdown stands at the selection's rectangle rather than under the field, so it opens where the word is.
A word matching nothing opens nothing.
On pick the chosen Entry id is handed to the caller, and the dropdown closes.

## `internal void PProspectHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and links it.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## Inline notes

### `private bool PProspectHandle(Key key)`

The dropdown never takes focus, so the caret's key handler drives it.
The arrows walk the selection and wrap at either end, and enter takes what is selected.
Reports whether the dropdown used the key, so an unused one falls through to the field.

### `private void PProspectSelect(PProspectItem item)`

When a selection opened the dropdown, hands the row's Entry to the caller and closes.
Otherwise links the chosen row and closes the dropdown, starting a tentative entry first on a create row.
No database row is written for it, so an edit thrown away leaves nothing behind.
The tentative entry carries the typed word and the chosen language, ready to be stored.
A court row ties it to this draft, and storing this draft stores it too.
The entry and its row are asked for in one call.
A stub half made is a draft nothing points at.
The link is asked for under the target the row names, not under an id the form guessed.
The chip then shows the court's word and language, since no Entry answers a draft id yet.
A stub that cannot be started raises a notice rather than leaving the field looking unanswered.

### `private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found, bool chosen)`

A dropdown opened while the user types selects nothing, so enter still means what was typed.
One opened because a committed word was ambiguous selects its first row, since the user must choose.

The create row stands after the matches rather than among them.
It carries the typed word untouched, because that word is what the tentative entry will be called.
Rows sharing a headword are numbered by entry id, so each keeps the number the catalog shows.
The entry being edited joins that numbering and is dropped only afterwards, so its twin keeps its own number.
Create rows take no number, because their styling already sets them apart.

### `private IReadOnlyList<string> PProspectLanguageRead()`

The stub is offered once per language the workspace holds, so the user picks rather than accepts.
The language being edited comes last, because a link usually crosses into another one.
The workspace's own list is asked, because a language it does not hold has no flag to draw.
A list that has not loaded yet still names the language being edited, so a stub is always reachable.
