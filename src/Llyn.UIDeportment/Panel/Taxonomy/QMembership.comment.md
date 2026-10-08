# QMembership.cs
Hash: `09cb1443c1a738b5`

## `internal sealed class QMembership`

The entry list of the taxonomy panel, with the entry search field over it.
It shows the Entries carrying the chosen Tag, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QTaxonomy` only builds and introduces it.

## `internal QMembership(UserControl surface)`

Takes the taxonomy page, and finds `PMembership`, `PMembershipEmpty` and `PScout` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QMembershipIntroduce(CTaxonomy taxonomy)`

`QTaxonomyIntroduce` calls it once the Conduct taxonomy exists.
It subscribes the area's opening event and the membership panel's rows event.
It binds the list and attaches its row fill.

## `private void QScoutObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the membership vista, whose announcement refills the entry list.

## `private void QScoutRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied the entry query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

## `private void QMembershipRefine()`

Answers the entry list's rows event, which every successful Tag read also raises.
So the list follows the catalog, because the chosen Tag may have just changed or vanished.
The first paint arrives the same way, from the catalog's read after the flags are loaded.
Rows sharing a headword arrive numbered, so the reader can tell them apart.
Conduct picks the empty line's key from whether the entry search holds text.
A failed read has already been shown by the area, which then answers no rows.

## `private void QMembershipObserve(object sender, RoutedEventArgs e)`

A clicked row hands its entry's id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

## `private void QMembershipItemRefine(FrameworkElement container, object item, string? _)`

Fills one entry row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The entry list is a catalog like the library's, so it is marked the same way.
The epithet leads with an en space, as its string format did.
