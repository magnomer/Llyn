# Llyn Database Proposal

## Scope

This document proposes the database structure for rebuilding Llyn from scratch.

The proposal focuses only on data construction, relationships, ownership, identity, and integrity. It does not describe program architecture, migration, compatibility, repository paths, or implementation history.

UI component names such as `PIndex`, `PInventory`, `PDirectory`, `PDisplay`, and `PEditor` do not alter database identity or ownership.

---

# 1. Core principles

1. Independent data has its own stable identity.
2. Shared data is referenced rather than duplicated.
3. Subordinate data is owned only when it has no meaningful existence outside its parent.
4. Removing a reference does not remove the independent object being referenced.
5. `Unspecified`, `Unknown`, and a specified value are distinct data states.
6. Display names governed by language settings are not copied into lexical records.
7. Ordering that belongs to a relationship is stored on the relationship, not on the referenced object.
8. Visible lexical content is never used as the identity of an independent object.
9. Entry, Example, and Situation receive program-generated random identifiers when they are created.

---

# 2. Main independent entities

The principal independent entities are:

```text
Entry
Example
Situation
Source
Author
```

Each has its own stable ID.

Their IDs do not depend on another entity's ID or visible lexical content.

## Identifier generation

When a new Entry, Example, or Situation is created, the program assigns it a random identifier.

The identifier may contain letters and digits, for example:

```text
176f86dr
```

The identifier is opaque and stable. It is not derived from the object's visible content and does not change when that content is edited.

In particular:

```text
Entry.id      != headword
Example.id    != example text
Tag           == its own text
Situation.id  != situation title
```

Objects with identical visible text may remain distinct records when they have different identifiers.

---

# 3. Entry

## `entry`

| Field | Meaning |
|---|---|
| `id` | Program-generated random stable Entry ID |
| `headword` | Main headword; not an identifier |
| `language` | Language identifier |
| `proficiency` | Optional proficiency information |
| `frequency` | Optional frequency information |
| `added_utc` | Optional creation timestamp |
| `updated_utc` | Optional modification timestamp |

An Entry owns lexical structures that exist specifically as parts of that Entry.

An Entry may also reference independent Examples and Sources.

---

# 4. Written forms

## `form`

| Field | Meaning |
|---|---|
| `entry_id` | Parent Entry |
| `position` | Order within the Entry |
| `text` | Written form |
| `local` | Optional local representation |
| `role` | Stable role identifier |

Primary identity:

```text
(entry_id, position)
```

Forms are subordinate to Entry.

---

# 5. Part of speech

## `part_of_speech`

| Field | Meaning |
|---|---|
| `entry_id` | Parent Entry |
| `position` | Order |
| `value_id` | Stable language-controlled POS identifier, or empty when the assignment is custom |
| `custom_name` | Part of speech exactly as typed, when no preset in the language names it |

Primary identity:

```text
(entry_id, position)
```

A row says its part of speech exactly one of two ways, and a constraint enforces it. When the
language declares a preset for what was chosen, the Entry stores that preset's stable identifier and
never its display name. When it does not, the typed text is stored as it stands: the part-of-speech
field is editable, so a part of speech a language pack has not thought of is still the one the user
meant. A custom row resolves against nothing and is displayed as it was written.

---

## `part_of_speech_value`

| Field | Meaning |
|---|---|
| `language` | Language |
| `value_id` | Stable POS identifier |
| `display_name` | Display name governed by the language settings |
| `position` | Display order |

Example:

```text
language = English
value_id = noun
display_name = Noun
```

The rows of one language are that language's presets: what the part-of-speech field offers in its
dropdown, in the order the language pack lists them. They come from
`languages/<Lang>/vocabulary.json` and are rewritten from it whenever a workspace is opened, so a
correction to a pack's wording reaches every Entry that named the preset, and none that typed past
it.

---

# 6. Inflection

## `inflection`

| Field | Meaning |
|---|---|
| `entry_id` | Parent Entry |
| `position` | Order |
| `text` | Inflected form |
| `local` | Optional local representation |
| `part_of_speech_id` | Optional stable POS identifier |

Primary identity:

```text
(entry_id, position)
```

---

## `inflection_feature`

| Field | Meaning |
|---|---|
| `entry_id` | Parent Entry |
| `inflection_position` | Parent Inflection |
| `position` | Feature order |
| `feature_id` | Stable grammatical-feature identifier |
| `value_id` | Stable grammatical-value identifier |

Primary identity:

```text
(entry_id, inflection_position, position)
```

Lexical data does not store copied display names for features or values.

---

## `morphology_value`

| Field | Meaning |
|---|---|
| `language` | Language |
| `part_of_speech_id` | POS identifier |
| `feature_id` | Stable feature identifier |
| `feature_display_name` | Language-controlled display name |
| `value_id` | Stable value identifier |
| `value_display_name` | Language-controlled display name |
| `position` | Display order |

---

# 7. Meaning

## `sense`

| Field | Meaning |
|---|---|
| `id` | Stable Meaning ID |
| `entry_id` | Parent Entry |
| `parent_id` | Parent Meaning, if subordinate |
| `position` | Order among siblings; updated when senses are reordered |
| `gloss` | Optional short gloss |
| `definition_language` | Optional language of the definition |
| `definition` | Single definition field for this Meaning |
| `labels` | Meaning labels |

A Meaning is subordinate to an Entry.

A subordinate Meaning is subordinate to another Meaning within the same Entry.

`parent_id` must refer to an existing Meaning in the same Entry.

Sibling positions must be unique within the same parent.

Each Meaning has one definition field. The UI counterpart is `PSenseDefinition`.

---

# 8. Meaning relations

## `relation`

| Field | Meaning |
|---|---|
| `id` | Relation ID |
| `sense_id` | Originating Meaning |
| `position` | Order |
| `relation_type` | Stable relation identifier |
| `label` | Optional descriptive label |
| `labels` | Optional additional labels |

A relation may point to an Entry or a Meaning.

Targets must be actual references rather than unchecked free-text IDs.

---

## `relation_entry`

| Field | Meaning |
|---|---|
| `relation_id` | Relation |
| `entry_id` | Target Entry |

---

## `relation_sense`

| Field | Meaning |
|---|---|
| `relation_id` | Relation |
| `sense_id` | Target Meaning |

A Relation has at most one target.

---

# 9. Pronunciation

## `pronunciation`

| Field | Meaning |
|---|---|
| `id` | Stable Pronunciation ID |
| `entry_id` | Parent Entry; unique |
| `level` | Pronunciation level/category |
| `ipa` | IPA representation |

Pronunciation is subordinate to Entry.

Each Entry may have at most one Pronunciation. There is therefore no pronunciation order and no primary/default-pronunciation distinction.

The UI counterpart is `PPronunciation`.

---

## `syllable`

| Field | Meaning |
|---|---|
| `pronunciation_id` | Parent Pronunciation |
| `position` | Order |
| `orthography` | Optional orthographic form |
| `local` | Optional local representation |
| `onset` | Optional onset |
| `medial` | Optional medial |
| `nucleus` | Nucleus |
| `coda` | Optional coda |
| `tone_number` | Optional tone number |
| `tone_local` | Optional local tone representation |
| `tone_points` | Optional tone-point data |

Primary identity:

```text
(pronunciation_id, position)
```

---

## `representation`

| Field | Meaning |
|---|---|
| `pronunciation_id` | Parent Pronunciation |
| `position` | Order |
| `system` | Representation system |
| `role` | Stable role |
| `text` | Representation |
| `local_tone` | Optional local tone representation |

Primary identity:

```text
(pronunciation_id, position)
```

---

# 10. Collocation

## `collocation`

| Field | Meaning |
|---|---|
| `id` | Stable Collocation ID |
| `entry_id` | Parent Entry |
| `position` | Order |
| `expression` | Collocation expression |

A Collocation is subordinate to an Entry.

Its editable construction mirrors the Sense construction, except that a Collocation has an expression rather than a Sense definition.

At the UI level:

```text
PSenseDefinition  ->  PCollocationExpression
```

Collocations may reference independent Examples and Situations, which they do not own, and carry Tags of their own.

The Collocation synonym field is an interlink rather than Entry-owned text. Its precise relation-storage form remains to be finalized together with the detailed targeting rules for collocation synonyms.

Reordering collocation cards changes `collocation.position` without changing the identity of the Collocation.

---

# 11. Note

## `note`

| Field | Meaning |
|---|---|
| `entry_id` | Parent Entry; primary identity |
| `text` | Note content |

Each Entry has at most one Note.

The Note is subordinate to the Entry.

The UI counterpart is `PNoteContents`, which presents the Note as a WYSIWYG Markdown editing surface. This proposal does not prescribe the editor's internal serialization format beyond preserving the Note content.

---

# 12. Example

## `example`

| Field | Meaning |
|---|---|
| `id` | Program-generated random stable Example ID |
| `language` | Example language |
| `text` | Example text; not an identifier |
| `local` | Optional local representation |
| `source_id` | Optional reference to one independent Source |

An Example is independent.

It has no owning Entry, Meaning, or Collocation.

An Example may reference at most one Source. The Source remains independent and is not owned by the Example.

In the UI, `PExample` is the overarching Example concept and `PExampleSource` is its single Source field.

---

## `example_translation`

| Field | Meaning |
|---|---|
| `id` | Translation ID |
| `example_id` | Parent Example |
| `language` | Translation language |
| `text` | Translation |
| `position` | Order |

Translations are subordinate to the Example.

An Example may have multiple translations.

---

# 13. Example and Collocation associations

## Example associations

### `entry_example`

| Field | Meaning |
|---|---|
| `entry_id` | Entry |
| `example_id` | Example |
| `position` | Order within this Entry |

Primary identity:

```text
(entry_id, example_id)
```

The position is unique within the Entry.

---

### `sense_example`

| Field | Meaning |
|---|---|
| `sense_id` | Meaning |
| `example_id` | Example |
| `position` | Order within this Meaning |

Primary identity:

```text
(sense_id, example_id)
```

The position is unique within the Meaning.

---

### `collocation_example`

| Field | Meaning |
|---|---|
| `collocation_id` | Collocation |
| `example_id` | Example |
| `position` | Order within this Collocation |

Primary identity:

```text
(collocation_id, example_id)
```

The position is unique within the Collocation.

---

## Collocation associations

### `collocation_tag`

| Field | Meaning |
|---|---|
| `collocation_id` | Collocation |
| `text` | Tag text, which is the Tag |
| `position` | Order within this Collocation |

Primary identity:

```text
(collocation_id, tag_id)
```

The position is unique within the Collocation.

Removing the row removes the Tag from that card; nothing else holds it.

---

### `collocation_situation`

| Field | Meaning |
|---|---|
| `collocation_id` | Collocation |
| `situation_id` | Referenced Situation |
| `position` | Order within this Collocation |

Primary identity:

```text
(collocation_id, situation_id)
```

The position is unique within the Collocation.

Removing the association does not delete the Situation.

---

## Example sharing

A single Example may be referenced by any number of Entries, Meanings, and Collocations.

```text
Entry A --------------------\
Meaning A.1 -----------------\
Meaning B.2 ------------------> Example X
Collocation A.3 -------------/
Collocation C.1 ------------/
```

Removing one association does not delete the Example.

---

# 14. Author

## `author`

| Field | Meaning |
|---|---|
| `id` | Stable Author ID |
| `name` | Author name |

Author is independent.

The same Author may be referenced by many Sources.

More than one Author may be associated with the same Source.

`Anonymous` is a valid specified Author entity. It is not a substitute for an unspecified author value.

---

# 15. Source

## `source`

Source is an independent entity.

There is no Source-type or Source-kind discriminator.

| Field | Meaning |
|---|---|
| `id` | Stable Source ID |
| `title_state` | `unspecified`, `unknown`, or `specified` |
| `title` | Title when specified |
| `program_name_state` | `unspecified`, `unknown`, or `specified` |
| `program_name` | Program name when applicable and specified |
| `channel_name_state` | `unspecified`, `unknown`, or `specified` |
| `channel_name` | Channel name when applicable and specified |
| `year_state` | `unspecified`, `unknown`, or `specified` |
| `year` | Publication/release year when specified |
| `url_state` | `unspecified`, `unknown`, or `specified` |
| `url` | URL when specified |
| `author_state` | `unspecified`, `unknown`, or `specified` |

The same Source structure is used regardless of what real-world material the Source represents.

The fields that are meaningful for a particular Source are populated; unrelated fields remain Unspecified.

Examples:

### Journal or newspaper article

```text
title
authors
year
url
```

### Television material

```text
title            = episode name
program_name
year
```

If there is no distinct episode title, the title may intentionally equal the program name.

### YouTube material

```text
title            = YouTube title
channel_name
year
url              = optional
```

### Picture

```text
title
authors
year
url              = optional
```

No category value is stored merely to distinguish these cases.

---

# 16. Source value states

For Source metadata, these states are distinct:

```text
Unspecified
Unknown
Specified(value)
```

## Unspecified

No value has been set.

## Unknown

The value has intentionally been recorded as unknown.

## Specified

A concrete value has been recorded.

For example:

```text
year_state = unspecified
year = NULL
```

means no year value has been set.

```text
year_state = unknown
year = NULL
```

means the year is intentionally recorded as unknown.

```text
year_state = specified
year = 1927
```

means the year is known.

The same distinction applies to other state-bearing Source fields.

`Untitled` is a specified title value, not an Unspecified state.

`Anonymous` is a specified Author reference, not an Unspecified author state.

---

# 17. Source and Author associations

## `source_author`

| Field | Meaning |
|---|---|
| `source_id` | Source |
| `author_id` | Author |
| `position` | Author order |

Primary identity:

```text
(source_id, author_id)
```

The position is unique within a Source.

A Source can have multiple Authors.

An Author can be referenced by multiple Sources.

---

# 18. Source references

Source information remains independent of the lexical objects that reference it.

## `entry_source`

| Field | Meaning |
|---|---|
| `entry_id` | Entry |
| `source_id` | Source |
| `position` | Order within the Entry |

Primary identity:

```text
(entry_id, source_id)
```

An Entry may reference multiple Sources through `entry_source`.

## Example Source reference

An Example may reference at most one Source through `example.source_id`.

This is a reference only. The Source remains independent and is not owned by the Example.

---

# 19. Tags

A Tag is its own text. It has no identifier, and no table of its own: the text *is* the Tag, so two cards writing the same text carry the same Tag, and a Tag nothing writes does not exist.

Tag text may hold spaces and ordinary punctuation. It is stored exactly as written, with leading and trailing spaces removed. An empty Tag is not stored.

---

## `sense_tag`

| Field | Meaning |
|---|---|
| `sense_id` | Meaning |
| `text` | Tag text, which is the Tag |
| `position` | Order within this Meaning |

Primary identity:

```text
(sense_id, text)
```

A Meaning therefore carries a given Tag at most once. The position is unique within the Meaning.

Deleting the Meaning deletes its Tag rows. Nothing else is deleted, because nothing else holds the Tag.

---

## `collocation_tag`

The same shape for a Collocation:

```text
(collocation_id, text)
```

---

## The Tag list

The set of Tags in a workspace is read from `sense_tag` and `collocation_tag` together. It is not stored separately, so it can never disagree with the cards.

Renaming a Tag rewrites the text on every card that carries it. Where a card already carries the new text, the two fold into one. Deleting a Tag removes it from every card and leaves the cards themselves untouched.

---

# 20. Situations

## `situation`

| Field | Meaning |
|---|---|
| `id` | Program-generated random stable Situation ID |
| `title` | Situation title; not an identifier |
| `description` | Description |
| `kind` | Situation/context classification |

Situation is independent data.

A Situation is not owned by an Entry, Meaning, or Collocation. The same Situation may be referenced by multiple Meanings and Collocations.

Changing its visible data does not change the Situation ID.

---

## `sense_situation`

| Field | Meaning |
|---|---|
| `sense_id` | Meaning |
| `situation_id` | Referenced Situation |
| `position` | Order within this Meaning |

Primary identity:

```text
(sense_id, situation_id)
```

The position is unique within the Meaning.

Removing a Meaning-Situation association does not delete the Situation.

---

# 21. Workspace data

## `workspace`

| Field | Meaning |
|---|---|
| `id` | Workspace row |
| `left_entry` | Selected Entry identifier |
| `right_entry` | Selected Entry identifier |
| `mode` | Workspace mode |
| `split` | Split state |
| `revision` | Current revision identifier |

Workspace state is operational data and does not define lexical ownership.

---

# 22. Revision data

## `revision`

| Field | Meaning |
|---|---|
| `id` | Revision ID |
| `created_utc` | Creation timestamp |

---

## `revision_change`

| Field | Meaning |
|---|---|
| `revision_id` | Revision |
| `position` | Change order |
| `target_id` | Target entity |
| `target_type` | Target entity type |
| `kind` | Change kind |
| `summary` | Change summary |

Primary identity:

```text
(revision_id, position)
```

---

## `tombstone`

| Field | Meaning |
|---|---|
| `entry_id` | Deleted Entry |
| `revision_id` | Deletion revision |
| `deleted_utc` | Deletion timestamp |

---

# 23. Ownership map

```text
Entry
├── Form
├── Part of Speech association
├── Inflection
│   └── Inflection Feature
├── Meaning
│   ├── Relation
│   ├── Example references ------> Example
│   ├── Tag                  ----> its own text
│   └── Situation references ----> Situation
├── Pronunciation
│   ├── Syllable
│   └── Representation
├── Collocation
├── Note
├── Example references ----------> Example
└── Source references -----------> Source

Collocation
├── Example references ----------> Example
├── Tag                  --------> its own text
├── Situation references -------> Situation
└── Synonym interlink ----------> lexical target

Example                           [independent]
├── Translation
└── Source reference ------------> Source

Situation                         [independent]

Source                            [independent]
└── Author references -----------> Author

Author                            [independent]
```

A synonym associated with a Meaning is represented as a lexical relation rather than as data owned by the Entry. The existing Relation structure can point to an Entry or a Meaning.

---

# 24. Deletion rules

## Deleting an Entry

Deleting an Entry deletes its subordinate data:

- Forms
- Part-of-speech associations
- Inflections and their features
- Meanings and their definition fields
- Relations originating from those Meanings
- Pronunciation, Syllables, and Representations
- Collocations
- Note

It also removes the Entry's association rows, including associations made through its Meanings and Collocations.

It does not delete independent Examples, Situations, Sources, or Authors.

---

## Deleting a Meaning

Deleting a Meaning deletes that Meaning, its definition field, originating Relations, and subordinate Meanings according to the chosen deletion operation.

It also removes that Meaning's Tags and its associations to Examples and Situations.

It does not delete the independent Examples or Situations that were referenced by the Meaning.

---

## Deleting a Collocation

Deleting a Collocation removes the Collocation, its Tags, and its association rows, including its references to Examples and Situations and its synonym interlink data.

It does not delete the independent Examples or Situations that were referenced by the Collocation.

---

## Deleting an Example

An Example must not be deleted while lexical objects still reference it unless those references are explicitly removed.

Deleting an Example removes its subordinate translations and its reference to its Source, if one exists.

It does not delete the referenced Source.

---

## Deleting a Tag

Deleting a Tag removes its text from every Meaning and Collocation that carries it.

Deleting a Tag does not delete the Meanings or Collocations that carried it.

---

## Deleting a Situation

A Situation must not be deleted while Meanings or Collocations still reference it unless those references are explicitly removed.

Deleting a Situation does not delete the Meanings that previously referenced it.

---

## Deleting a Source

A Source must not be deleted while Entries or Examples still reference it unless those references are explicitly removed.

Deleting a Source removes its Source-Author associations.

It does not delete Authors.

---

## Deleting an Author

An Author must not be deleted while a Source still references it unless that reference is explicitly removed.

---

# 25. Relationship ordering

Ordering belongs to the association when the same independent object may appear in different positions for different referrers.

For example:

```text
Meaning A -> Example X, position 1
Meaning B -> Example X, position 4
Collocation C -> Example X, position 2
```

Example X itself has no global lexical position.

The same rule applies to ordered Entry-Source references and Author order.

Sense order is stored by `sense.position`. Reordering `PSense` cards in the UI changes this ordering value; it does not change any Sense ID.

Collocation order is stored by `collocation.position`. Reordering collocation cards in the UI changes this ordering value; it does not change the Collocation ID.

---

# 26. Summary

## Independent data

```text
Entry
Example
Situation
Source
Author
```

## Entry-owned data

```text
Form
Part-of-Speech association
Inflection
Meaning
Pronunciation
Collocation
Note
```

## Meaning-owned data

```text
single definition field
Relation
subordinate Meaning
```

## Pronunciation-owned data

```text
Syllable
Representation
```

## Example-owned data

```text
Translation
```

## Reference-based relationships

```text
Entry       -> Example
Meaning     -> Example
Collocation -> Example

Meaning     -> Situation

Collocation -> Situation
Collocation -> lexical synonym target

Entry       -> Source
Example     -> Source (at most one)

Source      -> Author
```

Additional cardinality rules:

```text
Entry   -> at most one Pronunciation
Sense   -> one definition field
Entry   -> at most one Note
Example -> at most one Source
```

The central rules are:

> Independent data is never embedded into or owned by an object merely because that object currently uses it. Shared data is represented once and connected through explicit references.

> Visible lexical content is not database identity, with one deliberate exception: a Tag is its own text. Entry, Example, and Situation receive program-generated random stable identifiers when they are created.
