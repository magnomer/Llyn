# LLanguageLoaderShengfu.cs

## `private static LShengfuRule? LLanguageShengfuRead(JsonElement root)`

Reads the pack's `shengfu` block into one rule, or null when the pack declares none.
A block without an address or a pattern is no rule, because neither has a sensible default.
The separator defaults to a middle dot, which joins the series of a character with two.
