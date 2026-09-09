# PEditorLanguage.cs

## `public partial class PEditor`

The form moved onto the language of the entry it is showing.
The selector, its flag, and the typography the headword and example fields are drawn in all follow from it.
A language is a property of the entry, so filling the form is what moves the panel onto one.

## Inline notes

### `private void PEditorLanguageShow(string language)`

The language selector moved onto the entry's language, flag included.
A language whose pack is no longer on disk is still shown: it is what the entry was written in.

### `_pSpeakerEntry = true;`

Recorded even when the selector already stands on it.
What matters to the language menu being built is that this language is an entry's.
It does not matter that the selector had to move.

### `PHeadwordFontApply(language);`

An entry is shown in the typography its own language declares, not the panel's last choice.
This runs before the equality check, because a reopened form may hold another pack's typography.

### `private void PEditorExampleShow(string language)`

The example typography a language declares, put where every card's example field reads it.
It is set on the panel's resources rather than on each card, so a card added later is already in it.
A language declaring neither family nor size leaves the theme's own value standing.
