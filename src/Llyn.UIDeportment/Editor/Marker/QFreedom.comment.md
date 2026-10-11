# QFreedom.cs
Hash: `5a8c8788e06e7771`

## `internal sealed class QFreedom`

Drives the freedom dropper that follows the unit dropper at the head of the part-of-speech row.
The dropper names the held freedom, or the field's title while none is held.
Its menu lists the freedoms every draft is offered, the held one checked.
Every name is a key Conduct chose, so the driver only looks it up.
Each menu row is a `QFreedomItem`.

## `private const string QFreedomTitle = "Freedom.Title";`

The key the dropper shows while no freedom is held.

## `internal void QFreedomIntroduce(CEntry entry)`

Takes the entry facet, whose freedom gate a pick goes to.

## `internal void QFreedomRefine(IReadOnlyList<(string, bool)> freedoms)`

Rebuilds the menu rows and names the dropper after the held row.
`QMarker` calls it on every draft change with the freedoms read beside the field.

## `private void QFreedomApply(FrameworkElement container, object item, string? _)`

Fills one menu row with its name and check, and wires its click.

## `private void QFreedomObserve(object sender, RoutedEventArgs e)`

Closes the menu and hands the picked key to the gate.
Picking the held freedom clears it, a rule the tenure keeps.
