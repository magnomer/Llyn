# QLookSheet.cs
Hash: `27eb51f1c0379994`

## `internal static class QLookSheet`

The table of dismantled trigger setters that `QLook` switches.
It stands apart from `QLook`, so the handler stays small while the rows grow with each returned theme.

## `internal static readonly IReadOnlyList<QLook.QLookSetter> QLookSheetState`

One row per dismantled trigger setter, in the order the triggers stood.
A later row wins over an earlier one on the same part and property, as a later trigger did.
A row in state `Base` applies without a state cue.
Base rows supply template values, icons and animations.
A style key without a dot names a control type, for the implicit styles.
A style trigger outranked a template trigger, so its rows follow the template's rows.
A style based on another needs none of that style's rows, since `QLook` applies them first.
Input rows come from `QLookInput`, and control rows come from `QLookControl`.
These precede the sound and display rows.
The sound themes' rows come from `QLookSound` and the display's from `QLookDisplay`, spread after them.
A style merged by one view rather than the application is registered by that view through `QLookStyleAttach`.
The editor dictionaries are such styles, and the corpus and imprint panels merge the popup one too.
A row's cues are `QLookCue` flags, and a row holds while every flag it names holds.
The paradigm fold rows put checked after hover, so a hovered checked button keeps its pill.
The checked fold button gets the page ground, a faint outline and the accent ink.
It reads as the thumb of a segmented switch on its tinted track.
The card hinge rows give its chevron the expand icon in the icon's own colours.
Hover lays the soft accent under it.
An unfolded card turns the icon half a turn, which the app reads as open and able to close.
Checked means folded, so it leaves the icon unturned, which reads as closed and able to open.
Its tooltip then offers unfolding.
An icon row copies `QLook.QLookIconProperty`.
Hover, press, inset and scale values are veneer resources, set by key so a theme switch reaches them.
The progress sweep is the veneer resource `Theme.Popup.ProgressBar.Sweep`, pulled by contract ID.
So it agrees with the card spotlight, which is a veneer resource too.
