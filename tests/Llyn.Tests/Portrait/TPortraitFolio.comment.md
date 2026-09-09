# TPortraitFolio.cs

## `public sealed class TPortraitFolio`

Covers the Word export, whose package is written by hand.

## `public void FolioSave_FullPortrait_WritesAReadablePackage()`

Every part is parsed, because an unbalanced tag makes a file Word simply refuses to open.
That failure is invisible until someone tries, so it is caught here instead.

## `public void FolioSave_FullPortrait_CarriesEveryFieldTheDisplayShows()`

The same fields the page carries must reach the document.
