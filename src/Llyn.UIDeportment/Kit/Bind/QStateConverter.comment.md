# QStateConverter.cs

## `internal static class QStateConverter`

Paints a written value from the ready wording Conduct hands over.
Conduct chose between the text, the unknown mark and the field's hint, so nothing is judged here.
It only looks up the key it was given and puts the result on one property.

## `internal static void QStateTextRefine(DependencyObject target, DependencyProperty property, CStateWording wording)`

Shows the value as a reader sees it.
The key's text stands in for the value while Conduct names a key, and the value's own text shows otherwise.

## `internal static void QStateHintRefine(DependencyObject target, DependencyProperty property, CStateWording wording)`

Shows the placeholder an empty field invites with.
It is the unknown mark or the field's hint that Conduct named, and nothing when Conduct named neither.
The field's own text covers the placeholder while a value stands, so the value is never drawn twice.
