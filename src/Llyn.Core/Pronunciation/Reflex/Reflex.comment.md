# Reflex

This folder holds the records for reflexes, the readings of a character in borrowing languages.
It also holds audio-source answers and ordered respelling rules.
The rule records carry pack data, while the guises carry display choices.
The engine knows only that a page is fetched, matched and rewritten.
Every URL, pattern and symbol lives in the language pack, so a new language needs no code.

## `LRecording`

What one audio source had to say about a headword.
A source that answered produces one per reading it returned, exactly as `LCandidate` does for a lookup.
A source that had nothing or was never reached still produces exactly one.
`LRecordingSource` is the name the language pack declares, config-driven and never an enum.
`LRecordingAddress` is the absolute audio URL, or null when the source offered none.
`LRecordingOrder` is the source's position in the pack's `audio` list.
It carries the declared order to the menu, which fetching in parallel would otherwise lose.
`LRecordingReached` tells whether the source answered at all.
A row with no address reads as "no entry" or "failed to retrieve" from this alone.
`LRecordingVariety` mirrors `LCandidateVariety` and names the saved file's variety suffix.
It is empty when the source does not say, or when the row carries no address.

## `LReflexGuise`

How one reflex row prints, as the engine answers it for the row's language.
`LReflexGuiseRespelled` is true when the language shows its respelling under the current switch.
`LReflexGuisePhonemic` is true when the language writes its readings as phonemes.
`LReflexGuiseFolded` is true when the pack of the entry's language folds the row's language away.
`LReflexGuiseShown` is filled once by the engine through `LReflexGuiseRead`.

## `LReflexGuiseFind`

Guises run by position beside their reflex rows.
An index past the list, or below zero, answers null, so a row without a guise prints plain.
The Outpost entry sheet and the series members read their guises through it.

## `LReflexGuiseRead`

The one owner of the respelling choice.
It is the respelling while the guise is respelled and one is filled, and the phonetic otherwise.
`LLanguageClerk.LLanguageGuiseScan` applies it once per row and keeps the answer in `LReflexGuiseShown`.

## `LReflexOrder`

The order a pack declares for the reflex rows of its entries.
It comes from the pack's `order` key, apart from the fetch rules.
So the order of the fetch rules never reaches the screen.
`LReflexOrderLanguages` lists the borrowing languages in row order.
`LReflexOrderKinds` lists, per language, the kinds in row order.

## `LReflexOrderEmpty`

The order of a pack that declares none.
Its rows then list by language name and kind name.

## `LReflexOrderSort`

The one rule ordering the reflex rows of an entry for every view.
Declared languages come first in their declared order, and the rest follow by name.
Within a language, declared kinds come first in their order, and the rest follow by name.
Within a language and kind, the main row comes first, and the rest follow by reading text.
The stored id is only the last tie-break, so the result is stable.
Storage order never decides, since a fetch stores rows in whatever order its rules run.

## `LReflexOrderFind`

The place of a name in a declared list.
A name the list lacks sorts after every declared one.
The match is exact, since row names are written from the same pack that declares the list.
`LTallyLineScan` ranks its lines with it too, so both lists order a pack alike.

## `LReflexRule`

One rule a language pack lists under `reflex`, fetching the entry's readings in one borrowing language.
The rule names the web page that carries the reading and the pattern that reads it out.
The engine knows nothing of the site.
`{word}` in the URL, form fields and pattern is replaced by the character.
A rule with no form field is fetched with a GET instead of a POST.
`LReflexRulePattern` is a .NET regex with one reading per match.
Its named group `text` is the reading, `kind` the kind before it, and `romanization` its romanized form.
The groups `meaning` and `note` carry the lexical gloss and the source annotation.
The group `main` is non-empty when the reading is in common use.
Any other named group is only there for the format.
`LReflexRuleTemplate` builds the stored text, with `{name}` standing for a named group.
It defaults to the `text` group alone.
A `[[...]]` segment is printed only when every slot inside it was captured.
`LReflexRuleEvery` keeps every match as its own row, and false keeps the first alone.
`LReflexRuleBusy` is a regex over the answer that means the site refused the request.
A refusal is a throttle, not a miss, so the character is asked again later.
`LReflexRuleInterval` is the seconds the engine waits after one fetch before the next to any rule.
`LReflexRuleHeaders` are request headers, such as the Referer a search API demands.
`LReflexRuleEpithet` is the template of the epithet shown after the headword in every list.
Only a rule with a template feeds the epithet, and the rows of every such rule are joined by commas.
`LReflexRuleClip` cuts its matches out of each epithet piece.
`\(.*\)` cuts the `(농)` a Korean reading carries after its 두음법칙 form.
`LReflexRuleFirst` marks the first row as the one in common use when no match captured `main`.
A dictionary listing the representative reading first needs no `main` group at all.
`LReflexRuleRewrite` holds ordered rewrite rules applied to the formatted text before it is stored.
Every slash and bracket is dropped after them, so a stored reading is always bare.
`LReflexRuleRegion` is the place every row is taken from, such as `Shanghai`.
The view shows it when the language name is hovered.
`LReflexRuleSplit` is a regex the text and romanization are both cut on.
Each reading is paired with its own romanization by position.
When the two lists differ in length every reading keeps the whole romanization.
`LReflexRuleGloss` is a regex read after the match for the note and meaning of one reading.
`{romanization}` stands for the row's romanization.
`LReflexRuleUntil` ends the stretch of page the gloss is looked for in.
Without it the gloss is looked for to the end of the page.
The next language header keeps a Hakka reading from taking a Southern Min note.
`LReflexRuleFolded` folds the rows of the rule away under the visible rows.
The view and the editor show them only after the fold is opened.
`LReflexRuleRecast` holds ordered rewrite rules applied to each romanization piece before it is stored.
`^(\d)(.+)$` to `$2$1` moves the tone Wugniu writes first to the end, so `7oq` is stored as `oq7`.
The gloss is still looked for under the romanization as the page wrote it.
`LReflexRuleSuperscript` stores every digit of the romanization as a superscript, so `oq7` becomes `oq⁷`.
The digits are raised after the recasts, so a recast still sees plain digits.

## `LReflexRuleText`

The named group carrying the reading itself.

## `LReflexRuleKind`

The named group carrying the kind printed before the reading, such as Go-on.

## `LReflexRuleRomanization`

The named group carrying the romanized reading.

## `LReflexRuleMeaning`

The named group carrying the lexical meaning paired with the reading.

## `LReflexRuleNote`

The named group carrying what the source says of the reading, such as `literary`.

## `LReflexRuleMain`

The named group whose non-empty capture marks the reading as the one in common use.

## `LReflexSource`

The port that asks one reflex rule's site for one character's modern readings.
It is a fetch over the network, not storage, so it is not a Vault.
`LReflexSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.
It answers the drafts the rule reads and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.

## `LRespelling`

One group of ordered rewrite rules a language pack declares, scoped to the regional varieties it names.
A pack recasts a looked-up transcription into its preferred symbol convention through these groups.
Nothing consumes the groups until the lookup applies them and the user switches respelling on.
`LRespellingVarieties` is compared ordinally against a candidate's tag.
An empty list applies the group to every candidate, tagged or not.
A non-empty list matches an untagged candidate only when it contains the empty tag.

## `LRespellingResolve`

Returns the transcription unchanged when the variety falls outside the group's scope.
Otherwise it runs every rule in written order, each on the previous rule's output.
The input is NFC-normalized once before the first rule.
So a combining mark and its precomposed form from different sites match the same pattern.

## `LRespellingScan`

Applies a whole pack's groups in list order, feeding the output of one into the next.
An empty list returns the input untouched.

## `LRespellingStaleCheck`

True when a stored respelling was derived by older rules of the same pack.
The groups run over the stored respelling, and the result must equal the groups run over the reading.
The stored respelling must differ from that derivation, so a current one is never stale.
A respelling written by hand rarely rewrites to the derivation, so it stays.
A blank respelling is never stale, since filling blanks is a separate step.

## `LRespellingRule`

One rewrite step a language pack declares, a regex pattern and the text that replaces every match.
The engine knows nothing of the symbols involved.
Every symbol lives in the pack, so a convention changes without a recompile.
The replacement is a .NET replacement string, so `$1` may echo a captured group.
The pattern is compiled once when the record is built, culture-invariant so no locale bends a class.
An invalid pattern throws there, so a bad pack fails while it loads rather than during a lookup.
The compiled form is a private field, so it never enters the record's equality.

## `LRespellingRuleResolve`

Runs the rule over the whole string and returns the rewritten form.

## `Equals`

Two rules are equal when their pattern and replacement text match ordinally.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.
`GetHashCode` hashes the same two strings.

## Stamp

Commit: `aae5349dbb94`
- `LRecording.cs` `1f78459a2bd0625b`
- `LReflexGuise.cs` `c886a7cc91060e18`
- `LReflexOrder.cs` `517b53769a2f330d`
- `LReflexRule.cs` `fc8e93cfc84b13ae`
- `LReflexSource.cs` `38631f0e411b057b`
- `LRespelling.cs` `d2fc98a73dd5b35b`
- `LRespellingRule.cs` `57925fff177f64f7`
