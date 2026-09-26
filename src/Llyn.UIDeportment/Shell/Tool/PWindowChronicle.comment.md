# PWindowChronicle.cs

## `public partial class PWindow`

The three key chords that walk a draft's chronicle: Ctrl+Z back, Ctrl+Y or Ctrl+Shift+Z forward.
The window owns the chords so every draft editor answers them the same way.
Which editor answers is decided by where the keyboard is, not by which panel is showing.

## `private static PChronicleHost? PChronicleHostRead()`

Climbs from the focused element to the first ancestor that answers undo and redo.
The situation and example forms each hold an entry form inside them.
Climbing meets the nearer of the two first, which is the one the user is typing in.
A focused element outside the visual tree climbs the logical tree instead, so a text run still finds its form.
Nothing focused, or nothing above it that answers, says the chord is not ours.

## `private void PChronicleKeyHandle(object sender, KeyEventArgs e)`

Turns the chord into an undo or a redo on the host found, and swallows the key when one was.
It runs on preview, so it beats the text box's own undo, which the theme has switched off anyway.
A chord with no host under it falls through untouched.
