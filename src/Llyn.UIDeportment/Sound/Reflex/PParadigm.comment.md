# PParadigm.cs

## `public sealed class PParadigm : ContentControl`

The box above the meanings listing an inflecting headword's forms, one row per slot.
It is built in code like `PGrasp`, so no template has to be kept in step with it.
The box only draws the rows it is handed and never asks the engine which rows to show.
It collapses while it holds nothing, so a headword without forms leaves no gap.

## `public static readonly DependencyProperty PParadigmItemsProperty`

The rows shown, already mapped from slots by `PParadigmItemCreate`.
An empty or null list collapses the box.

## `public PParadigm()`

A bordered box in the paradigm theme holding one list of rows.
The list is a shared size scope, so the part and name columns line up across every row.
The row template is read from the theme by key, so the box carries no markup of its own.
The box neither focuses nor tabs, since nothing in it is edited.
Each realized row is filled by `PParadigmItemRefine`, since the template carries no bindings.

## `private static void PParadigmItemRefine(FrameworkElement container, object item, string? _)`

Writes the part, name and form into the named text blocks of one row.
The form text arrives ready, and a row with a tip key shows it muted with that tooltip.
A row without a tip clears the mark, so a reused row shows no stale tooltip.

## `private static void PParadigmItemsRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Hands the new rows to the list and shows or collapses the box by their count.
## `public IReadOnlyList<PParadigmItem>? PParadigmItems`

The rows shown, or `null` for none.

## `internal void PParadigmRefine(IReadOnlyList<CParadigmSlot> slots)`

The seam the lectern's sound draws through, mapping the slots to rows, and no slots collapse the box.
It only sets a value, so the box redraws itself as for any other change.
