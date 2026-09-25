# PRepertoireScenario.xaml

## `ResourceDictionary`

The editing side of the Repertoire panel: the bare fields a Situation is written into.
The measure style is based on the vignette's kind, so the dictionary merges the vignette shapes itself.
A loose dictionary resolves a static reference only in itself, in what it merges, and in the application resources.
The panel merges the vignette shapes too, so the reading side does not depend on the editing side being loaded.

## `<Style x:Key="Theme.Scenario.Measure" TargetType="TextBlock">`

The unseen twin that measures the kind.
Its chip closes on the written kind, or on the placeholder while it is empty.
Without it the chip in the editor stood at a fixed width the reading side never showed.

## `<Style x:Key="Theme.Scenario.Hint" TargetType="TextBlock">`

The unseen twin that measures the empty title, copied from the entry editor's `Editor.Field.Hint`.
It reads the field's own placeholder rather than one fixed word, because the placeholder changes with the state.
It is written here rather than shared, because the entry editor's styles are bound to `PHeadword` by name.

## `<Style x:Key="Theme.Scenario.Ghost" TargetType="TextBlock">`

The unseen twin that measures the written title.
The head row closes on the text rather than on a fixed box.

## `<Style x:Key="Theme.Scenario.Kind" TargetType="TextBox">`

The bare field the kind is written into, inside the same chip the reading side draws.
Bare, so the written kind sits exactly where the read kind sits.

## `<Style x:Key="Theme.Scenario.Description" TargetType="TextBox">`

The bare field the description is written into, where the reading side renders it.
Its size matches the rendered paragraph, so a line of description sits at one height in both modes.

## `<Style x:Key="Theme.Scenario.Media" TargetType="ItemsControl">`

The two row lists, indented to the description's edge and spaced as a card spaces its media.
