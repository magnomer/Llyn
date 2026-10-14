# localization

The interface catalogs, one JSON file per language.
They stand apart from the language packs because they translate the interface, not the dictionary data.
A new catalog is listed from the embedded resource names, so no code lists the languages.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.

## `en.json`

The English interface catalog, and the language every launch starts from.
It is embedded into `Llyn.Infrastructure` and never read from disk.
Every other catalog must carry exactly these keys, or a `TLocalizationLoader` test fails.

### Shape

The file opens with flat `terms.*` pairs and closes with one `texts` object.
`LLocalizationLoader` refuses any other order, a non-string value or a repeated key.

### `terms.*`

A term is a reusable word or phrase, such as the product name, that texts cite.
Each term is published under its capitalized key, such as `Terms.Product`, so markup or code can read a bare term.
The imperfect, future and conditional terms exist so inflection labels can share them.
The sync term names the push to Joplin, shared by the sync button and the Sync settings page.
The print, cancel, color and custom terms are shared within the print window.
The print term also labels the Scribe print button, and the cancel term the Leave, Coinage and Customs dialogs.

### `texts`

Keys are resource names that markup binds to or code reads, grouped by a dotted prefix per area.
A text cites a term as `{Terms.name}` or `{terms.name}`, and the capital sets the casing.
Numbered slots like `{0}` are left for the caller to format.
`Inflection.*` names grammar terms that a pack's inflection layout cites as group, line and header keys.
Most are generic, so other packs can reuse them, and the inflection table shows them in the interface language.
`Paradigm.Fold` names the switch between the short and the full inflection table.
`Paradigm.Short` and `Paradigm.Full` label the buttons that show the short and the full table.
`Card.Fold` and `Card.Unfold` are the hinge tooltips that fold or unfold one card.
`Card.SenseCaption` captions the row of link chips on a collocation card.
`Card.SenseAdd` labels the chip that adds one more Meaning link.
`Display.CollocationOther` heads the collocations with no home Meaning, in the read view and the exports.
`Display.CollocationStub` words the "see N" line under a further link.
Its slot `{0}` is the collocation and `{1}` its home Meaning's number.
`Display.CollocationHeading` captions the collocation rows under a Meaning, and words that Meaning's muted contents line.
It carries no slot, so neither place shows a count.
`Display.CollocationExample` and `Display.CollocationExampleOne` word a closed row's example marker.
The pair holds a plural and a singular form.
`Display.CollocationAlso` words a closed row's "also N" marker.
Its slot `{0}` is the comma-joined numbers of the further Meanings.
`Notebook.*` labels the Sync page field naming the top Joplin notebook.
`Xiesheng.ReadingsShow` and `Xiesheng.ReadingsHide` label the series member hinge that shows or hides all its readings.
`Settings.Inflection` names the settings page that holds the `Analysis.*` switch and its hint.
`Input.Ancestor*` labels the etymology editor's boxes, their hints and their insert, add, remove and restore controls.
`Relation.*` and `Tongue.*` serve the relation and language pickers of an etymology box.
The five relation names label the built-in relations, which `LAncestorRelation` maps from their stored names.
Each `DetachConfirm` and `DetachCount` pair replaces `DeleteConfirm` while boxes still use the choice.
A failure text is keyed `<Area>.<Action>Failed`, or `<Area>.Failed` for an area's main action.
It sits beside its area's other texts.
Most reach the user through the ledger, but some show in place, such as `Card.VideoFailed`.
An area without other texts, such as `Sentence`, `Fold` or `Box`, sits beside the nearest texts on the same subject.
`Print.*` labels the print window, one key per control, choice, hint, status line and failure.
`Print.Page` names the page number box.
`Print.PageTotal` is one entry so each language orders "of {0}" itself.
`Print.Sheets` and `Print.SheetsOne` give the plural and singular count of paper sheets.
`Print.Idle` is the footer text while nothing prints.

## `ko.json`

The Korean interface catalog, with the same shape and rules as `en.json`.
The keys mirror `en.json` exactly, and numbered placeholders stay for the caller.
Each failure text sits beside its area's other texts at the same place as in `en.json`.
A failure area with no other texts, such as `Sentence`, `Fold` or `Box`, sits where `en.json` puts it.
The person headers spell out person and number instead of the English abbreviations.
`Display.CollocationStub` reads `→ {0} ({1} 참조)`, keeping the same two slots as `en.json`.
`Display.CollocationExample` and `Display.CollocationExampleOne` both read `예문 {0}개`, since Korean counts without a plural.
`Display.CollocationAlso` reads `{0}번 뜻에도 쓰임`, "used in sense N too", rather than a literal "also".
`Print.*` reads the print window in polite Korean, with short noun labels for controls and 합니다 sentences for failures.
`Print.PageTotal` reads `/ {0}`, a slash in place of the English word "of".
`Yunjing.Division` cites slot `{0}`, the raw division, where `en.json` cites the formatted `{1}`.

## Stamp

Commit: `aae5349dbb94`
- `en.json` `c121d28d49d699be`
- `ko.json` `9b50d9f1705db6f6`
