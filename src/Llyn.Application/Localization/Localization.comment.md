# Localization

The Localization folder holds the interface language catalog below the shell.
It parses one language file into a flat dictionary and answers every text key from it.
The shell copies the loaded dictionary into its resources, so markup keeps binding by key.
The engine reads it directly where a formatted answer needs a localized pattern.

## `LCase`

Changes the case of the first text element of a value under a culture.
A text element is taken whole, so a combined character never splits.
The rest of the value stays as written.

## `LLocalization`

### `LLocalizationDefault`

The language every launch starts from before the settings are read.

### `LLocalizationGate`

Guards the current dictionary, since a load and a read may run on different threads.

### `LLocalizationListed`

The languages the build carries a catalog for, as the localization port lists them.
Until the port is asked, only the default is listed.

### `LLocalizationLanguageSet`

The bootstrap sets the list before the first catalog is applied, and the engine sets it again on construction.
No language is named in code, so a new catalog file is listed without an edit here.

### `LLocalizationListedRead`

Reads the list under the gate it is set under.
The settings panel offers exactly these, so a new catalog file appears there without an edit.

### `LLocalizationNormalize`

The stored language when it is one the catalog ships, else the default.
`LLocalizationDefaultCheck` is true when the stored language needs no second load after the default.

### `LLocalizationCultureRead`

The culture a language's terms are cased under, which is the predefined culture of that name.
A name no culture answers to is refused.
It does not consult the listed languages, so a reader can be loaded before any vault was scanned.

### `LLocalizationLoad`

The vault overload lists the languages the port carries.
It then reads the catalog of the language through the port as the current one.
The bootstrap loads the default this way before any engine exists.
The engine loads the chosen one the same way.
The raw overload resolves the pairs handed in and makes the result the current one.
The engine reads the embedded file through `LLocalizationLoader` in the infrastructure.
A test hands in pairs it parsed the same way.
The JSON never reaches this ring, so it opens, parses and closes nothing.

### `QLocalizationTextRead`

The text of a key, or the key itself when the catalog lacks it.
`QLocalizationTextFind` answers null instead.

### `LLocalizationGroupFind`

The names of the groups with a key whose text reads the given text, in the order handed in.
The text is trimmed, and a null or blank text reads every group.
Matching is the catalog's shared rule, so it ignores case and honours its wildcards.

## `LLocalizationReader`

Resolves one parsed localization catalog into a flat dictionary of texts.
The catalog arrives as raw pairs, `terms.*` entries beside the texts, already lifted off the JSON by the adapter.
Each text may cite a term as `{terms.name}`, cased by the capital of the reference.
Every term is also published under its capitalized key, so markup can bind a bare term.

### `LLocalizationRead`

Gathers the terms, then resolves the texts, and refuses a catalog with no term or a repeated key.
A refusal is a `FormatException`, the one exception a pure ring may raise for bad data.

### `LLocalizationTextResolve`

Replaces each term reference in one text and refuses a brace that is neither a term nor a slot.
A numbered format slot is left alone when the text is checked for stray braces.

### `LLocalizationTermFormat`

The published key of a term, each segment capitalized.

## Stamp

Commit: `c2f335c331fe`
- `LCase.cs` `c1f75946f621b3a1`
- `LLocalization.cs` `61dcb1b98b6ce723`
- `LLocalizationReader.cs` `387deda70a914eae`
