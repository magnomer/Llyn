# LLanguage.cs

## `public sealed record LLanguage(`

A loaded language pack: the language's name, the two source lists declared for it, and its transcription schemes.
Loaded from `languages//source.json`.
The engine holds no language-specific facts of its own.
Everything language-specific arrives through this record.

**Parameters**

- `LLanguageFlag` — The pack's flag as an ISO 3166-1 alpha-2 country code (for example `gb`).
  It may instead be the full path of an SVG shipped in the pack folder.
  It is `null` when the pack declares none.
  A code names no shipped image.
  The engine downloads the matching flag from the flag-icons set on demand and caches it in the workspace.
  A pack whose `flag` ends in `.svg` names its own file, for a language no country flag stands for.
- `LLanguageFont` — The typography the pack declares for its own words, held as an [LFont](LFont.comment.md).
  A pack that declares none carries a blank record, and the theme's own typography stands.
- `LLanguageExample` — The typography the pack declares for example sentences, held as an [LFont](LFont.comment.md).
  It stands apart from the headword typography, because a headword is read as a specimen and an example as prose.
  A pack that declares none carries a blank record, and the reading view's own typography stands.
- `LLanguageLookupSources` — The sources the pack declares for reading transcriptions.
- `LLanguageHarvestSources` — The sources the pack declares for finding downloadable recordings.
  The two lists are independent.
  A site good for transcriptions need not serve audio, and either list may stand empty.
- `LLanguageSchemes` — The transcription schemes the pack declares, held as [LScheme](LScheme.comment.md) records, in the order the form shows them.
  An empty list turns the transcription rows off for that language.
  The list seeds new rows only.
  A stored row keeps its scheme after the pack changes.
  Each scheme carries the sources its lookup asks, so a scheme declared as a bare name is typed by hand.
- `LLanguageSeparated` — Whether the language writes a space between its words.
  The pack states it with the top-level key `separator`, `space` or `none`.
  A pack that omits the key is read as separated.
  Japanese, Mandarin and Cantonese declare `none`, so a word there is a run of one script.
- `LLanguageVarieties` — The regional varieties the pack declares, held as [LVariety](LVariety.comment.md) records.
  A reading names its variety by one of these names.
  A pack that declares none carries an empty list, and every reading stands untagged.
- `LLanguageVarietyFlagged` — Whether the UI draws a variety as its flag rather than as its name.
  `true` shows the flag, `false` shows the text.
  A pack that omits the key is read as flagged.
- `LLanguageGloss` — The typography the pack declares for the Glosses under its example sentences, held as an [LFont](LFont.comment.md).
  A Gloss takes the pack of the sentence it renders, since it is read beside that sentence.
  A pack that declares none carries a blank record, and the reading view's own typography stands.
- `LLanguageCleanups` — The rewrite groups the pack declares under `cleanup`, held as [LRespelling](LRespelling.comment.md) records.
  They always run on a reading, after the built-in cleanup and before the trove caches it.
  They exist for site quirks the built-in step cannot know, such as a source writing `ɡ` as `g`.
  A pack that declares none carries an empty list.
- `LLanguageRespellings` — The rewrite groups the pack declares under `respelling`, held as [LRespelling](LRespelling.comment.md) records.
  They recast a cleaned transcription into the pack's preferred symbol convention.
  They run only when the user switches respelling on.
  A pack that declares none carries an empty list.
- `LLanguageFrequencies` — The sources the pack declares for fetching an entry's frequency, held as [LSourceSpec](LSourceSpec.comment.md) records.
  Each carries its own bands.
  The engine asks every one and stores one row per source that answered.
  A pack that declares none carries an empty list, and no frequency is fetched for that language.
  A fetched figure earns the name of the first band of its source that matches it.
- `LLanguageMorphologies` — The sources the pack declares for fetching inflected forms, held as [LSourceSpec](LSourceSpec.comment.md) records.
  Each reading is keyed on a morphology value code written as a decimal string.
  A pack that declares none carries an empty list, and no form is fetched for that language.
- `LLanguageTonal` — Whether the language carries lexical tone, declared with the top-level key `tonal` set to `true`.
  The UI then draws a tone contour under each IPA reading, from the tone marks the reading itself carries.
  A pack that omits the key is read as not tonal, and no contour is drawn.
- `LLanguageGlyph` — The glyph section the pack declares under `glyph`, held as an [LGlyph](LGlyph.comment.md) record.
  It turns the glyph row on for a language written in Han characters.
  A pack that declares none carries `null`, and no glyph row is shown.
- `LLanguageScripts` — The character styles the pack lists under `script`, each an [LScriptStyle](LScriptStyle.comment.md) record, in written order.
  The reading view shows one row per style above the first meaning, and the order here is the row order.
  A pack that lists none carries an empty list, and no script box is shown.
- `LLanguageFanqieBooks` — The rime books the pack lists under `fanqie`, each an [LFanqieBook](LFanqieBook.comment.md) record, in written order.
- `LLanguageShengfu` — The phonetic-series source the pack declares under `shengfu`, or null when it declares none.
  The reading view shows one block per book under the script box, and the order here is the block order.
  A pack that lists none carries an empty list, and no fanqie box is shown.
- `LLanguageHypothesis` — The reconstruction the pack names under `hypothesis`, held as an [LHypothesis](LHypothesis.comment.md) record.
  The fanqie box then prints each placement's reading before the placement.
  A pack that declares none carries `null`, and the box prints the placement alone.
- `LLanguageSilent` — Whether the language takes no pronunciation, declared with the top-level key `silent` set to `true`.
  The input panel then hides its pronunciation rows, so no reading is typed, looked up or downloaded for the entry.
  A pack that omits the key is read as spoken, and the rows stay shown.
- `LLanguageReflexRules` — The fetch rules the pack lists under `reflex`, each an [LReflexRule](LReflexRule.comment.md) record, in written order.
  An entry of the language with no reflex rows is filled once from these, one rule per borrowing language.
  A pack that lists none carries an empty list, and nothing is fetched or shown.
- `LLanguagePhonemic` — Whether a respelled reading is shown between slashes, declared with the top-level key `phonemic` set to `true`.
  The respelling groups of such a pack collapse allophones, so their output is phonemic rather than phonetic.
  The brackets stay square while the original reading is shown, and in a pack that omits the key.
- `LLanguageAnatomies` — The rules the pack declares under `anatomy`, held as [LAnatomyRule](LAnatomyRule.comment.md) records.
  They cut the reflex readings of this pack's entries into onset, vowel, coda and tone, one rule per reflex language.
  Empty for every pack but Classical Chinese, whose file `anatomy.json` holds them.
- `LLanguageAnatomyTones` — The tone correspondence rows the pack declares under `tone`, held as [LAnatomyTone](LAnatomyTone.comment.md) records.
  They say which tone classes a reflex reading's contour may descend from, one row per reflex language.
  Empty for every pack but Classical Chinese, whose file `anatomy_tone.json` holds them.
