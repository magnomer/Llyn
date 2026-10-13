# Dialog

This folder holds the small windows the program raises over its panels.
They are markup only, and their C# shells only call `InitializeComponent()`.
The deportment fills every list, number and state through the named parts.
The buttons wear the editor's own command styles, so each dialog reads as part of the program.

## `PSCoinage`

It is the dialog New opens when nothing in the taxonomy or tenor panel is chosen.
A Tag or a Register is only a name, so an editor for one would hold one field.
The message says which of the two is being named.
The mint button stays dark until something is typed.
Escape and the retreat button leave without making anything.

## `PSCustoms`

It is the declaration shown before imported entries enter the workspace.
One row per entry shows number, headword, language, the way it enters, and the stored entry joined.
Mode and target are ordinary dropdowns, so a reader changes one row without touching the rest.
The deportment tags the named mode rows with Conduct's customs modes.
The target rows show the candidate's headword and its id, since candidates share one headword by definition.
The loss column says what a replaced entry gives up, and stays blank for any other mode.
Accept is the default and Cancel answers the escape key.
A second face reports the omissions the import skipped, one line and text per row, with one Close button.
Only one face is visible at a time, so the window keeps one size and place through the import.

## `PSLeave`

It stands in front of a user about to leave unsaved work behind.
It says what would be lost and offers three ways out, which are save, discard and stay.
A system message box could offer only yes and no, and neither word said what it did.
Save is the default, because a leaving user more often means to keep the word than to lose it.
Cancel answers the escape key, so backing out costs one keystroke.

## `PSPress`

It is the print preview window, with its settings beside the page canvas and the commit buttons below.
The pickers declare no items, so the setting's ready lists are their only source and order.
The chrome, margins and footer copy the customs dialog, so every Llyn dialog reads as one family.
The window is resizable, since a reader enlarges it to see a page at a readable size.
The settings column is a fixed width and scrolls alone, so the canvas keeps its place.
Each control column is 232 wide, so Microsoft Print to PDF and both segments show in full.
The page fields, the hint and the warning sit under the range, because they qualify that choice.
The margin fields stay hidden until the manual margin is chosen, and the unit sits beside them.
Orientation and color are mode pairs, since each is one choice between two values.
The two switches take focus, so the keyboard reaches every setting in reading order.
The footer keeps the system dialog link left and the status, Print and Cancel right.
Print is the default and Cancel answers the escape key.
Tab order follows the declaration order, which is the reading order of the window.

## `PSGalley`

It is the print preview: the page canvas with its veils above and its bar below.
`PSPress` hosts it in the bordered cell beside the settings.
It is a control of its own because the window's markup outgrew one file.
Its class is a bare shell holding no logic, and the drivers find every part by its contract ID.
The lookup walks the logical tree, so the IDs inside this control stay reachable from the window.

### `PSGalleyCanvas`

The canvas stacks one white paper per page on the raised surface.
It is named `PSGalleyCanvas`, since the control itself carries the `PSGalley` class name.
White stays fixed whatever the palette, because the printed sheet is white.
Each paper holds the drawn page, a shade and a badge for a page left out of the range.

### `PSPressBusy`

The busy veil keeps the last pages visible beneath it, so a change never blanks the canvas.
It covers only the canvas row, so the bar stays usable while a page renders.
It keeps its `PSPress` ID, since `QSPress` drives it as window state.

### `PSPressFailure`

The failure veil says what went wrong and offers one retry.
It covers only the canvas row and keeps its `PSPress` IDs, as the busy veil does.

### Canvas bar

The bar moves between pages on the left and sets the zoom on the right.
`QSGalley` drives the page half and `QSGalleyBar` drives the zoom half.
The sheet count fills the space between them and trims first when the window narrows.
The zoom percent is a text field, since the input choice template has no editable face.
Every icon button carries a tooltip and every control an accessible name from the print keys.

## Stamp

Commit: `c2f335c331fe`
- `PSCoinage.xaml` `40f278684d097fc1`
- `PSCoinage.xaml.cs` `236c0943b576a5b9`
- `PSCustoms.xaml` `1ec49982e96d943e`
- `PSCustoms.xaml.cs` `cf47ea49039ed919`
- `PSGalley.xaml` `a982ebf5b1fed6ae`
- `PSGalley.xaml.cs` `1e51f276e0be1f6d`
- `PSLeave.xaml` `d290e36b12c457a3`
- `PSLeave.xaml.cs` `aa4c01cc390871f7`
- `PSPress.xaml` `6d3d1309d7106767`
- `PSPress.xaml.cs` `4a4336a2f0ae139c`
