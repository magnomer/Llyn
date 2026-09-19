# LLocalizationReader.cs

## `internal static class LLocalizationReader`

Parses one localization file into a flat dictionary of texts.
A file opens with `terms.*` entries and closes with one `texts` object.
Each text may cite a term as `{terms.name}`, cased by the capital of the reference.
Every term is also published under its capitalized key, so markup can bind a bare term.

## `private static readonly Regex LLocalizationReaderKey`

The shape a term key must take.

## `private static readonly Regex LLocalizationReaderReference`

A term reference inside a text, with its casing carried by the first letter.

## `private static readonly Regex LLocalizationReaderSlot`

A numbered format slot, left alone when the text is checked for stray braces.

## `internal static IReadOnlyDictionary<string, string> LLocalizationRead(Stream stream, CultureInfo culture)`

Reads the terms, then the texts, and refuses any file that breaks the order or repeats a key.

## `private static void LLocalizationTermAdd(JsonProperty property, IDictionary<string, string> terms)`

Keeps one term after checking its key shape and its string value.

## `private static Dictionary<string, string> LLocalizationTextScan(`

Resolves every text of the `texts` object against the terms read before it.

## `private static string LLocalizationTextResolve(`

Replaces each term reference in one text and refuses a brace that is neither a term nor a slot.

## `private static string LLocalizationTermFormat(string qualifiedTermKey)`

The published key of a term, each segment capitalized.
