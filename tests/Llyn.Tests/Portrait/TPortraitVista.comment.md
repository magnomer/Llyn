# TPortraitVista.cs

## `public sealed class TPortraitVista`

Portrait export and printing resolve content from the selected view.
An empty Entry selection produces no file or print job.
A catalog selection resolves the catalog's own subject.

## `Export_Vista_UsesChosenEntryAndDoesNotCreateAFileForEmptySelection()`

Exporting without a chosen Entry leaves no output file.
Choosing one writes its content to the portrait.

## `Print_Vista_UsesChosenEntryAndIgnoresClearedSelection()`

Printing without a chosen Entry submits nothing.
A selected Entry supplies the content and requested print ticket.

## `Print_CatalogVista_ResolvesItsSubject()`

A selected Example in the corpus view prints the Example text.
Portrait resolution therefore works for catalog subjects too.
