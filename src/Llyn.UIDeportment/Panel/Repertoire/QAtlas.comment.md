# QAtlas.cs
Hash: `2ff8bdece4ed8a5b`

## `internal sealed class QAtlas`

The left column of the Repertoire panel, with the Situation search field over it.
It lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
It subscribes what it paints itself, so the owner `QRepertoire` only builds and introduces it.

## `internal QAtlas(UserControl scope)`

Takes the Repertoire page, and finds `PInquest`, `PAtlas` and `PAtlasEmpty` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QAtlasIntroduce(CRepertoire repertoire)`

`QRepertoireIntroduce` calls it once the repertoire Conduct exists.
It subscribes the atlas's rows event and the repertoire's dropped search.
It binds the list and attaches its row fill.

## `private void QInquestObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

## `private void QInquestRefine()`

Empties the inquest box after an arrival dropped the query on the vista.
The owner's `QRepertoireClearRefine` rebuilds the filter menu on the same notice.

## `private void QAtlasRefine()`

Refills the catalog with the ready rows the Conduct reads, already matched and already ordered.
The Conduct words the unknown and untitled texts, and reports its own read failure, answering no rows.
The rows arrive as `CCatalogSituation` shapes, the kind and the count carrying their own text.
The Conduct read drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Its clear announces fresh rows that list the same Situations, so the nested refill is harmless.
The tally chips and the entry column follow through the atlas's row notice, so neither is painted here.

## `internal void QAtlasRefine(IReadOnlyList<CCatalogSituation> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and the owner's flag-fill Refine hands in what its load answered.

## `private void QAtlasObserve(object sender, RoutedEventArgs e)`

A clicked row asks the Conduct to select its Situation.
The Conduct asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAtlasItemRefine(FrameworkElement container, object item, string? _)`

Fills one atlas row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The kind is written as it is, and the look sheet collapses it while empty.
