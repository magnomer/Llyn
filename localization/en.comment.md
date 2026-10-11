# en.json
Hash: `827bf119a87fec1c`

The English interface catalog, and the language every launch starts from.
It is embedded into `Llyn.Infrastructure` and never read from disk.

## Shape

The file opens with flat `terms.*` pairs and closes with one `texts` object.
`LLocalizationLoader` refuses any other order, a non-string value or a repeated key.

## `terms.*`

A term is a reusable word or phrase, such as the product name, that texts cite.
Each term is published under its capitalized key, such as `Terms.Product`, so markup or code can read a bare term.
The imperfect, future and conditional terms exist so inflection labels can share them.
The sync term names the push to Joplin, shared by the sync button and the Sync settings page.
The print, cancel, color and custom terms are shared within the print window.
The print term also labels the Scribe print button, and the cancel term the Leave, Coinage and Customs dialogs.

## `texts`

Keys are resource names that markup binds to or code reads, grouped by a dotted prefix per area.
A text cites a term as `{Terms.name}` or `{terms.name}`, and the capital sets the casing.
Numbered slots like `{0}` are left for the caller to format.
`Unit.*` labels the unit chooser and its morpheme, word and expression choices.
`Freedom.*` labels the freedom chooser and its dependent and standalone choices.
`Inflection.*` names grammar terms that a pack's inflection layout cites as group, line and header keys.
Most are generic, so other packs can reuse them, and the inflection table shows them in the interface language.
`Paradigm.Fold` names the switch between the short and the full inflection table.
`Paradigm.Short` labels the switch button that shows the short inflection table.
`Paradigm.Full` labels the switch button that shows the full inflection table.
`Card.Fold` and `Card.Unfold` are the hinge tooltips that fold or unfold one card.
`Notebook.*` labels the Sync page field naming the top Joplin notebook.
`Xiesheng.ReadingsShow` and `Xiesheng.ReadingsHide` label the series member hinge that shows or hides all its readings.
`Settings.Inflection` names the settings page that holds the `Analysis.*` switch and its hint.
A failure text is keyed `<Area>.<Action>Failed`, or `<Area>.Failed` for an area's main action.
It sits beside its area's other texts.
Most reach the user through the ledger, but some show in place, such as `Card.VideoFailed`.
An area without other texts, such as `Sentence`, `Fold` or `Box`, sits beside the nearest texts on the same subject.
`Print.*` labels the print window, one key per control, choice, hint, status line and failure.
`Print.Page` names the page number box, and `Print.PageTotal` is one entry so each language orders "of {0}" itself.
`Print.Sheets` and `Print.SheetsOne` give the plural and singular count of paper sheets.
`Print.Idle` is the footer text while nothing prints.
Every other catalog must carry exactly these keys, or a `TLocalizationLoader` test fails.
