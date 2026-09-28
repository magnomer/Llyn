# PYunjing.cs

## `public class PYunjing : UserControl`

The yunjing panel: the workspace browsed as a rime table, by onset and rime of the user's reconstruction.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [CYunjing](../../../Llyn.Conduct/Panel/CYunjing.comment.md), and this file writes controls on notice.
The two columns, the entry list, the category page, the reader and the editor are all served from one file.

## `public PYunjing()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It points the export and print buttons at their commands.
It ties the droppers to their popups, sets every icon, and attaches the row fills.
Row clicks are taken on each list, and every button and search field is subscribed here.

## `private Border PLadder`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PYunjingAttach(PWindow host)`

Builds the Conduct session, wraps its editor, and subscribes the notices.
The lectern is built here to follow the panel, so the session names no driver type.
It then wires the lists, the page and the editor.
The print and portrait command bindings are added last, so no can-execute query meets a session not yet built.

## `internal void PYunjingVistaRestore()`

The session starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Attaches the bulletins the panel follows, restores the order menus, and loads.

## `private void PYunjingQueryRefine()`

Answers the area's opening of a cell a fanqie chip names by emptying both search fields.

## `private void PYunjingColumnUpdate()`

Lists both columns afresh with their empty texts, then the page and the mode, which follow the chosen cell.

## `private void PXiaoyunUpdate()`

Lists the entries at the chosen cell afresh with the empty text the session names.

## `private void PDiweiUpdate()`

Hands the page its composed content, blank while it is hidden.

## `private void PYunjingModeUpdate()`

Writes every visibility and enablement off the session's verdicts.

## `private void PYunjingClearUpdate()`

The entry list was cleared, so the page is read again.
The lectern empties the reader, and the session resets the editor itself.

## `private void PYunjingHandle(object sender, RoutedEventArgs e)`

A click on either column hands the cell's id and side to the select gate.

## `internal void PYunjingVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void PYunjingRetreatHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void PYunjingAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void PYunjingUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PYunjingRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PYunjingChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

