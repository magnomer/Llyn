# PMedia.cs
Hash: `5fa76a2c6ba83acb`

## `internal static class PMedia`

The reveal of a media list's row handles, shared by the editor and the reading view.

## `internal static void PMediaRevealAttach(ItemsControl list)`

Makes a media list reveal its row handles while the pointer or the focus is inside it.
Attaching twice is harmless, since each handler is removed before it is added.

## `private static void PMediaRevealRefine(object sender, MouseEventArgs e)`

Answers the pointer entering or leaving the list.

## `private static void PMediaRevealRefine(object sender, DependencyPropertyChangedEventArgs e)`

Answers the focus entering or leaving the list.

## `private static void PMediaRevealApply(ItemsControl list)`

Shows or hides the handle panel of every realized row at once.
Hiding clears the values, so the theme's hidden base shows again.
