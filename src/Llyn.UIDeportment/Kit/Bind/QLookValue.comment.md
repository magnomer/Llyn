# QLookValue.cs

## `internal abstract record QLookValue`

The value one look-sheet row sets, typed by what the row does with it.
A row names a resource key, a control property to copy, an animation, or a fixed framework value.
Each kind knows how to set itself and how to step back, so `QLook` holds no switch over them.
The implicit conversions let a row be written with the bare value, as the trigger setter was.
A fixed value keeps its own type, so no row holds an untyped value.

## `internal virtual void QLookValueClear(FrameworkElement target, DependencyProperty property, object saved)`

Puts back what the target held before any row set it.
A binding is bound again, an unset value is cleared, and any other value is set back.

## `internal sealed record QLookKey(string QLookKeyName) : QLookValue`

A resource key, set as a dynamic reference so a theme or language switch reaches it.

## `internal sealed record QLookRelay(DependencyProperty QLookRelaySource) : QLookValue`

A property of the control that a template part copies, the takeover of one template binding.
The binding takes the property's default mode, so a dropdown's toggle writes back as before.

## `internal sealed record QLookMotion(AnimationTimeline QLookMotionTimeline) : QLookValue`

An animation started on a fresh copy of the target's render transform.
A transform already animated is left running, so a hover does not restart a sweep or a spin.
Stepping back stops the animation instead of restoring a value.

## `internal sealed record QLookFix<QLookFixValue>(QLookFixValue QLookFixSetting) : QLookValue`

A fixed framework value, such as an opacity, a visibility, a cursor or a command.
