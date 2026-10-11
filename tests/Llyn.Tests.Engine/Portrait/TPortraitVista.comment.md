# TPortraitVista.cs
Hash: `22284381645c0ddc`

## `public sealed class TPortraitVista`

Portrait export and the print press resolve content from the view's selection.
An empty Entry selection writes no file and opens no press.
A catalog selection resolves the catalog's own subject.

## `public async Task Export_Vista_UsesChosenEntryAndDoesNotCreateAFileForEmptySelection()`

Exporting without a chosen Entry leaves no output file.
Choosing one writes a file holding its headword.

## `public async Task Print_Vista_UsesChosenEntryAndIgnoresClearedSelection()`

Opening the press after another Entry was chosen and then cleared hands it nothing.
A selected Entry then opens the press over its content.
The opened sheet holds no trace of the cleared one.

## `public async Task Print_CatalogVista_ResolvesItsSubject()`

A selected Example in the corpus view opens the press over the Example text.
Portrait resolution therefore works for catalog subjects too.
