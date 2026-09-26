# PVita.cs

## `public class PVita : UserControl`

The reading of one Author as a control of its own, held by the authors panel.
It draws the vita sheet it is handed and asks the engine for nothing.
The owning panel decides what is read and when the control is shown.

## `public PVita()`

Loads the markup from the Veneer, wears it as content, and copies its name scope.
Subscribes the clicks of both lists and attaches the co-author and citation row fills.

## `private StackPanel PVitaBody`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PVitaAttach(PWindow host, LGuild guild)`

Keeps the window and the deportment the panel built, for the two row clicks.

## `internal void PVitaShow(LVita vita, bool held)`

Writes the vita from its read sheet: name, counts, fellows and citing places, and shows or hides the body.

## `private void PFellowHandle(object sender, RoutedEventArgs e)`

A co-author row selects that Author in the same panel.

## `private void PVitaCitationHandle(object sender, RoutedEventArgs e)`

A citing place leads to its Example or its Entry, as the row knows.
