# ko.json
Hash: `c0470e663b37fb99`

The Korean interface catalog, with the same shape and rules as `en.json`.

## `terms.*`

The imperfect, future and conditional terms exist so inflection labels can share them.

## `texts`

The keys mirror `en.json` exactly, and numbered placeholders stay for the caller.
Each failure text sits beside its area's other texts at the same place as in `en.json`.
A failure area with no other texts, such as `Sentence`, `Fold` or `Box`, sits where `en.json` puts it.
`Unit.*` follows Korean grammar terms, such as 내용어, 기능어, 형태소 and 단어.
`Inflection.*` uses Korean grammar labels, such as 직설법 and 주완.
`Paradigm.Short` labels the switch button for the short table as 간략.
`Paradigm.Full` labels the switch button for the full table as 전체.
The person headers spell out person and number instead of the English abbreviations.
`Xiesheng.ReadingsShow` and `Xiesheng.ReadingsHide` read 독음 보기 and 독음 숨기기 on the series member hinge.

## Adding a language

A new catalog is listed from the embedded resource names, so no code names a language.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.
