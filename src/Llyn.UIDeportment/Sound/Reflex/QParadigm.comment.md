# QParadigm.cs
Hash: `3e26747dc81aaded`
Hash: `64fcce5d82c31306`

## `public sealed class QParadigm : Decorator`

The box lists paradigm rows and optionally draws an inflection sheet.
It draws the sheet as two tables, a collapsed one and an expanded one.
It is a drawn leaf, so it derives from `Decorator` and the Veneer only places it.
It is built in code like `PGrasp`, so no template has to be kept in step with it.
The box only draws the rows and the sheet it is handed.
It never asks the engine which rows to show.
It reads Deportment items only, so no Conduct record reaches the surface.
It collapses while it holds nothing, so a headword without forms leaves no gap.

## `public static readonly DependencyProperty QParadigmItemsProperty`

The rows shown, already mapped from slots by `QParadigmItemScan`.
An empty or null list collapses the box unless a sheet is shown.

## `public static readonly DependencyProperty QParadigmSheetProperty`

The inflection sheet, already mapped from the Conduct view by `QParadigmSheetCreate`.
It is `null` for a pack without a layout, and the box then shows only its rows.

## `public QParadigm()`

A box in the paradigm theme holding a stack of the list, the switch and both tables.
The list is a shared size scope, so the part and name columns line up across every row.
The row template is read from the theme by key, so the box carries no markup of its own.
The box never takes focus, and only the two switch buttons are tab stops.
Each realized row is filled by `QParadigmItemRefine`, since the template carries no bindings.
The switch is two text buttons on one track, so both choices stay readable at once.
They share the track's panel as their group, so checking one unchecks the other.
The short button starts checked, so the collapsed table shows first.
The track carries the `Paradigm.Fold` tooltip and accessible name for the pair.
It starts collapsed, so a box without a sheet draws no empty track.

## `private static void QParadigmItemRefine(FrameworkElement container, object item, string? _)`

Writes the part, name and form into the named text blocks of one row.
The form text arrives ready, and a row with a tip key shows it muted with that tooltip.
A row without a tip clears the mark, so a reused row shows no stale tooltip.

## `private static void QParadigmItemsRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Hands the new rows to the list and shows the box while it has rows or a sheet.

## `private static void QParadigmSheetRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Rebuilds both tables from the new sheet and shows the switch only while a sheet exists.
The buttons keep their state, so a repaint after a save does not fold the table back.
The box shows while it has rows or a sheet.

## `private void QParadigmFoldRefine(object sender, RoutedEventArgs e)`

Shows the expanded table while the full button is checked and the collapsed table otherwise.
Both buttons call it on check, since an unchecked button raises no check of its own.
The fold is a visual state only, so it calls no gate and stores nothing.
The button colours come from the state sheet, so the handler never decides a colour from a control.
With no sheet both tables stay hidden.

## `private static void QParadigmTableRefine(Grid grid, QParadigmTable table)`

Lays one table out as a grid of a group column, a label column and one column per form.
The form columns number the larger of the header count and the widest line.
Headers take the first row when the table has any.
Group, label and header texts are resource keys, looked up like the tip keys.
An empty key adds no text block, so its cell stays blank.
A group name shares the row of its group's first line, so form columns hold one position across groups.
Lines keep the order the sheet gives, and rules mark the group ends that order implies.
The header row and every line but the last get a rule from `QParadigmRuleDraw`.

## `private static void QParadigmRuleDraw(Grid grid, int row, bool closed)`

Draws one line under a table row, sharing the row with its cells and sitting at their bottom.
A closed rule spans every column, since it ends the header or a whole group.
An open rule starts at the label column, so the group name stands clear of the inner rules.
The span is at least one column, so a table without forms still draws a valid rule.

## `private static void QParadigmFormRefine(TextBlock block, QParadigmForm form)`

Splits the form text into runs at its marked ranges and at its split.
A marked run takes `Theme.Paradigm.Marked`, leaving its colour to the theme.
A split inside the text adds a hyphen run styled by `Theme.Paradigm.Cut`.
The split is one more run boundary, so a mark crossing it still paints both sides.
A range running past the text is cut at its end.
A form with a tip key shows muted with that tooltip, as a listed row does.

## `public IReadOnlyList<QParadigmItem>? QParadigmItems`

The rows shown, or `null` for none.

## `public QParadigmSheet? QParadigmSheet`

The inflection sheet shown, or `null` for none.
