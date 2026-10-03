# TParadigm.cs
Hash: `7fe3ad57dfa43ab2`

## `public sealed class TParadigm`

Covers what a language pack declares about inflection.
That is which part specialises which, and which forms a part takes.
The loader tests write a pack of their own into a temporary language folder and load it.
So the vocabulary loader is read as it reads a real pack, and nothing shipped is touched.
The other tests read the shipped packs or build rows in memory.

## `public void SpeechPackLoad_PartNamingParent_ReadsParentCode()`

A part naming its parent carries that code, and one naming none or naming it badly carries `0`.

## `public void SpeechPackLoad_ParadigmDeclared_ReadsValuesInOrder()`

A paradigm keeps its values in the order the pack lists them, because that order is the display order.
A regular-form rule keeps its pattern as written.
A rule with a bad pattern or shape is skipped, and the paradigm is kept.

## `public void SpeechPackLoad_ParadigmRowMalformed_SkipsRow()`

A row whose part or any value is not a positive integer is dropped, and its neighbours are kept.
A malformed regular or except entry is ignored and the row is kept.

## `public void SpeechPackLoad_EnglishPack_DeclaresParadigms()`

The shipped English pack states parents and paradigms, so the loader is read against disk and not a fixture alone.

## `public void SpeechPackLoad_ClassicalPack_DeclaresPrincipalParts(`

The shipped Latin and Greek packs each declare three paradigms.
The noun asks for the genitive alone and excepts the proper noun, the verb for its principal parts.
Every value a paradigm names is a morphology value the same pack declares.

## `public void ParadigmRowScan_TwoPartsOfSpeech_LeadsEachPartOnce()`

Slots sharing one form fold into one row, and the first row of each part of speech leads that part.
An uncertain first slot marks its row, and no slots give no rows.
The table prints a part name once per part, so a lost or doubled lead would drop or repeat it.

## `public void ParadigmClerkCheck_WrittenOrEmptyForm_AnswersTextOrAbsentWhateverIsPending()`

A written form answers text and an empty one answers absent, whatever the pending and morphology verdicts say.

## `public void ParadigmClerkCheck_NoFormUnknownSlot_AnswersUnknownBeforePendingOrLost()`

An unknown slot without a form answers unknown before the pending and morphology verdicts are weighed.

## `public void ParadigmClerkCheck_NoFormUnansweredSlot_AnswersPendingLostOrAbsent()`

An unanswered slot answers pending while the fetch runs, then lost when morphology is on, else absent.

## Inline notes

### `private static LSpeechPack TParadigmPackLoad(string json)`

Writes one vocabulary file under the application's language folder and removes it after the load.

### `private static LParadigmStatus TParadigmStatusRead(LInflection? inflection, LState state, bool pending, bool enabled)`

Builds a one-slot row and asks it for its status.
