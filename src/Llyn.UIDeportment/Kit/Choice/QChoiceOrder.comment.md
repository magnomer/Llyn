# QChoiceOrder.cs
Hash: `db7a0d6fc08dfa63`

## `internal sealed class QChoiceOrder`

Drives the shared ordering picker `PChoiceOrder` inside whichever browse panel holds it.
It owns the picker's icon, tooltip, rows and the closing of its menu, so no panel repeats them.
The chosen ordering goes straight to the panel's aperture, which saves and announces it.

## `internal QChoiceOrder(UserControl choice, FrameworkElement bar)`

Takes the placed picker and the bar its menu hangs under.
The menu opens under the whole bar, not under the button, so it lines up with the search field.
It ties the button to its popup and sets the sort icon.

## `internal void QChoiceOrderIntroduce(CAperture aperture, string title, IReadOnlyList<CCatalogOrder> orders)`

Hands the picker the aperture its choice sets.
`title` names both the tooltip resource and the prefix of every row's label.
The rows are built once, from the orderings the owner offers.

## `internal void QChoiceOrderRefine()`

Checks the row of the aperture's current ordering.
The owner calls it when a vista is restored, so the menu shows the ordering it carried.

## `internal void QChoiceOrderClose()`

Closes the menu, so nothing stays open over a window that is going.
