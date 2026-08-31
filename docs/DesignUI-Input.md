# Llyn UI Design — Input

## Scope

This document defines the UI structure and behavior of `PInput`.

`PInput` is opened by `PNavigationInput` and is used to create a new lexical Entry.

UI fields may create or associate data without implying database ownership. Data identity and ownership follow `Database-Structure-Proposal.md`.

---

# 1. Overall layout

The current conceptual layout is:

```text
PHeadword  PLangcode                         PDiscard  PStore

PPronunciation  PPlayback                        PLookup  PDownloader

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
├── PPlayback
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

`PPronunciation` is an editable field in which the user can type or edit the pronunciation.

It always presents brackets:

```text
[ pronunciation ]
```

## PPlayback

`PPlayback` is shown when the local database has an audio file for the pronunciation.

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
├── PSenseDefinition
├── PSenseExample
├── PSenseSituation
├── PSenseSynonym
└── PSenseTag
```

The card also has an `X` button at its upper-right corner. Pressing the `X` removes that `PSense`.

Conceptually:

```text
┌─────────────────────────────────────────────┐
│ PSenseOrder   PSenseDefinition          [X] │
│               PSenseExample                 │
│                                             │
│ PSenseSituation                             │
│ PSenseSynonym                               │
│ PSenseTag                                   │
└─────────────────────────────────────────────┘
          ↕ draggable for reordering
```

All six named `PSense` elements are editable fields.

## PSenseOrder

`PSenseOrder` is the editable field for the order of the Sense.

Dragging a `PSense` card to another position updates the ordering represented by `PSenseOrder`.

## PSenseDefinition

`PSenseDefinition` is the editable definition field.

## PSenseExample

`PSenseExample` is an editable field for an Example associated with the Sense.

The Example is not owned by the Entry or Sense. When a new Example is created from this field:

1. it is stored as independent `Example` data;
2. Llyn assigns it its own random stable identifier;
3. the Sense stores an association to that Example.

```text
PSenseExample  ---->  Example
                     [independent]
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

`PSenseSynonym` is an editable field used to create or edit a synonym interlink.

A synonym is not subordinate data owned by the Entry. It is represented as a lexical relation from the Sense to another lexical object through the database relation structure.

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
- each card has an `X` button at the upper-right for removal;
- cards are draggable for reordering;
- the corresponding order, example, situation, synonym, and tag fields are editable;
- Examples, Situations, and Tags are referenced independent data rather than data owned by the Entry.

The one explicitly different field is:

```text
PSenseDefinition  ->  PCollocationExpression
```

`PCollocationExpression` is the editable collocation-expression field.

Conceptually:

```text
PMeaning / PSense                    PCollocation
─────────────────                    ────────────
order                                corresponding order
PSenseDefinition        ->           PCollocationExpression
PSenseExample                        corresponding example
PSenseSituation                      corresponding situation
PSenseSynonym                        corresponding synonym
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

`PNoteContents` is the editable note area.

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
├── PPlayback
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
    │       ├── PSenseDefinition
    │       ├── PSenseExample ───> Example [independent]
    │       ├── PSenseSituation ─> Situation [independent]
    │       ├── PSenseSynonym ───> lexical relation
    │       └── PSenseTag ───────> Tag [independent]
    │
    ├── PCollocation
    │   └── collocation cards [mirror PSense; draggable]
    │       ├── [X] remove collocation
    │       ├── corresponding order
    │       ├── PCollocationExpression
    │       ├── corresponding example ─────> Example [independent]
    │       ├── corresponding situation ───> Situation [independent]
    │       ├── corresponding synonym ─────> lexical interlink
    │       └── corresponding tag ─────────> Tag [independent]
    │
    └── PNote
        └── PNoteContents [WYSIWYG Markdown]
```
