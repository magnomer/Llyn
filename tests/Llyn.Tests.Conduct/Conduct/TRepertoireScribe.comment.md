# TRepertoireScribe.cs
Hash: `95556055de3a03cf`

## `public sealed class TRepertoireScribe`

Covers the repertoire scenario desk, how it opens and how it is left, on a real workspace.
It builds each repertoire through `TRepertoire.TRepertoirePrepare`.
A fresh start with no row chosen opens a blank scenario under a checked scribe.
A saved fresh scenario shows the stored Situation on the vignette, outside the scribe.
A stored leave on a click stores the scenario and shows the clicked Situation.
A kept leave on a click records nothing.
A leave with nothing unsaved asks nothing, and a stored leave keeps the Situation.
A kept leave stays on the scenario, from the leave question and from an occurrence click alike.
Closing the scenario cancels the desk, and closing the occurrence editor falls back to the chosen Situation.
An entry notice with no chosen occurrence keeps the scenario held.
