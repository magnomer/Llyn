# LDiweiClerk.cs

## `public sealed class LDiweiClerk`

Diwei read, found and composed into a page, with the tally of anchored reflexes.
The rows themselves are rebuilt by the fanqie clerk when a fetch lands.

## `public LDiweiClerk(LRig rig, LLanguageCache languages)`

Reads the diwei, entry and reflex ports out of `rig`.

## `public IReadOnlyList<LDiwei> LDiweiClerkRead(string language, string kind)`

The diwei of one kind in one language.

## `public LDiwei? LDiweiClerkRead(long? id)`

One diwei by id, or null for no id.

## `public LDiwei? LDiweiClerkFind(string language, string kind, string key)`

The diwei of one kind with the given key.

## `public IReadOnlyList<LFanqieRow> LDiweiFanqieRead(LDiwei diwei)`

The fanqie rows filed under the diwei.

## `public IReadOnlyList<LEntry> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds, string query)`

The entries filed under all of the diwei that match `query`.

## `public LDiweiPage LDiweiPageRead(LDiwei diwei, bool switched, bool tallied, Func<string, string?> localize)`

The page of one diwei, its sections scanned from the rows, the hypothesis and the tally.
`switched` says whether respellings show, and the tally shows only when both flags stand.

## `public IReadOnlyList<LTally> LTallyRead(LDiwei diwei)`

The reflexes anchored to the diwei's rows, grouped by heading and character.
The heading is the division, or the place of articulation for a rime.
Languages are ranked in the order of the reflex rules.

## `private LHypothesis? LHypothesisRead(string language)`

The hypothesis of `language`, or null for a blank language.

## `private IReadOnlyList<LReflexRule> LReflexRuleRead(string language)`

The reflex rules of `language`, or none for a blank language.

## `private static string LTallyHeadingRead(string kind, LFanqieRow row, LHypothesis? hypothesis)`

The tally heading of one row under the diwei's kind.

## `private static void LTallyLoad(Dictionary<string, IReadOnlyList<LReflex>> readings, LFanqieRow row, IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored)`

Adds the reflexes anchored to `row` under its character, without repeating a reflex.
