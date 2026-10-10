# LSchemaFold.cs
Hash: `8f54d54779c68955`

## `public static class LSchemaFold`

Creates the fold marks, one table per folding surface.
A row's presence is the flag, as with the favorite marks.

## `public static void LSchemaFoldCreate(SqliteConnection connection)`

A row in `sense_fold` means that Meaning card is folded.
A row in `collocation_fold` means that Collocation card is folded.
A row in `reflex_fold` means the "More readings" fold of that entry is opened.
A row in `fanqie_fold` means the Fanqie box of that entry is opened.
A row in `script_fold` means the Script box of that entry is opened.
A row in `stem_fold` means that entry's "More readings" fold is opened on that series key's page.
It is keyed by the series key, not the series id, since a series rebuild mints new ids.
No row means folded, which is the default of both boxes.
The marks sit in tables of their own, so the card save path and the draft never see them.
Folding therefore never dirties an entry, never makes a revision and never enters undo.
Each cascade drops the mark with the card or entry it stands on.
Saving an entry updates its cards in place, so a mark follows its card across saves.
`LSchemaCollocation.LSchemaCollocationSettle` finds `collocation_fold` through its foreign key and renumbers it with its card.
