# TInflectionLoader.cs
Hash: `fcd1cc2164aeb2e2`

## `public sealed class TInflectionLoader`

Covers the `inflection` side of the pack loader, on fixture packs and on the real Spanish pack.
The fixture book holds one kind, one stem with a `null` ending, one rule and one fold.

## `public void InflectionPackRead_FileNamed_LoadsBook()`

A file named under `inflection` loads as a book that predicts a form.
The `null` ending leaves its column out, so the stem keeps one ending.
The stamp is 64 lowercase hex digits, and an edit to the file changes it.
A book without a `layout` carries none.

## `public void InflectionPackRead_Inline_LoadsBook()`

An object written in place under `inflection` loads the same book with a stamp of its own.

## `public void InflectionPackRead_MissingFile_ReturnsNull()`

A named file that is absent, or no key at all, leaves the pack without a book.

## `public void InflectionPackRead_BadRegex_SkipsRule()`

A rule or fold whose pattern does not parse is skipped, and the rest load.
A rule's third element is kept as the kind that limits it.

## `public void InflectionPackRead_SpanishPack_LoadsLayoutForVerbs()`

The Spanish book lays out part 6 in three default collapsed groups and eleven expanded lines under six header keys.
Every expanded line takes the six columns.
The collapsed imperfect subjunctive line joins its values with each form value, codes ascending.

## `public void InflectionPackRead_SpanishPack_LoadsDefaultAndCustomLayouts()`

The Spanish book carries both pairs of sheets.
The default pair lists the indicative, the subjunctive, then the imperative.
The default collapsed sheet ends with the affirmative imperative second singular.
The custom pair keeps the custom order on the same part and has no custom pair of its own.

## `public void InflectionPackRead_LayoutWithoutCustom_UsesDefaultSheets()`

A layout without `custom` still carries a custom pair.
That pair holds the very default sheets, so either setting shows the same box.

## `private static string[] TInflectionGroupRead(LInflectionSheet sheet)`

The group labels of a sheet in order, read from the lines that open a group.

## `public void InflectionPackRead_SpanishPack_PredictsHandChecks(string headword, string cell, string predicted, string actual, string marks)`

The design's hand checks hold on the real pack, not only on a book built in code.
Each row states the prediction and the marked ranges of the actual form.
The first singular imperative has no ending, so it stays uncovered.
