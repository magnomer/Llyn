# LLanguage.cs

## `public sealed record LLanguage(`

A loaded language pack: the language's name, the two source lists declared for it, and its transcription schemes.
Loaded from `languages//source.json`.
The engine holds no language-specific facts of its own.
Everything language-specific arrives through this record.

**Parameters**

- `LLanguageName` — The language's name, matching its folder under `languages/`.
- `LLanguageFlag` — The pack's flag as an ISO 3166-1 alpha-2 country code (for example `gb`).
  It is `null` when the pack declares none.
  The image itself is not shipped.
  The engine downloads the matching flag from the flag-icons set on demand.
  It caches the flag in the workspace.
- `LLanguageFont` — The typography the pack declares for its own words, held as an [LFont](LFont.comment.md).
  A pack that declares none carries a blank record, and the theme's own typography stands.
- `LLanguageExample` — The typography the pack declares for example sentences, held as an [LFont](LFont.comment.md).
  It stands apart from the headword typography, because a headword is read as a specimen and an example as prose.
  A pack that declares none carries a blank record, and the reading view's own typography stands.
- `LLanguageLookupSources` — The sources the pack declares for reading transcriptions.
- `LLanguageHarvestSources` — The sources the pack declares for finding downloadable recordings.
  The two lists are independent.
  A site good for transcriptions need not serve audio, and either list may stand empty.
- `LLanguageSchemes` — The transcription schemes the pack declares, such as Jyutping or Pinyin, in the order the form shows them.
  An empty list turns the transcription line off for that language.
  The list seeds new rows only.
  A stored row keeps its scheme after the pack changes.
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
