# PThemeSearch.xaml

## `<Style x:Key="Theme.Search.Bar" TargetType="Border">`

The ground every panel's ordering control and search field stand on together.
Every searching panel stands on it, the two duplex wings included, so a search reads alike everywhere.
The outline is clear at rest and appears under the pointer or on keyboard focus.
An edge therefore marks what the control is doing rather than that it exists.
It is held at one pixel throughout.
A bar that gains its edge therefore does not shift the row beneath it.
The bar is the placement target of the ordering dropdown, so the dropdown falls from the whole control.
A bar with no name hangs the dropdown from its dropper instead, offset back to the bar's own edge.

## `<Style x:Key="Theme.Search.Dropper" TargetType="ToggleButton">`

The ordering button as it stands inside the bar, with no frame and no ground of its own.
It takes a soft ground only under the pointer or while its dropdown is open.
It is unfocusable, so tabbing into the bar reaches the field the user came to type in.

## `<Style x:Key="Theme.Search.Field" TargetType="TextBox">`

The search field inside the bar, on one line and without a frame.
A field whose matches drop beneath the bar hooks its own key and focus events itself.
It takes the plain template, which draws the placeholder but no frame of its own.
The bar around it already frames the dropper, the divider and the field as one control.

## `<Style x:Key="Theme.Search.Helper" TargetType="ToggleButton">`

A folding control standing next to the bar rather than inside it.
It serves a panel the search bar does not own.
It borrows the bar's height and corner, so the two shapes read as one row of controls over the catalog.
It carries a marked label like the command row does.
The mark is left out when no icon is named.
The label is left out when no content is named, so the control stands as a square mark alone.
An open panel is shown by the soft accent ground the mode buttons use.
The fold is a state and not an action.
`PLook` in the deportment leaves out the mark and label and switches every look.
