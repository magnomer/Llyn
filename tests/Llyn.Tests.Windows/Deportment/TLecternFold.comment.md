# TLecternFold.cs
Hash: `da21492030f2ea49`

## `public sealed class TLecternFold`

Covers the reflex fold toggle the reading view shares with the editor through one display.
The reflex section and its named parts live on their own STA thread, since a WPF control demands one.

## `public void FoldObserve_DisagreeingToggle_SettlesOnFlag()`

The editor opens the fold through the fold gate while the reading view's toggle still reads closed.
The gate's change repaints the toggle, whose event runs the observer once and then settles.
The flag and the toggle both end open, where a flip would recurse until the stack overflows.
