# Speech

Speech holds the parts of speech an entry carries and the vocabulary a language pack declares for them.
A pack is the only place a language's vocabulary is stated, so no such fact is compiled in.

## `LSpeech`

One part-of-speech assignment on an entry, ordered within it, identified by entry and position.
A row says its part of speech in exactly one of two ways.
The first is a link to an `LSpeechValue` row, whose display name is read from that row and never copied.
The second is the text the user typed when no value names it.
The second way exists because the field is editable.
A user may name a part their pack does not declare.
Refusing to store it would lose what they typed.
A custom row is text and stays text, so renaming a value never touches it and it resolves against nothing.

## `LSpeechDraft`

One part of speech as the input form carries it.
It is either a value row the language declares or a typed name.
The draft keeps the two apart so a custom name never imports as a link.
The display name falls back to the custom text when none was resolved.
`LSpeechDraftCreate` from a typed name leaves a decision to the engine.
The engine decides at the write whether a value row names it.
`LSpeechDraftEmpty` is true when the draft links no value and carries no custom name.

## `LSpeechOffer`

The parts of speech the language offers for the typed text, with the verdicts the offer needs.

## `LSpeechPack`

The display vocabulary one language pack declares.
It lists the parts, the features each takes and the values of each feature.
It is loaded from `languages/<Name>/vocabulary.json` and upserted into `speech_value`, `morphology_feature` and `morphology_value`.
Rows in a pack have row id 0.
Their parent links hold the parent's code, which the import resolves to row ids.
`LSpeechPackRetirements` is never null, so the import walks it without a check.
`LSpeechPackSort` is the one rule that orders a language's parts for every view.
Values the pack declares come first, in the order the pack lists them.
Values the user typed follow, alphabetical in the current culture without case.
A stored position or row id never decides the order, because import and typing fill them by arrival.
The row id breaks only a tie between identical names, so the result is stable.
A stored value the pack no longer lists sorts with the typed ones, since nothing declares its place.
`LSpeechPackFind` gives `int.MaxValue` when the pack does not declare the code.
A code of zero or less is never a pack code, since a typed value takes a negative one.

## `LSpeechRetirement`

One part of speech a language pack once published and has since removed.
A pack never reuses a published code, so it keeps the code here and says where its links go.
The import applies every retirement on each start, and one already applied finds nothing to do.

## `LSpeechRow`

One part of speech the language offers the draft's speech field.

## `LSpeechValue`

One row of a language's part-of-speech vocabulary.
Lexical rows link it by id and never copy its name.
A pack row carries the pack's own number as its code.
Re-seeding a pack upserts on language and code, so a renamed preset keeps its row id.
A value the user added from the editor carries a negative code, which no pack can collide with.

## Stamp

Commit: `c2f335c331fe`
- `LSpeech.cs` `829653767021fd60`
- `LSpeechDraft.cs` `5f6927ecbc78a962`
- `LSpeechOffer.cs` `b9e2434a27f873f0`
- `LSpeechPack.cs` `1b09958a138f4949`
- `LSpeechRetirement.cs` `f7f395a97e64a7e0`
- `LSpeechRow.cs` `7b83143f53c29afc`
- `LSpeechValue.cs` `952a748c7cb55732`
