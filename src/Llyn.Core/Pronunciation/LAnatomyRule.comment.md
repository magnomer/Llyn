# LAnatomyRule.cs

## `public sealed record LAnatomyRule(`

One rule of the Classical Chinese pack's `anatomy` file: how the readings of some reflex languages are cut.
A rule names the languages it serves and holds one pattern for the reading and one for the respelling.
The rules live in the Classical Chinese pack alone, since only its entries carry reflex rows.

**Parameters**

- `LAnatomyRuleLanguages` — The reflex languages the rule cuts, such as `Korean`, or a list of Sinitic ones.
- `LAnatomyRuleIpa` — The pattern cutting the reading as fetched.
- `LAnatomyRuleRespelling` — The pattern cutting the respelling, the IPA pattern again when the file names none.

## `public bool LAnatomyRuleMatch(string language)`

Whether the rule names this language, compared without regard to case.

## `public LAnatomy LAnatomyRuleResolve(string text, string respelling)`

The anatomy of one reading: the IPA pattern over the text, the respelling pattern over the respelling.
A blank respelling is cut from the text instead, so the respelling set is never empty for want of one.
