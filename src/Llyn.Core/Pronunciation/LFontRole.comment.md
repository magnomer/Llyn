# LFontRole.cs

## `public enum LFontRole`

Which of a language pack's declared typographies is being asked for.
A pack declares one for the word itself, one for the sentences that show it in use, and one for the Glosses that render those sentences.
The two are separate because a headword is read as a specimen and an example is read as prose.

- `LFontRoleHeadword` — The typography the pack declares for its own words.
- `LFontRoleExample` — The typography the pack declares for example sentences.
- `LFontRoleGloss` — The typography the pack declares for the Glosses under its example sentences.
  A Gloss is asked of the pack the sentence is in, whatever language the Gloss itself is written in.
