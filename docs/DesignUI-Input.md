# Llyn UI Design — Input

## Scope

This document defines the UI structure and behavior of `PInput`.

`PInput` is opened by `PNavigationInput` and is used to create a new lexical Entry.

UI fields may create or associate data without implying database ownership. Data identity and ownership follow `Database-Structure-Proposal.md`.

The shared `PEditor` used by `PList`, `PSound`, and `PTag` mirrors the lexical editing structure defined in this document.

---

# 1. Overall layout

The current conceptual layout is:

```text
PHeadword  PLangcode                         PDiscard  PStore

PPronunciation  PPlay                        PLookup  PDownloader

PStackMeaning | PStackCollocation | PStackNote

PContents
```

Structurally:

```text
PInput
├── PHeadword
├── PLangcode
│   ├── PLangcodeBase
│   └── PLangcodeList
│       ├── PLangcodeListName
│       └── PLangcodeListFlag
├── PDiscard
├── PStore
├── PPronunciation
├── PPlay
├── PLookup
│   └── PLookupMenu
│       └── PLookupMenuItem
│           └── PLookupMenuSelector
├── PDownloader
├── PStack
│   ├── PStackMeaning
│   ├── PStackCollocation
│   └── PStackNote
└── PContents
    ├── PMeaning
    ├── PCollocation
    └── PNote
```

---

# 2. Entry header

## PHeadword

`PHeadword` is an editable field in which the user types the headword of the new Entry.

The headword is visible lexical data. It is not the Entry identifier. When an Entry is created, Llyn assigns it a separate random stable identifier.

## PLangcode

`PLangcode` is the language-selection dropdown.

### PLangcodeBase

`PLangcodeBase` is the unopened language selector.

### PLangcodeList

`PLangcodeList` is shown below `PLangcodeBase` when the selector is opened.

Each language option contains:

```text
PLangcodeListName  PLangcodeListFlag
```

`PLangcodeListName` displays the language name.

`PLangcodeListFlag` displays the associated flag.

## PDiscard

`PDiscard` discards the current input.

## PStore

`PStore` saves the current Entry and its associations.

Saving an Entry does not make independent Examples, Tags, or Situations subordinate to that Entry.

---

# 3. Pronunciation row

## PPronunciation

`PPronunciation` is the single pronunciation field for the Entry.

The user can type or edit the pronunciation directly.

Each Entry records only one pronunciation.

It always presents brackets:

```text
[ pronunciation ]
```

## PPlay

`PPlay` is shown when the local database has an audio file for the pronunciation.

It plays the locally stored pronunciation audio.

## PLookup

`PLookup` starts a pronunciation lookup and opens `PLookupMenu` below it.

## PLookupMenu

`PLookupMenu` is a small dropdown-like UI element shown below `PLookup`.

It first shows the lookup procedure while Llyn searches verified sources. It then shows one or more pronunciation candidates.

```text
PLookup
   │
   ▼
PLookupMenu
├── lookup procedure / status
├── PLookupMenuItem  PLookupMenuSelector
├── PLookupMenuItem  PLookupMenuSelector
└── ...
```

## PLookupMenuItem

`PLookupMenuItem` displays a pronunciation candidate returned by the lookup.

The menu may contain one candidate or several candidates.

## PLookupMenuSelector

`PLookupMenuSelector` appears next to a `PLookupMenuItem`.

The user uses it to choose which candidate should be added to `PPronunciation`.

## PDownloader

`PDownloader` downloads the audio file for the pronunciation.

---

# 4. PStack

`PStack` is the line on which the `PInput` tab titles sit.

It contains:

```text
PStack
├── PStackMeaning
├── PStackCollocation
└── PStackNote
```

The tabs map to `PContents` as follows:

```text
PStackMeaning       -> PMeaning
PStackCollocation   -> PCollocation
PStackNote          -> PNote
```

---

# 5. PContents

`PContents` is the common content area below `PStack`.

It shows the content belonging to the selected `PStack` tab.

```text
PMeaning
PCollocation
PNote
```

---

# 6. PMeaning

`PMeaning` is opened in `PContents` through `PStackMeaning`.

It can contain multiple `PSense` items.

```text
PMeaning
├── PSense
├── PSense
└── ...
```

Each `PSense` is shown as a separate card.

The cards are draggable. The user can drag them to reorder the senses. Reordering changes the sense order but does not change the identity of any Sense.

---

# 7. PSense

`PSense` is a card representing one Sense.

Each card contains:

```text
PSense
├── PSenseOrder
├── PSenseTitle
├── PSenseDefinition
├── PSenseExample
├── PSenseSituation
└── PSenseTag
```

The card also has an `X` button at its upper-right corner. Pressing the `X` removes that `PSense`.

Conceptually:

```text
┌─────────────────────────────────────────────┐
│ PSenseOrder   PSenseTitle               [X] │
│                                             │
│ PSenseDefinition                            │
│ PSenseExample                               │
│ PSenseSituation                             │
│ PSenseTag                                   │
└─────────────────────────────────────────────┘
          ↕ draggable for reordering
```

All named `PSense` elements are editable fields.

## PSenseTitle

`PSenseTitle` is the editable title of the card, shown in the card header beside `PSenseOrder`. A
Collocation card carries the same title field in the same place.

## PSenseOrder

`PSenseOrder` is the editable field for the order of the Sense.

Dragging a `PSense` card to another position updates the ordering represented by `PSenseOrder`.

## PSenseDefinition

`PSenseDefinition` is the single editable definition field for the Sense.

## PExample

`PExample` is the overarching UI concept for working with an Example.

It does not mean that an Example is owned by the Entry, Sense, or Collocation. Example data remains independent.

The overarching Example structure is:

```text
PExample
└── PExampleSource
```

### PExampleSource

`PExampleSource` is the single Source field for an Example.

An Example may reference one Source. The Source itself remains independent data.

---

## PSenseExample

`PSenseExample` is the Sense-specific editable field for an Example associated with the Sense.

Its Example behavior follows the overarching `PExample` structure, including `PExampleSource`.

When a new Example is created from this field:

1. it is stored as independent `Example` data;
2. Llyn assigns it its own random stable identifier;
3. the Sense stores an association to that Example;
4. the Example may reference one Source through `PExampleSource`.

```text
PSenseExample  ---->  Example [independent]
                         │
                         └── PExampleSource ----> Source [independent]
```

The same Example may be referenced by multiple lexical objects.

## PSenseSituation

`PSenseSituation` is an editable field used to associate the Sense with Situation data.

Situation is independent data rather than data owned by an Entry or Sense. A newly created Situation receives its own random stable identifier.

```text
PSenseSituation  ---->  Situation
                       [independent]
```

The same Situation may be associated with multiple Senses.

## PSenseSynonym

A synonym is not subordinate data owned by the Entry. It is represented as a lexical relation from the Sense to another lexical object through the database relation structure.

Because that relation points at a stored Entry or Sense rather than at typed text, no `PSenseSynonym` field is offered on the card: neither the Sense card nor the Collocation card carries one until a picker exists that can resolve what the user types to the lexical object the relation needs.

## PSenseTag

`PSenseTag` is an editable field used to associate the Sense with Tag data.

Tag is independent data rather than data owned by an Entry or Sense. A newly created Tag receives its own random stable identifier.

```text
PSenseTag  ---->  Tag
                  [independent]
```

The same Tag may be associated with multiple Senses.

## Removing a PSense

The `X` button at the upper-right of the card removes that `PSense`.

Removing a Sense removes the Sense and its association records. It does not delete independent Examples, Tags, or Situations merely because they were linked from that Sense.

---

---

# 8. PCollocation

`PCollocation` is opened in `PContents` through `PStackCollocation`.

Its construction mirrors the card-based construction used by `PMeaning` and `PSense`.

The same interaction rules apply:

- multiple collocation cards may be present;
- each card carries an editable title in its header, beside the order badge;
- each card has an `X` button at the upper-right for removal;
- cards are draggable for reordering;
- the corresponding order, example, situation, and tag fields are editable;
- Examples, Situations, and Tags are referenced independent data rather than data owned by the Entry.

Where a Sense card carries one definition, a Collocation card carries two fields:

```text
PSenseDefinition  ->  PCollocationExpression
                      PCollocationMeaning
```

`PCollocationExpression` is the editable collocation-expression field: the word combination itself.

`PCollocationMeaning` is the editable field explaining what that combination means. It is the
Collocation's definition, kept separate from the expression because the expression is the lexical
form and the meaning is what it says.

The mirrored Collocation Example field follows the same overarching `PExample` structure as `PSenseExample`, including one `PExampleSource`.

Conceptually:

```text
PMeaning / PSense                    PCollocation
─────────────────                    ────────────
card title                           card title
order                                corresponding order
PSenseDefinition        ->           PCollocationExpression
                                     PCollocationMeaning
PSenseExample                        corresponding example
PSenseSituation                      corresponding situation
PSenseTag                            corresponding tag

[X] remove card                      [X] remove card
drag to reorder                      drag to reorder
```

Names for the other collocation-specific mirrored fields have not yet been separately specified. This document therefore does not invent additional component names for them.

Removing a collocation card removes that Collocation and its association records. It does not delete independent Examples, Tags, or Situations merely because they were associated with that Collocation.

---

# 9. PNote

`PNote` is opened in `PContents` through `PStackNote`.

What is shown in `PNote` is:

```text
PNote
└── PNoteContents
```

## PNoteContents

`PNoteContents` is the single editable Note area for the Entry.

It must support Markdown in a WYSIWYG manner. The user edits the visually formatted result directly rather than being required to edit raw Markdown syntax as the primary interface.

For example, Markdown semantics such as headings, emphasis, and lists are shown and edited as formatted content.

This UI requirement does not by itself prescribe the database storage encoding for the note.

---

# 10. Data identity relevant to PInput

Visible text entered through `PInput` does not serve as the identity of independent data.

```text
PHeadword       -> Entry.id is separate from the headword
PSenseExample   -> Example.id is separate from the example text
PSenseTag       -> Tag.id is separate from the tag text
PSenseSituation -> Situation.id is separate from the situation title/text
```

`PExampleSource` references one independent Source. The Source is not part of the Example's identity and is not owned by the Example.

The same independence rule applies when a Collocation references an Example, Tag, or Situation.

When a new Entry, Example, Tag, or Situation is created, Llyn assigns a random stable identifier containing letters and/or digits, for example:

```text
176f86dr
```

Editing visible text does not change the identifier.

---

# 11. Current PInput hierarchy

```text
PInput
├── PHeadword
├── PLangcode
│   ├── PLangcodeBase
│   └── PLangcodeList
│       ├── PLangcodeListName
│       └── PLangcodeListFlag
├── PDiscard
├── PStore
│
├── PPronunciation
├── PPlay
├── PLookup
│   └── PLookupMenu
│       ├── lookup procedure / status
│       └── PLookupMenuItem
│           └── PLookupMenuSelector
├── PDownloader
│
├── PStack
│   ├── PStackMeaning ───────────> PMeaning
│   ├── PStackCollocation ───────> PCollocation
│   └── PStackNote ──────────────> PNote
│
└── PContents
    ├── PMeaning
    │   └── PSense [card; draggable]
    │       ├── [X] remove PSense
    │       ├── PSenseOrder
    │       ├── PSenseTitle
    │       ├── PSenseDefinition
    │       ├── PSenseExample ───> Example [independent]
    │       │                       └── PExampleSource ──> Source [independent]
    │       ├── PSenseSituation ─> Situation [independent]
    │       └── PSenseTag ───────> Tag [independent]
    │
    ├── PCollocation
    │   └── collocation cards [mirror PSense; draggable]
    │       ├── [X] remove collocation
    │       ├── corresponding order
    │       ├── corresponding title
    │       ├── PCollocationExpression
    │       ├── PCollocationMeaning
    │       ├── corresponding example ─────> Example [independent]
    │       │                                  └── PExampleSource ──> Source [independent]
    │       ├── corresponding situation ───> Situation [independent]
    │       └── corresponding tag ─────────> Tag [independent]
    │
    └── PNote
        └── PNoteContents [WYSIWYG Markdown]
```
