# LParadigmSlot.cs

## `public sealed record LParadigmSlot(`

One form an entry is expected to have, paired with the inflection stored for it.
A paradigm names forms by pack code, and a slot is that form resolved to the workspace's rows.
The part of speech is the one the entry carries, never the parent the paradigm was declared on.
The inflection is the stored form that answers the slot, or null when none does.
The state says whether the slot is answered, following the state convention.
Unspecified means no inflection is stored for the slot.
Specified means one is.
Unknown is reserved for a slot the reader has marked as having no such form.
The paradigm the form was declared on rides along, so a reader can judge whether the stored form is regular.

**Parameters**

- `LParadigmSlotSpeech` — The entry's part of speech the slot belongs to.
- `LParadigmSlotMorphology` — The morphology value the slot asks for.
- `LParadigmSlotInflection` — The stored inflection answering the slot, or null when none does.
- `LParadigmSlotState` — Whether the slot is answered.
- `LParadigmSlotParadigm` — The paradigm that named the form, carrying its regular pattern if any.
