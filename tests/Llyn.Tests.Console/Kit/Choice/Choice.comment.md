# Choice

Tests of the order and language choosers, alone and inside a `BSParley` box.

## `TChoice`

Drives `BSChoiceOrder` and `BSChoiceFilter` with the glyphs of `ABootstrap`.
The order draw and choose tests load the real English catalog.
The applied order wears the radio mark and the lit row uses the `lit` role.
Down then Enter chooses the next order and reports it once.
The applied mark stays on its row, since only the applied function moves it.
Escape is not taken by the order body and chooses nothing.
With no orders listed, the order body answers false to Enter, so the box beneath takes it and answers Close.
With no languages listed, the filter answers false to Space and shows no live key row.
Space on the filter ticks and unticks at once, and reports the full hidden list each time.
A hidden name the list does not hold is dropped from the report.
The starting ticks match the hidden names without case.
In a box, Enter and Escape both close the filter and report no hide.
In a small console with 28 languages, the lit row stays inside the frame above the Close button.

## Stamp

Commit: `c37ca1837ded`
- `TChoice.cs` `230afa79ede55af7`
