# TReflexLabel.cs
Hash: `16ad740fe5d5075f`

## `public sealed class TReflexLabel`

Covers a reflex row's language label and kind tag after a typed edit.
The row runs on its own STA thread, since the labels look up application resources.
The two stored names get translations for the test and lose them once it ends.

## `public void ReflexTypeRefine_TypedNames_LabelsTheNewNames()`

A lead row over a translated language and kind first prints both translations.
Typing a new language and kind relabels the row under the keys of the typed names.
The typed names have no translation, so the row prints them as typed.
A row keeping the stored keys would still print the previous translations.
