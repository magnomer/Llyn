# Llyn UI Design — Overall

## Scope

This document defines the overall UI structure and internal naming of Llyn.

It covers the principal regions of the program window and the relationship between navigation elements and the panels displayed in the main content area.

The internal structure of individual panels such as `PInput`, `PList`, `PSound`, `PTag`, `PSituation`, `PFavorite`, `PDuplex`, and `PSettings` is defined separately.

---

# 1. Naming rule

Principal UI component names use:

```text
P + one word
```

Examples:

```text
PWindow
PRoof
PBrand
PLogo
```

Navigation buttons may use the `PNavigation...` naming pattern to identify the panel they open.

---

# 2. PWindow

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

# 3. PRoof

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

# 4. PHouse

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

# 5. PNavigation

`PNavigation` contains the principal navigation buttons.

Each navigation element selects the corresponding content shown in `PPanel`.

Current mappings:

```text
PNavigationInput        -> PInput
PNavigationList         -> PList
PNavigationSound        -> PSound
PNavigationTag          -> PTag
PNavigationSituation    -> PSituation
PNavigationFavorite     -> PFavorite
PNavigationDuplex    -> PDuplex
PNavigationSettings     -> PSettings
```

---

# 6. PPanel

`PPanel` is the principal content area of `PHouse`.

It displays the panel selected through `PNavigation`.

The currently defined principal panels are:

```text
PInput
PList
PSound
PTag
PSituation
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

Its internal design is defined separately.

## PSound

`PSound` is opened through `PNavigationSound`.

Its internal design is defined separately.

## PTag

`PTag` is opened through `PNavigationTag`.

Its internal design is defined separately.

## PSituation

`PSituation` is opened through `PNavigationSituation`.

Its internal design is defined separately.

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

Its internal design is defined separately.

---

# 7. PEstablishment

`PEstablishment` is the area below `PHouse`.

It displays the status of the program.

Its internal elements are defined separately.

---

# 8. Current hierarchy

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
│       ├── PFavorite
│       ├── PDuplex
│       └── PSettings
│
└── PEstablishment
```

---

# 9. Spatial model

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

# 10. Summary

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
PFavorite
PDuplex
PSettings
```

This document defines only the overall UI structure. The internal design of each principal panel is specified independently.

The current specification for `PInput` is stored in `DesignUI-Input.md`.
