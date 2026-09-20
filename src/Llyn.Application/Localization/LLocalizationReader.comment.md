# LLocalizationReader.cs

## `internal static class LLocalizationReader`

Resolves one parsed localization catalog into a flat dictionary of texts.
The catalog arrives as raw pairs, `terms.*` entries beside the texts, already lifted off the JSON by the adapter.
Each text may cite a term as `{terms.name}`, cased by the capital of the reference.
Every term is also published under its capitalized key, so markup can bind a bare term.

## `private const string LLocalizationReaderTerms = "terms.";`

The prefix that tells a term pair from a text pair in the raw map.

## `private static readonly Regex LLocalizationReaderKey`

The shape a term key must take.

## `private static readonly Regex LLocalizationReaderReference`

A term reference inside a text, with its casing carried by the first letter.

## `private static readonly Regex LLocalizationReaderSlot`

A numbered format slot, left alone when the text is checked for stray braces.

## `internal static IReadOnlyDictionary<string, string> LLocalizationRead(IReadOnlyDictionary<string, string> raw, CultureInfo culture)`

Gathers the terms, then resolves the texts, and refuses a catalog with no term or a repeated key.
A refusal is a `FormatException`, the one exception a pure ring may raise for bad data.

## `private static void LLocalizationTermAdd(string key, string value, IDictionary<string, string> terms)`

Keeps one term after checking its key shape.

## `private static Dictionary<string, string> LLocalizationTextScan(`

Resolves every text pair of the raw map against the terms gathered before it.

## `private static string LLocalizationTextResolve(`

Replaces each term reference in one text and refuses a brace that is neither a term nor a slot.

## `private static string LLocalizationTermFormat(string qualifiedTermKey)`

The published key of a term, each segment capitalized.
