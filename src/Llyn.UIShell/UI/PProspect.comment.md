# PProspect.cs

## `public partial class PEditor`

The dropdown of Entries a typed translation may link to, and the rows it offers.
It is one popup the editor owns rather than one per card, because only one card is being typed into at a time.
So it is retargeted at the caret it was opened from and remembers which card asked for it.
A card built at runtime could not declare a popup of its own anyway.

It sits beside the other candidate popups the editor owns rather than under the field that opens it.
The pronunciation lookup, the recording search and the part-of-speech menu are the same shape.

## `internal void PProspectHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and links it.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## Inline notes

### `private bool PProspectHandle(Key key)`

The dropdown never takes focus, so the caret's key handler drives it.
The arrows walk the selection and wrap at either end, and enter takes what is selected.
Reports whether the dropdown used the key, so an unused one falls through to the field.

### `private void PProspectSelect(PProspectItem item)`

Links the chosen row and closes the dropdown, starting a tentative entry first on a create row.
No database row is written for it, so an edit thrown away leaves nothing behind.
The tentative entry carries the typed word and the chosen language, ready to be stored.
A court row ties it to this draft, and storing this draft stores it too.
The entry and its row are asked for in one call.
A stub half made is a draft nothing points at.
The chip is named from the row that came back, not from an id the form guessed.
A stub that cannot be started raises a notice rather than leaving the field looking unanswered.

### `private void PProspectShow(PCard card, string word, IReadOnlyList<LEntry> found)`

The create row stands after the matches rather than among them.
It carries the typed word untouched, because that word is what the tentative entry will be called.

### `private IReadOnlyList<string> PProspectLanguageRead()`

The stub is offered once per language the workspace holds, so the user picks rather than accepts.
The language being edited comes last, because a link usually crosses into another one.
The workspace's own list is asked, because a language it does not hold has no flag to draw.
A list that has not loaded yet still names the language being edited, so a stub is always reachable.
