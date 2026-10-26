# Marker

The console's part-of-speech row of the editor, as the GUI's `QMarker`, `QUnit` and `QFreedom` are.
The row holds the unit chooser, the freedom chooser and the parts of speech taken.
The field for the next one follows.
Conduct's `CCardSpeech` and the entry decide every verdict, so the row holds only what it last read.

## `BMarker`

It joins the two choosers and the speech field into one row with three spots.
The spot is the unit, the freedom or the speech field.
Left and Right move among them while nothing is open.
Enter on a spot opens that spot's list or field.
The row reads the draft again on each draft change, so it never asks Conduct while drawing.
The keys are live only when the frame says the row is lit, a test the frame passes in.
So the row never binds a key on a row the user is not on.
A printable key on the speech spot opens the field and types the key, so no key is lost.
The field is a text box and not a chooser on purpose.
The user may type a part of speech the menu does not offer.
Each typed text asks Conduct for a category, and the menu opens only when Conduct says it should show.
A menu opens with no row lit, and the first arrow lights the first row.
The first move chooses nothing and only starts the choice.
So Enter keeps adding the typed text until a row is chosen.
Enter adds the lit row when one is chosen, else the typed text.
Backspace on an empty field drops the last part of speech taken.
Closing the field persists the draft, so a field edit is kept as one step.
A menu with no row but a hint shows the hint as one muted line that nothing can light.
The lists draw after the row and hang from the row's anchor line, each under its own spot.
The unit list sits at the row's start, the freedom list after the unit, and the menu after the chips.
Closing the row closes the unit and freedom lists and the field.

## `BUnit`

The unit chooser, which names the held unit or the field's title while none is held.
Every name is a key Conduct chose, so the chooser only looks it up in the catalog.
The row list is the one Conduct last announced, with a flag for the unit the draft holds.
The list shows a check beside the held unit and starts with it lit.
The page holds all rows, so a unit list never scrolls.
A pick hands Conduct's key and the entry's gate decides the result.
Letters go to the drop's seek.

## `BFreedom`

The freedom chooser, which works as `BUnit` does over the freedom rows.
Picking the held freedom clears it, a rule the entry keeps, so the chooser does not repeat it.

## Stamp

Commit: `f3a687d70b47`
- `BFreedom.cs` `579e3c6633edb000`
- `BMarker.cs` `0d2b0496be9591fa`
- `BUnit.cs` `ab63dbb1d2c19dd3`
