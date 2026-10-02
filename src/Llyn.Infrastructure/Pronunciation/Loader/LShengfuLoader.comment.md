# LShengfuLoader.cs
Hash: `68404391d41adc01`

## `internal static class LShengfuLoader`

The `shengfu` side of the pack loader.
It reads the one phonetic-series lookup a Han language pack may declare.
`LLanguageLoader` calls it.

## `private const string LShengfuKey = "shengfu";`

The key of the series block.

## `public static LShengfuRule? LShengfuPackRead(JsonElement root)`

Reads the pack's `shengfu` block into one rule, or null when the pack declares none.
A block without an address or a pattern is no rule, because neither has a sensible default.
The separator defaults to a middle dot, which joins the series of a character with two.
