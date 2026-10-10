# LParadigmClerk.cs
Hash: `c5d0b5bf0537b70b`

## `public sealed class LParadigmClerk`

The clerk over paradigms.
It answers which forms an entry is expected to have and which of them are stored.
A language pack declares the forms by code on a part of speech.
The clerk resolves those codes to the workspace's rows and pairs each with a stored inflection.
Persisted lacunae distinguish reached-but-missing forms from slots that have not been answered.
A lacuna without a cell key or morphology id names no slot and is ignored.
Speech packs remain cached for the clerk's lifetime.

## `public LParadigmClerk(LRig rig, LLanguageCache languages)`

Reads the entry, inflection, lacuna, morphology and speech ports out of `rig`.
Keeps `languages` to reach the rule book a pack names under `inflection`.

## `public IReadOnlyList<LParadigmSlot> LParadigmClerkRead(LEntry entry)`

Reads every slot of an entry already in hand, shown or not.
The background fetch check shares it, so an entry is read once per question.
Lacuna rows with a cell key mark missed multi-value cells, and the rest mark one-value slots by id.

## `public IReadOnlyList<LParadigmSlot> LParadigmClerkShow(long entryId)`

Reads the slots of the entry and drops those the display need not show.
A specified slot whose stored form carries the regular flag is dropped.
So a noun whose plural is `cats` shows no plural row, while `mice` keeps one.
The stored regular flag controls hiding even when the paradigm declares no fallback rules.
The flag was derived when the form was stored, so showing runs no rule.

## `public IReadOnlyList<LParadigmRow> LParadigmRowRead(long entryId)`

The display slots of an entry joined into the rows a paradigm box shows.
Slots declared on the part a rule book's layout names are dropped.
The inflection view shows that part instead, so the list never repeats it.
A pack without a layout keeps every display slot.

## `public string LParadigmLanguageRead(long entryId)`

The language the entry's slots are written in, or empty when it has none.
It reads every slot, not only the shown ones.
So an entry whose forms are all regular still names its language for the view's font.

## `public LParadigmView? LParadigmClerkBuild(long entryId, bool pending, bool enabled, bool held, bool custom)`

Builds the entry's inflection view from its slots and the layout of the pack's rule book.
It answers null for a missing entry, a blank language, or a pack without a layout.
It also answers null when no slot lies on the layout's part.
Custom cells carry the stored marks, widened to the root or ending when the cell is divided.
It hands the view the language's rule book and the headword, so custom cells can split root from ending.
`pending` and `enabled` feed each cell's status, as they do for the list.
`held` passes on unread to the view, which picks each lost cell's tip by it.
`custom` picks the custom sheets with their marks, or the default sheets without marks.

## `private LLanguage? LParadigmClerkFind(LEntry entry)`

The language pack the entry is written in, read through the language cache.
An entry without a language has no pack, so it answers null.
The list, the view and the judgement all reach the rule book and its layout through it.

## `public void LParadigmClerkUpdate(LEntry entry)`

Judges every answered slot of the entry and stores the result where it changed.
A pack without a rule book judges by `LParadigmClerkMatch` and stores the flag alone.
A pack with a book predicts each answered slot once with `LInflectionBookResolve`.
A covered slot's stored text is then compared with that prediction by `LInflectionDifference.LInflectionDifferenceScan`, through the book's folds.
It stores the prediction, the marks, the book's stamp and the flag together through `LInflectionAnalysisSave`.
A slot the book does not cover stores a null prediction and null marks.
It carries the book's stamp all the same.
So the background check reads an uncovered row as analysed by this book and leaves it alone.
Its flag falls back to `LParadigmClerkMatch`, so the pack's own `regular` rules judge it.
A covered form is regular when nothing is marked and it is not empty.
So an absent form never counts as regular and stays shown.
An entry without a language has no pack and no paradigm, so nothing is judged.
Analysis depends on the forms, headword and parts of speech, so callers must refresh it after those change.
It also runs when the background fetch check finds an analysis made by an older book.
It reads only stored text and never the network.

## `private static IReadOnlyList<LParadigm> LParadigmClerkScan(LSpeechPack pack, long speechCode)`

Collects the paradigms that apply to the part of speech named by `speechCode`.
It first walks the part's parent chain, nearest first.
The walk stops at a repeated code or after as many hops as the pack has parts.
A paradigm declared on any code of the chain applies.
It is skipped when one of its excepted codes is also on the chain.
So a plural declared on nouns reaches countable nouns and stops at uncountable ones.

## `private IReadOnlyList<LParadigmSlot> LParadigmClerkResolve(LSpeech speech, LSpeechPack pack, IReadOnlyList<LInflection> stored, HashSet<long> missed, HashSet<string> missedCells)`

Turns one part of speech the entry carries into its slots.
A part typed by hand has no code and answers nothing.
Each code of a cell is resolved to a morphology row.
A cell with a code the workspace lacks is skipped.
A set of rows already given a slot by a nearer paradigm is not given a second.
The slot's inflection is the first stored one carrying every row of the cell and naming this part or none.
A one-value slot without one is unknown when its morphology id is in `missed`.
Any slot without one is unknown when its key is in `missedCells`, and unspecified otherwise.
Each slot keeps the paradigm it came from, so the display seam can judge regularity without scanning again.

## `private static LMorphology? LMorphologyResolve(LSpeechPack pack, LMorphologyVault morphologies, string language, long code)`

Resolves one morphology value code to the workspace row the pack seeded.
The pack says which feature and which part the code belongs to.
The archive is asked for exactly that row.
A code the pack does not declare answers `null`.

## `public static LParadigmShown LParadigmClerkCheck(LParadigmRow row, bool pending, bool enabled, bool held)`

Answers the text and tip key that stand in the row for its form.
It asks `LParadigmSlot.LParadigmSlotShow` of the row's first slot.
So the list and the inflection view share one status rule and one wording.

## `public static bool LParadigmClerkMatch(LParadigm paradigm, string headword, string form)`

Reports whether `form` is a regular inflection of `headword` under the paradigm's rules.
Each rule matching the headword predicts a spelling, and the form is regular when it equals any prediction, case ignored.
A paradigm stating no rules answers false, so a form is never regular by default.
An empty headword or form answers false.
