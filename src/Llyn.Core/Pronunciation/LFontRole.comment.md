# LFontRole.cs

## `public enum LFontRole`

Which of a language pack's declared typographies is being asked for.
A pack declares one for the word itself and one for the sentences that show it in use.
A third serves the Glosses that render those sentences, and a fourth the character chips of the glyph row.
The two are separate because a headword is read as a specimen and an example is read as prose.

- `LFontRoleHeadword` — The typography the pack declares for its own words.
- `LFontRoleExample` — The typography the pack declares for example sentences.
- `LFontRoleGloss` — The typography the pack declares for the Glosses under its example sentences.
  A Gloss is asked of the pack the sentence is in, whatever language the Gloss itself is written in.
- `LFontRoleGlyph` — The typography the pack declares for the glyph row's character chips.
  A traditional character is read in a serif face, so the pack names one apart from the headword face.
  A pack that declares none falls back to its example typography, which the Han packs keep serif.
