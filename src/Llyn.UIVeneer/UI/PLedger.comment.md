# PLedger.cs

## `private void PLedgerBuild()`

Fills the catalog with the setting groups the Dial table lists, titled from their localized names.
The order is the order of the cards, since both come from the one table.
The empty text is hidden here and only shown once a search leaves nothing.
The summaries are written last, once every row exists to carry one.

## `private string PLedgerMetaRead(string child)`

Reads the one-line summary of one group from the engine's settings and workspace.
The workspace shows the name the engine formats, its last folder segment, since the card shows the full path.
The language shows the display name the choice box carries for the stored tag, never the tag itself.
The web group counts its switches rather than naming them, because two switches make one number.

## `private string PLedgerTitleRead(string child)`

Reads the localized name of one group, the text its row shows as a heading.

## `private void PLedgerTitleApply()`

Writes every row's title anew from the localization now applied.
The language choice calls it, so the catalog changes language with the rest of the panel.

## `private void PLedgerMetaApply()`

Writes every row's summary anew.
Each handler calls it after its save, so the row reflects the value the engine now holds.
The rows are written in place rather than rebuilt, so a narrowed catalog keeps its rows.

## `private void PLedgerFind(string text)`

Narrows the catalog to the rows whose words contain the search text.
A row is matched on its title, its purpose, and the card labels and helpers read from the localization.
The card keys come from the Dial table, so a card added there is searched without editing this file.
The match is lowercased in the current culture, so the case of the typed text does not matter.
Hidden rows leave the items source rather than collapsing, so the frame keeps its sibling spacing.
The chosen row may leave the catalog while its card stays, since the card is the Dial's to swap.

## `private void PLedgerHandle(object sender, RoutedEventArgs e)`

A click on a row shows the card of that row's group.
Marking the row chosen is left to the card showing, so the first card marks its row the same way.

## `private void PWinnowHandle(object sender, TextChangedEventArgs e)`

The search field's text changed, so the catalog is narrowed to it.
A null text is read as empty, which shows every row.
