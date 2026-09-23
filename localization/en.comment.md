# en.json

The English interface catalog, and the language every launch starts from.
It is embedded into `Llyn.Infrastructure` and never read from disk.

## Shape

The file opens with flat `terms.*` pairs and closes with one `texts` object.
`LLocalizationLoader` refuses any other order, a non-string value or a repeated key.

## `terms.*`

A term is one reusable word, such as the product name, cited by many texts.
Each term is also published under its capitalized key, so markup can bind a bare term.

## `texts`

Keys are the resource names markup binds to, grouped by a dotted prefix per area.
A text cites a term as `{Terms.name}` or `{terms.name}`, and the capital sets the casing.
Numbered slots like `{0}` are left for the caller to format.
Every other catalog must carry exactly these keys, or a `TLocalizationLoader` test fails.
