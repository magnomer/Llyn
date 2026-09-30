# PMedia.cs

## `internal sealed class PMedia`

The row maker a reading view hands down to the picture and film rows built inside its templates.
A reading view lists a ready card's media rows, so no code of its own builds the loading rows.
The rows still need the engine to turn a location into an address, and this carries it down for them.

## `internal PImage PMediaImageCreate(CImageDraft draft)`

The loading picture row for one ready picture row of a reading card.

## `internal PVideo PMediaVideoCreate(CVideoDraft draft)`

The loading film row for one ready film row of a reading card.

## `public static readonly DependencyProperty PMediaProperty`

Attached and inherited, so one write on the view's root reaches every template under it.

## `internal static void PMediaAttach(DependencyObject root, CAtelier atelier)`

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
