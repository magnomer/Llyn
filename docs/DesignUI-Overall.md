# Llyn UI Design — Overall

## Scope

This document defines the overall UI structure and internal naming of Llyn.

It covers the principal regions of the program window and the relationship between navigation elements and the panels displayed in the main content area.

The internal structure of individual panels such as `PInput`, `PList`, `PSound`, `PTag`, `PSituation`, `PExample`, `PReference`, `PFavorite`, `PDuplex`, and `PSettings` is defined separately.

---

# 1. Naming convention

Internal UI names begin with `P`.

The exact component names defined in these UI specifications are normative.

Principal regions use concise names such as:

```text
PWindow
PRoof
PBrand
PHouse
PPanel
```

Nested elements may extend the parent concept where needed, for example:

```text
PNavigationInput
PLangcodeListName
PLookupMenuSelector
PWorkbenchSelect
```

When an accepted concise name already exists, a longer generic replacement should not be invented. For example, the window-caption area is `PCaption`.


---

# 2. Shared functional categories and tab-specific names

Some tabs use different concrete component names for the same broader functional category.

The browse-style tabs currently follow this mapping:

| Functional category | PList | PSound | PTag | PExample | PReference |
|---|---|---|---|---|---|
| Sorting | `POrder` | `PSequence` | `PFunnel` | `PRank` | `PGrade` |
| Search | `PInquiry` | `PProbe` | `PExploration` | `PQuery` | `PSurvey` |
| `PCatalog` | `PIndex` | `PInventory` | `PDirectory` | `PAnthology` | `PShelf` |
| Read/display area | `PDisplay` | `PDisplay` | `PDisplay` | `PDisplay` | `PDisplay` |
| Mode toggle | `PScribe` | `PScribe` | `PScribe` | `PScribe` | `PScribe` |
| Editing area | `PEditor` | `PEditor` | `PEditor` | `PEditor` | `PEditor` |

`PCatalog` is an overarching functional term. It does not require the concrete component itself to be named `PCatalog`.

Thus:

```text
PCatalog
├── PIndex          [PList]
├── PInventory      [PSound]
├── PDirectory      [PTag]
├── PAnthology      [PExample]
└── PShelf          [PReference]
```

`PDisplay`, `PScribe`, and `PEditor` are intentionally shared concrete names across these tabs.

What they share is the read/edit mechanism, not the object. In `PList`, `PSound`, and `PTag` they stand on an Entry. In `PExample` they stand on an Example, and in `PReference` on a Source.

---

# 3. PWindow


`PWindow` is the internal name of the entire program window UI.

It consists of three principal areas arranged vertically:

```text
PWindow
├── PRoof
├── PHouse
└── PEstablishment
```

Spatially:

```text
PRoof
PHouse
PEstablishment
```

---

# 4. PRoof

`PRoof` is the uppermost area of `PWindow`.

It contains:

```text
PRoof
├── PBrand
└── PCaption
```

## PBrand

`PBrand` is the branding area of the program.

It contains:

```text
PBrand
├── PLogo
└── PHeadline
```

### PLogo

`PLogo` displays the program logo.

Clicking `PLogo` opens a dropdown menu called `PHeadquarter`.

By default:

```text
PHeadquarter
├── About
└── Exit
```

### PHeadline

`PHeadline` displays the name of the program.

## PCaption

`PCaption` is the area containing the standard window caption functions:

```text
Minimize
Maximize / Restore
Close
```

---

# 5. PHouse

`PHouse` is the principal working area below `PRoof`.

It contains two regions arranged horizontally:

```text
PNavigation | PPanel
```

Structurally:

```text
PHouse
├── PNavigation
└── PPanel
```

---

# 6. PNavigation

`PNavigation` contains the principal navigation buttons.

Each navigation element selects the corresponding content shown in `PPanel`.

Current mappings:

```text
PNavigationInput        -> PInput
PNavigationList         -> PList
PNavigationSound        -> PSound
PNavigationTag          -> PTag
PNavigationSituation    -> PSituation
PNavigationExample      -> PExample
PNavigationSource       -> PReference
PNavigationFavorite     -> PFavorite
PNavigationDuplex       -> PDuplex
PNavigationSettings     -> PSettings
```

---

# 7. PPanel

`PPanel` is the principal content area of `PHouse`.

It displays the panel selected through `PNavigation`.

The currently defined principal panels are:

```text
PInput
PList
PSound
PTag
PSituation
PExample
PReference
PFavorite
PDuplex
PSettings
```

## PInput

`PInput` is opened through `PNavigationInput`.

Its internal design is defined in `DesignUI-Input.md`.

## PList

`PList` is opened through `PNavigationList`.

It displays the entire list of recorded entries.

Its internal design is defined in `DesignUI-List.md`.

## PSound

`PSound` is opened through `PNavigationSound`.

Its internal design is defined in `DesignUI-Sound.md`.

## PTag

`PTag` is opened through `PNavigationTag`.

Its internal design is defined separately.

## PSituation

`PSituation` is opened through `PNavigationSituation`.

Its internal design is defined separately.

## PExample

`PExample` is opened through `PNavigationExample`.

It browses the Examples the workspace holds, which are independent data no Entry owns.

Its internal design is defined in `DesignUI-Example.md`.

## PReference

`PReference` is opened through `PNavigationSource`.

It browses the bibliographic Sources the workspace holds, which are independent data no Entry or Example owns.

The panel is shown to the user as **Source**. Its internal base is `Reference`, because the base `Source` already names a pronunciation source defined by a language pack.

Its internal design is defined in `DesignUI-Source.md`.

## PFavorite

`PFavorite` is opened through `PNavigationFavorite`.

It displays the list of favorite items.

Its internal design is defined separately.

## PDuplex

`PDuplex` is opened through `PNavigationDuplex`.

It offers a side-by-side comparison interface.

Its internal design is defined separately.

## PSettings

`PSettings` is opened through `PNavigationSettings`.

Its internal design is defined in `DesignUI-Settings.md`.

---

# 8. PEstablishment

`PEstablishment` is the area below `PHouse`.

It displays the status of the program.

Its internal elements are defined separately.

---

# 9. Current hierarchy

```text
PWindow
├── PRoof
│   ├── PBrand
│   │   ├── PLogo
│   │   │   └── PHeadquarter
│   │   │       ├── About
│   │   │       └── Exit
│   │   └── PHeadline
│   └── PCaption
│       ├── Minimize
│       ├── Maximize / Restore
│       └── Close
│
├── PHouse
│   ├── PNavigation
│   │   ├── PNavigationInput       -> PInput
│   │   ├── PNavigationList        -> PList
│   │   ├── PNavigationSound       -> PSound
│   │   ├── PNavigationTag         -> PTag
│   │   ├── PNavigationSituation   -> PSituation
│   │   ├── PNavigationExample     -> PExample
│   │   ├── PNavigationSource      -> PReference
│   │   ├── PNavigationFavorite    -> PFavorite
│   │   ├── PNavigationDuplex   -> PDuplex
│   │   └── PNavigationSettings    -> PSettings
│   │
│   └── PPanel
│       ├── PInput
│       ├── PList
│       ├── PSound
│       ├── PTag
│       ├── PSituation
│       ├── PExample
│       ├── PReference
│       ├── PFavorite
│       ├── PDuplex
│       └── PSettings
│
└── PEstablishment
```

---

# 10. Spatial model

```text
┌─────────────────────────────────────────────┐
│                    PRoof                    │
├──────────────┬──────────────────────────────┤
│              │                              │
│ PNavigation  │            PPanel            │
│              │                              │
│              │                              │
│              │                              │
├──────────────┴──────────────────────────────┤
│               PEstablishment                │
└─────────────────────────────────────────────┘
```

Within `PRoof`:

```text
PBrand | PCaption
```

Within `PBrand`:

```text
PLogo | PHeadline
```

Within `PHouse`:

```text
PNavigation | PPanel
```

---

# 11. Summary

The principal organization of Llyn's window UI is:

```text
PWindow
    PRoof
        PBrand
            PLogo
                PHeadquarter
            PHeadline
        PCaption

    PHouse
        PNavigation
        PPanel

    PEstablishment
```

`PNavigation` selects among:

```text
PInput
PList
PSound
PTag
PSituation
PExample
PReference
PFavorite
PDuplex
PSettings
```

This document defines only the overall UI structure. The internal design of each principal panel is specified independently.

Current dedicated panel specifications include:

```text
PInput     -> DesignUI-Input.md
PList      -> DesignUI-List.md
PSound     -> DesignUI-Sound.md
PTag       -> DesignUI-Tag.md
PExample   -> DesignUI-Example.md
PReference -> DesignUI-Source.md
PDuplex    -> DesignUI-Duplex.md
PSettings  -> DesignUI-Settings.md
```

The remaining principal panels will receive their own documents as their designs are defined.
