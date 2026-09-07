# LFontRole.cs

## `public enum LFontRole`

Which of a language pack's declared typographies is being asked for.
A pack declares one for the word itself and one for the sentences that show it in use.
The two are separate because a headword is read as a specimen and an example is read as prose.

- `LFontRoleHeadword` — The typography the pack declares for its own words.
- `LFontRoleExample` — The typography the pack declares for example sentences.
