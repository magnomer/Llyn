# Tenor

The tenor panel reads entries by how formal the wording on a card is.
It stands apart from the taxonomy panel, which asks by a label somebody put on a card.
Its markup carries no hook.
The Deportment driver `QTenor` sets commands, the search hint and the register row fills.
`QCohort` drives the entry column and its search field.
The picker, the filter and the rail have their own drivers, which set their icons and popups.
The rail's driver folds the two button pairs by mode.

## `PTenor`

The shell behind `PTenor.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## Layout

Three columns run from narrow to wide.
They hold the register catalog, the Entries marked with the chosen Register, then the reader.
So the catalog here is a Register, and the entry list beside it is what carries that Register.

## `PDegree`

The sorting button and the search field are one control over the register column.
Both act on the register catalog and nothing else.
The ordering picker is the shared `PChoiceOrder`, placed as `PTenorOrder`.

## The third ordering

A register catalog offers most-marked-first, which the tag catalog does not.
A Register carries a real count of the cards marked with it.
A tag catalog has no such count to sort on.

## Entry column

The entry list is a bare catalog column, not a boxed surface, exactly as the taxonomy panel draws it.
Both lists on this panel are asked the same kind of question, so both are drawn the same way.
Each row carries the flag, the headword and the language, in the library catalog's order.

## `PGrille`

The search field and the language filter over the entry column.
`PQuest` narrows the entries marked with the chosen Register by typed text.
The language filter is the shared `PChoiceFilter`, placed as `PTenorFilter`.
It opens the menu of loaded languages, and its mark shows while any is hidden.

## `PTenorRail`

The entry actions over the reader, and the reader and editor toggle.
Export and print stand here as on every panel that reads an entry.
The command row is the shared `PPanelRail`.

## Stamp

Commit: `c2f335c331fe`
- `PTenor.xaml` `43e2e9147bc2d2ed`
- `PTenor.xaml.cs` `86aaf60058c17542`
