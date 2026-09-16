# LEngineParadigm.cs

## `public sealed partial class LEngine`

The paradigm side of the engine.
It answers which forms an entry is expected to have and which of them are stored.
A language pack declares the forms by code on a part of speech.
The engine resolves those codes to the workspace's rows and pairs each with a stored inflection.
A slot a reached source could not fill this session reads unknown, as `LEngineInflectionFetch.cs` records it.
The pack is read once per language through `LSpeechLoader` and kept beside the language packs.
The cache is cleared with the workspace, as the language packs are.

## `public IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)`

Reads the expected forms of the entry identified by `entryId`, one slot per form, in part-of-speech order.
The read never starts a fetch, because the caller decides when to ask the web.
An entry the workspace does not hold answers an empty list.
So does a headword whose parts of speech declare no paradigm, because it does not inflect.

## `private IReadOnlyList<LParadigmSlot> LEngineParadigmRead(LEntry entry)`

The same read for an entry already in hand, called under the gate.
The public seams and the fetch share it so an entry is read once per question.

## `public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)`

Reads the slots of the entry as `LEngineParadigmRead` does and drops those the display need not show.
A specified slot whose stored form carries the regular flag is dropped.
So a noun whose plural is `cats` shows no plural row, while `mice` keeps one.
A paradigm stating no rules keeps every row, which is why verb forms always show.
An empty answer means the headword is not inflecting for display.
The flag was derived when the form was stored, so showing runs no rule.

## `private void LEngineParadigmUpdate(long entryId)`

Derives the regular flag of every stored form of the entry `entryId` names, or does nothing for a missing entry.

## `private void LEngineParadigmUpdate(LEntry entry)`

Judges every answered slot of the entry by `LEngineParadigmMatch` and stores the flag where it changed.
An entry without a language has no pack and no paradigm, so nothing is judged.
It runs whenever the forms, the headword or the parts of speech are stored, since the judgement reads all three.

## `private static IReadOnlyList<LParadigm> LEngineParadigmScan(LSpeechPack pack, long speechCode)`

Collects the paradigms that apply to the part of speech named by `speechCode`.
It first walks the part's parent chain, nearest first.
The walk stops at a repeated code or after as many hops as the pack has parts.
So a pack naming its parents in a cycle still ends.
A paradigm declared on any code of the chain applies.
It is skipped when one of its excepted codes is also on the chain.
So a plural declared on nouns reaches countable nouns and stops at uncountable ones.
A code of `0` answers nothing.

## `private IReadOnlyList<LParadigmSlot> LEngineParadigmResolve(LSpeech speech, LSpeechPack pack, IReadOnlyList<LInflection> stored, HashSet<long>? missed)`

Turns one part of speech the entry carries into its slots.
A part typed by hand has no code and answers nothing.
Each code the paradigm names is resolved to a morphology row, and a code the workspace lacks is skipped.
A morphology row already given a slot by a nearer paradigm is not given a second.
The slot's inflection is the first stored one carrying that row and naming this part or none.
A slot with an inflection is specified.
A slot without one is unknown when its morphology id is in `missed`, and unspecified otherwise.
Each slot keeps the paradigm it came from, so the display seam can judge regularity without scanning again.

## `private static LMorphology? LEngineMorphologyResolve(LSpeechPack pack, LMorphologyArchive morphologies, string language, long code)`

Resolves one morphology value code to the workspace row the pack seeded.
The pack says which feature and which part the code belongs to.
The archive is asked for exactly that row.
So a pack reusing a value code under two features still binds the right one.
A code the pack does not declare answers `null`.

## `internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)`

Reports whether `form` is a regular inflection of `headword` under the paradigm's rules.
Each rule matching the headword predicts a spelling, and the form is regular when it equals any prediction, case ignored.
A paradigm stating no rules answers false, so a form is never regular by default.
An empty headword or form answers false.
