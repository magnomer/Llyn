# TLecternFold.cs

## `public sealed class TLecternFold`

Covers the reflex fold toggle the reading view shares with the editor through one lectern.
The toggle lives on its own STA thread, since a WPF control demands one.

## `public void FoldHandle_DisagreeingToggle_SettlesOnFlag()`

The editor opens the fold while the reading view's toggle still reads closed.
Applying the fold writes the toggle, whose event runs the handler once and then settles.
The flag and the toggle both end open, where a flip would recurse until the stack overflows.
