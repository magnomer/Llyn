# PEditorLanguage.cs

## `public partial class PEditor`

The form moved onto the language of the entry it is showing.
The selector, its flag, and the typography the headword and example fields are drawn in all follow from it.
A language is a property of the entry, so filling the form is what moves the panel onto one.

## Inline notes

### `private void PEditorLanguageShow(string language)`

The language selector moved onto the draft's language, flag included.
A language whose pack is no longer on disk is still shown.
It is what the entry was written in.
This runs on every render, so a selector already standing on the language does nothing.
Everything that follows the language moves with it: typography, sentence frames and the parts of speech on offer.

### `_pSpeakerEntry = true;`

Recorded only when the draft moved the selector.
A blank draft carries the language the selector chose, which tells the language menu nothing new.

### `private void PEditorExampleShow(string language)`

The example and Gloss typography a language declares, put where every card's example field reads it.
It is set on the panel's resources rather than on each card.
A card added later is therefore already in it.
A language declaring neither family nor size leaves the theme's own value standing.
