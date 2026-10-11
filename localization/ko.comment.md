# ko.json
Hash: `59462acab219034c`

The Korean interface catalog, with the same shape and rules as `en.json`.

## `terms.*`

The imperfect, future and conditional terms exist so inflection labels can share them.
The sync term names the push to Joplin, shared by the sync button and the Sync settings page.
The print, cancel, color and custom terms read 인쇄, 취소, 색상 and 사용자 지정.

## `texts`

The keys mirror `en.json` exactly, and numbered placeholders stay for the caller.
Each failure text sits beside its area's other texts at the same place as in `en.json`.
A failure area with no other texts, such as `Sentence`, `Fold` or `Box`, sits where `en.json` puts it.
`Unit.*` reads 단위, with the choices 형태소, 단어 and 표현.
`Freedom.*` reads 자립성, with the choices 의존 and 자립.
`Inflection.*` uses Korean grammar labels, such as 직설법 and 주완.
`Paradigm.Short` labels the switch button for the short table as 간략.
`Paradigm.Full` labels the switch button for the full table as 전체.
The person headers spell out person and number instead of the English abbreviations.
`Xiesheng.ReadingsShow` and `Xiesheng.ReadingsHide` read 독음 보기 and 독음 숨기기 on the series member hinge.
`Print.*` reads the print window in polite Korean, with short noun labels for controls and 합니다 sentences for failures.
`Print.PageTotal` reads `/ {0}`, a slash in place of the English word "of".
`Yunjing.Division` cites slot `{0}`, the raw division, where `en.json` cites the formatted `{1}`.

## Adding a language

A new catalog is listed from the embedded resource names, so no code lists the languages.
Its file still needs an `EmbeddedResource` line in `Llyn.Infrastructure.csproj`.
The file name must be a culture name, since the terms are cased under that culture.
