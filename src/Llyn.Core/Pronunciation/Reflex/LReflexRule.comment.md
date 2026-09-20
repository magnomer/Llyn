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
  Every slash and bracket is dropped after them, so a stored reading is always bare.
- `LReflexRuleRegion` — The place every row of the rule is taken from, such as `Shanghai`, or `null`.
  The view shows it when the language name is hovered.
- `LReflexRuleSplit` — A regex the text and the note are both cut on, or `null` to keep each whole.
  `\s*[,/]\s*` turns `ɣaʔ², ɣəʔ²` under `ghah4 / gheh4` into two rows, each reading paired with its own note by position.
  When the two lists differ in length every reading keeps the whole note.
- `LReflexRuleRemark` — A regex read after the match for what the page says of one reading, or `null`.
  `{note}` stands for the row's note, and the named group `remark` is stored.
  `<li>{note} - (?<remark>[^<]*?)[;.]?</li>` reads `literary` out of `<li>ok - literary;</li>`.
- `LReflexRuleUntil` — A regex ending the stretch of page the remark is looked for in, or `null`.
  Without it the remark is looked for to the end of the page.
  The next language header keeps a Hakka reading from taking a Southern Min note.
- `LReflexRuleFolded` — True when the rows of the rule are folded away under the visible rows.
  The view and the editor show them only after the fold is opened.
- `LReflexRuleRecast` — Ordered rewrite rules applied to each note piece before it is stored.
  `^(\d)(.+)$` to `$2$1` moves the tone Wugniu writes first to the end, so `7oq` is stored as `oq7`.
  The remark is still looked for under the note as the page wrote it.
- `LReflexRuleSuperscript` — True to store every digit of the note as a superscript, so `oq7` becomes `oq⁷`.
  The digits are raised after the recasts, so a recast still sees plain digits.

## `public const string LReflexRuleText = "text";`

The named group carrying the reading itself.

## `public const string LReflexRuleKind = "kind";`

The named group carrying the kind printed before the reading, such as Go-on.

## `public const string LReflexRuleNote = "note";`

The named group carrying the note printed after the reading, such as a pinyin or a Korean 훈.

## `public const string LReflexRuleMain = "main";`

The named group whose non-empty capture marks the reading as the one in common use.

## `public const string LReflexRuleSense = "remark";`

The named group of the remark pattern carrying what the page says of the reading.
