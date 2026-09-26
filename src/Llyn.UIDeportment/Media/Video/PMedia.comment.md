# PMedia.cs

## `internal sealed class PMedia`

The row maker a reading view hands down to the picture and film rows built inside its templates.
A reading view lists a card's locations straight from the draft, so no code of its own builds the rows.
The rows still need the engine to turn a location into an address, and this carries it down for them.

## `public static readonly DependencyProperty PMediaProperty`

Attached and inherited, so one write on the view's root reaches every template under it.

## `internal static void PMediaAttach(DependencyObject root, LWindow window)`

Puts one maker on the view's root, made over the engine the view was attached to.

## `internal static void PMediaRevealAttach(ItemsControl list)`

Makes a media list reveal its row handles while the pointer or the focus is inside it.
Attaching twice is harmless, since each handler is removed before it is added.

## `private static void PMediaRevealHandle(object sender, MouseEventArgs e)`

Answers the pointer entering or leaving the list.

## `private static void PMediaRevealHandle(object sender, DependencyPropertyChangedEventArgs e)`

Answers the focus entering or leaving the list.

## `private static void PMediaRevealApply(ItemsControl list)`

Shows or hides the handle panel of every realized row at once.
Hiding clears the values, so the theme's hidden base shows again.
