# Choice

The bodies of the console's order and language choosers.
Each is a `BSParleyBody`, so the box owns the frame and the Close button and the body owns the list.
Neither body applies a choice itself.
Each reports the choice to the callback it was given.

## `BSChoiceOrder`

A single-choice list of orderings, drawn with the radio glyph.
The labels are read once from the catalog as `prefix.word`.
So one body serves any list that shares the word scheme.
The applied order is read through a function on every draw.
So the mark follows the aperture and the body keeps no copy.
The lit row starts on the applied order.
It starts on the first row when the word is not in the list.
Enter hands the lit order to the callback and nothing else.
The box and the caller decide when the chooser closes.
Moving the lit row does not choose, so the applied mark stays on its row until Enter.
Escape is not bound, so the box handles it and closes the chooser unchanged.
The body is as wide as its widest label plus 10 cells, measured with the measure the box hands in.
When the list is taller than the body, the page scrolls to keep the lit row in view.
The radio glyph falls back to `(*)` when the glyph set gives none or a one-character one.

## `BSChoiceFilter`

A multi-choice list of languages, drawn with the check glyph.
A ticked language is shown and an unticked one is hidden, so the starting ticks come from the supplied filter.
Space flips the lit row and reports the whole hidden list at once.
So the filter applies while the chooser is still open, and no confirm step exists.
The hidden list is rebuilt from the ticks, so a hidden name the list does not hold is dropped.
Enter is not bound, so the box closes the chooser and the hide callback is not called.
The body is as wide as its widest language plus 10 cells.
The check glyph falls back to `[x]` when the glyph set gives none or a one-character one.
The unticked box keeps the glyph's edges and blanks the middle, so both states take the same cells.

## Stamp

Commit: `fe7e11daae88`
- `BSChoiceFilter.cs` `8bbafe5cffcab5ac`
- `BSChoiceOrder.cs` `027753d63321c805`
