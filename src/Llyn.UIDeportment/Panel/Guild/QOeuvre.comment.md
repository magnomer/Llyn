# QOeuvre.cs
Hash: `654b7905766409de`

## `internal sealed class QOeuvre`

The middle column of the authors panel, with the oeuvre search field over it.
It lists every Source crediting the chosen Author, or every Source while none is chosen.
Choosing a row shows that Source in the colophon, through the guild's gate.
It subscribes what it paints itself, so the owner `QGuild` only builds and introduces it.

## `internal QOeuvre(UserControl surface)`

Takes the guild page, and finds `POeuvre`, `POeuvreEmpty` and `PComb` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QOeuvreIntroduce(CGuild guild, CAtelier atelier)`

`QGuildIntroduce` calls it once the Conduct guild exists.
It subscribes the oeuvre panel's rows event and the workspace opening.
It binds the list, takes each row's click, and attaches its row fill.

## `private void QOeuvreRefine()`

Lists the oeuvre afresh with its empty text.
It answers the oeuvre's rows notice and the workspace opening.
Conduct restores the vistas before the opening, so the list reads them restored.

## `private void QCombObserve(object sender, TextChangedEventArgs e)`

Hands the typed oeuvre search to the oeuvre's query gate.

## `private void QOeuvreObserve(object sender, RoutedEventArgs e)`

Hands the clicked Source's id to the gate.
