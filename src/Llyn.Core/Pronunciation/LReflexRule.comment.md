# LReflexRule.cs

## `public sealed record LReflexRule(`

One rule a language pack lists under `reflex`, fetching the entry's readings in one borrowing language.
The rule names the web page that carries the reading and the pattern that reads it out.
The engine knows nothing of the site, only that a page is fetched and matched.
Every URL, field and pattern is pack data, so a new language needs no code.

**Parameters**

- `LReflexRuleLanguage` — The borrowing language the rows are stored under, such as Korean.
- `LReflexRuleUrl` — The address fetched, with `{word}` replaced by the escaped character.
- `LReflexRuleForm` — The form fields posted, in pack order, with `{word}` replaced by the character.
  A rule with no field is fetched with a GET instead.
- `LReflexRulePattern` — A .NET regex over the page, with `{word}` replaced, one reading per match.
  Its named group `text` is the reading, `kind` and `note` the kind before it and the note after it.
  Its group `main` is non-empty when the reading is in common use.
  Any other named group is only there for the format.
- `LReflexRuleTemplate` — The template the stored text is built from, `{name}` standing for a named group.
  It defaults to the `text` group alone, and `{sound} | {sense}` joins two groups.
  A `[[...]]` segment is printed only when every slot inside it was captured.
- `LReflexRuleEvery` — True to keep every match as its own row, false to keep the first alone.
- `LReflexRuleBusy` — A regex matched over the answer that means the site refused the request, or `null`.
  A refusal is a throttle, not a miss, so the character is asked again later.
- `LReflexRuleInterval` — Seconds the engine waits after one fetch before the next to any rule.
- `LReflexRuleHeaders` — Request headers sent with the fetch, such as the Referer a search API demands.
- `LReflexRuleEpithet` — The template of the epithet shown after the headword in every list, or `null`.
  `{text}`, `{kind}` and `{note}` stand for the stored row, and `{note} {text}` prints `희롱할 롱(농)`.
  Only a rule with a template feeds the epithet, and the rows of every such rule are joined by commas.
- `LReflexRuleClip` — A regex whose matches are cut out of each epithet piece, or `null`.
  `\(.*\)` cuts the `(농)` a Korean reading carries after its 두음법칙 form.
- `LReflexRuleFirst` — True to mark the first row as the one in common use when no match captured `main`.
  A dictionary listing the representative reading first needs no `main` group at all.
- `LReflexRuleRewrite` — Ordered rewrite rules applied to the formatted text before it is stored.
  `/([^/]+)/` to `[$1]` turns `/lʊŋ²²/, /nʊŋ²²/` into `[lʊŋ²²], [nʊŋ²²]`.

## `public const string LReflexRuleText = "text";`

The named group carrying the reading itself.

## `public const string LReflexRuleKind = "kind";`

The named group carrying the kind printed before the reading, such as Go-on.

## `public const string LReflexRuleNote = "note";`

The named group carrying the note printed after the reading, such as a pinyin or a Korean 훈.

## `public const string LReflexRuleMain = "main";`

The named group whose non-empty capture marks the reading as the one in common use.
