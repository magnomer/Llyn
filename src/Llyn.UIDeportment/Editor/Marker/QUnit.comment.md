# QUnit.cs
Hash: `2bc48fc810d65367`

## `internal sealed class QUnit`

Drives the lexical unit dropper at the head of the part-of-speech row.
The dropper names the held unit, or the field's title while none is held.
Its menu lists the units the draft's language offers, the held one checked.
Every name is a key Conduct chose, so the driver only looks it up.
Each menu row is a `QUnitItem`.

## `private const string QUnitTitle = "Unit.Title";`

The key the dropper shows while no unit is held.

## `internal void QUnitIntroduce(CEditor editor)`

Takes the editor, whose gate a pick goes to.

## `internal void QUnitRefine(IReadOnlyList<(string, bool)> units)`

Rebuilds the menu rows and names the dropper after the held row.
`QMarker` calls it on every draft change with the units read beside the field.
A language change repaints too, since the offered units follow the language.

## `private void QUnitApply(FrameworkElement container, object item, string? _)`

Fills one menu row with its name and check, and wires its click.

## `private void QUnitObserve(object sender, RoutedEventArgs e)`

Closes the menu and hands the picked key to the gate.
Picking the held unit clears it, a rule the tenure keeps.
