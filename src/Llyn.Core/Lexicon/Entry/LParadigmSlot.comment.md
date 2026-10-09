# LParadigmSlot.cs
Hash: `b923028e4bedc67e`
Hash: `a7a400a5d798cc46`

## `public sealed record LParadigmSlot(LSpeechValue LParadigmSlotSpeech, LMorphology LParadigmSlotMorphology, LInflection? LParadigmSlotInflection, LState LParadigmSlotState, LParadigm LParadigmSlotParadigm, IReadOnlyList<LMorphology>? LParadigmSlotMorphologies = null)`

One form an entry is expected to have, paired with the inflection stored for it.
A paradigm names forms by pack code, and a slot is that form resolved to the workspace's rows.
The part of speech is the one the entry carries, never the parent the paradigm was declared on.
The inflection is the stored form that answers the slot, or null when none does.
The state says whether the slot is answered, following the state convention.
Unspecified means no inflection is stored for the slot.
Specified means one is.
Unknown marks a persisted lacuna, distinguishing an unanswered slot from a reached source that supplied no form.
The paradigm the form was declared on rides along, so a reader can judge whether the stored form is regular.
A slot of a multi-value cell carries every value, and its first value stands as the morphology.

**Parameters**

- `LParadigmSlotSpeech`: The entry's part of speech the slot belongs to.
- `LParadigmSlotMorphology`: The morphology value the slot asks for.
- `LParadigmSlotInflection`: The stored inflection answering the slot, or null when none does.
- `LParadigmSlotState`: Whether the slot is answered.
- `LParadigmSlotParadigm`: The paradigm that named the form, carrying its regular pattern if any.
- `LParadigmSlotMorphologies`: Every value of the cell in pack order, or the morphology alone when left out.

## `public IReadOnlyList<long> LParadigmSlotCodes`

The pack codes of the cell's values, ascending.
Sorting makes a cell the same set whatever order the pack listed it in.

## `public string LParadigmSlotKey`

The codes joined with `+`, the key a source reading names as its variety.
A one-value cell's key is the bare code, so a one-value pack's readings keep working.
The same key marks a missed multi-value cell in the lacuna store.

## `public string LParadigmSlotName`

The value names joined with spaces in the order the pack listed them.
That order reads as a phrase, such as subjunctive imperfect first singular.

## `public LParadigmStatus LParadigmSlotCheck(bool pending, bool enabled)`

Answers what stands in the slot for its form.
A written form wins, and an empty one counts as absent.
An unknown slot follows, then a pending fetch.
A form still missing after that is lost when the morphology setting is on and absent when it is off.
The paradigm list and the inflection view both ask this one rule, so their cells never disagree.

## `public bool LParadigmSlotMatch(LParadigmSlot other)`

Whether two slots belong in one row of the paradigm table.
They match when they share a part of speech and both carry inflections of the same text.
A slot with no stored inflection matches nothing, so it never merges with a neighbour.

## `public IReadOnlyList<LMorphology> LParadigmSlotMorphologies { get; init; }`

An omitted cell list becomes the single morphology, preserving compatibility with one-value slots.

## `public bool LParadigmSlotUncertain`

Only the unknown state suppresses a pending or lost verdict when no inflection answers the slot.
