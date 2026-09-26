# PSentenceMention.cs

## `internal sealed partial class PSentence`

The row's side of the linking gesture: the Mentions the draft holds on its Example, and the chips showing them.
The row keeps the list the engine last sent only to draw the chips.
The menu asks the held draft which Mention a selection lies inside.
The row never resolves a Mention itself.
Every change goes out as a request and comes back through the redraw.

## `public PMentionLine PSentenceChip { get; }`

The chip line under the sentence field.

## `internal void PSentenceMentionShow(LWindow window, string silent)`

Redraws the chip line from the held Mentions over the sentence as the row shows it.
The editor calls it after each redraw, because the row holds no engine to read headwords from.
