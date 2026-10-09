# en.json
Hash: `8252701f198f6db4`

The English interface catalog, and the language every launch starts from.
It is embedded into `Llyn.Infrastructure` and never read from disk.

## Shape

The file opens with flat `terms.*` pairs and closes with one `texts` object.
`LLocalizationLoader` refuses any other order, a non-string value or a repeated key.

## `terms.*`

A term is one reusable word, such as the product name, cited by many texts.
Each term is published under its capitalized key, such as `Terms.Product`, so markup can bind a bare term.
The imperfect, future and conditional terms exist so inflection labels can share them.

## `texts`

Keys are the resource names markup binds to, grouped by a dotted prefix per area.
A text cites a term as `{Terms.name}` or `{terms.name}`, and the capital sets the casing.
Numbered slots like `{0}` are left for the caller to format.
`Unit.*` labels the unit chooser and its content-word, function-word, morpheme and word choices.
`Inflection.*` names grammar terms that a pack's inflection layout cites as group, line and header keys.
They are generic, so every pack can reuse them, and the box shows them in the interface language.
`Paradigm.Fold` names the switch between the short and the full inflection table.
`Paradigm.Short` labels the switch button that shows the short inflection table.
`Paradigm.Full` labels the switch button that shows the full inflection table.
`Settings.Inflection` names the settings tab that holds the `Analysis.*` switch and its hint.
A failure text is keyed `<Area>.<Action>Failed` beside its area's other texts.
The ledger shows it when a read or a write fails.
An area with no other texts, such as `Sentence` or `Language`, sits beside the nearest texts on the same subject.
Every other catalog must carry exactly these keys, or a `TLocalizationLoader` test fails.
