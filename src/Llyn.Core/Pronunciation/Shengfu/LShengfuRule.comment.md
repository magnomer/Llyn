# LShengfuRule.cs

## `public sealed record LShengfuRule(`

The pack section that says where a language's phonetic series are read from.
The engine holds no series of its own, so a pack without this section prints none.

**Parameters**

- `LShengfuRuleUrl` — The address fetched per character, with `{word}` standing for it.
- `LShengfuRulePattern` — The regex whose `shengfu` group carries one series.
- `LShengfuRuleForm` — The form fields posted instead of a plain fetch, empty for a fetch.
- `LShengfuRuleSource` — The name of the site the series is credited to.
- `LShengfuRuleBusy` — The regex that marks a refusal page, so nothing is stored.
- `LShengfuRuleInterval` — The seconds left between two fetches of this source.
- `LShengfuRuleSeparator` — The text joining several series of one character.

## `public const string LShengfuRuleGroup`

The regex group name the pattern carries each series in.
