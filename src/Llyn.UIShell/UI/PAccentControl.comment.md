# PAccentControl.cs

## `internal static class PAccentControl`

The hover handles of a row list, built for a row only once the pointer or the focus reaches it.
Every row used to carry its plus, minus and star buttons from the start, invisible until hovered.
Three buttons per row is three control templates and three accessibility peers nobody has asked for.
A row now carries an empty slot.
The handles are loaded into it the first time the row is hovered or focused.
The slot is a content control tagged with the template key, so the row template says which handles it grows.
A row that was never hovered draws nothing there, and nothing after the slot moves, since it ends the row.

## `private static readonly DependencyProperty PAccentRowProperty`

The row container the list last handed its handles to, kept on the list itself.
A pointer crossing the same row raises moves by the dozen, and each would otherwise walk the row's tree again.
The property is attached only in code, never from markup, so it needs no accessors.

## `internal static void PAccentControlAttach(ItemsControl host)`

Watches the list `host` for pointer moves and focus, so any row reached grows its handles.

## `private static void PAccentHoverHandle(object sender, MouseEventArgs e)`

The pointer moved over the list, so the row under it is given its handles.

## `private static void PAccentFocusHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focus landed in the list, so the row holding it is given its handles.
A cell editor entered by keyboard therefore shows the same handles a hovered row does.

## `private static void PAccentControlShow(ItemsControl host, DependencyObject origin)`

Finds the row container holding `origin`, then its empty tagged slot, and loads the template into the slot.
The row last served is remembered, and a move within it is answered without a walk.
A row already holding its handles, or one whose slot names no template, is left as it is.
The slot's content is bound to its own data.
So the handles follow the row even when the row's data is replaced.

## `private static ContentControl? PAccentSlotFind(DependencyObject node)`

The first tagged content control beneath `node`, found by walking the visual tree.
