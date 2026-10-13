# Taxonomy

The taxonomy panel reads entries by a label somebody put on one of their cards.
It stands apart from the library panel, which asks for an entry by its headword.
Its markup carries no hook.
The Deportment driver `QTaxonomy` sets commands, the search hint and the tag row fills.
`QMembership` drives the entry column and its search field.
The picker, the filter and the rail have their own drivers, which set their icons and popups.
The rail's driver folds the two button pairs by mode.

## `PTaxonomy`

The shell behind `PTaxonomy.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## Layout

Three columns run from narrow to wide.
They hold the tag catalog, the entries under the chosen tag, then the reader.
So the catalog here is a tag, and the entry list beside it is what that tag holds.

## Seams

The seams part the tag catalog, the entries under a tag, and the entry itself.
The row seam runs under the ordering bar and the command row and is bled past the panel margin.
There are two column seams, because this panel reads across three columns and not two.
Neither seam encloses anything, since a line marks a division and a box would claim an object.

## `PFunnel`

The sorting button and the search field are one control over the tag column.
Both act on the tag catalog and nothing else.
The ordering picker is the shared `PChoiceOrder`, placed as `PTaxonomyOrder`.

## Entry column

The entry list is a bare catalog column, not a boxed surface.
Both lists on this panel are asked the same kind of question, so both are drawn the same way.
A surface behind one of them would read as an object rather than a list.
Each row carries the flag, the headword and the language, in the library catalog's order.
The column uses `Theme.Catalog.Middle` to measure its left gutter from the intervening seam.
The catalog uses the shared `Theme.Catalog.Frame`, `Theme.Catalog.Scroll` and `Theme.Catalog.Row` styles.

## `PMembershipEmpty`

Shown when the chosen tag holds no entry.
It also stands while no tag is chosen and the workspace is empty.

## `PLattice`

The search field and the language filter over the entry column.
`PScout` narrows the entries under the chosen tag by typed text.
The language filter is the shared `PChoiceFilter`, placed as `PTaxonomyFilter`.
It opens the menu of loaded languages, and its mark shows while any is hidden.

## `PTaxonomyRail`

The same entry actions the library panel offers, over the same reader.
The command row is the shared `PPanelRail`.
Export and print act on the entry being read.

## Stamp

Commit: `c2f335c331fe`
- `PTaxonomy.xaml` `ce182c1f7a422bae`
- `PTaxonomy.xaml.cs` `186ac9504aaa3caa`
