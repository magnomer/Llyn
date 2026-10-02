# TLecternFold.cs
Hash: `eb875323ffad58ff`

## `public sealed class TLecternFold`

Covers the reflex fold toggle the reading view shares with the editor through one display.
The toggle lives on its own STA thread, since a WPF control demands one.

## `public void FoldObserve_DisagreeingToggle_SettlesOnFlag()`

The editor opens the fold through the fold gate while the reading view's toggle still reads closed.
The gate's change repaints the toggle, whose event runs the observer once and then settles.
The flag and the toggle both end open, where a flip would recurse until the stack overflows.
