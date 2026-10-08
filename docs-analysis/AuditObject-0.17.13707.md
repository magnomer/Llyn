# Object audit 0.17.13707

- Generation: 21
- Enforced: True
- Types: 1049, declared in several parts: 9
- Verdicts: (1) Hydra 1, (2) Kraken 4, (3) Spider 3, (4) Chameleon 8, (5) Octopus 4, (6) Centipede 21, (7) Serpent 1, (9) Colony 0, Hermit 1007
- (1) Hydra: 1, ceiling 1
- (2) Kraken: 5, ceiling 5
- (3) Spider: 5, ceiling 5
- (4) Chameleon: 9, ceiling 9
- (5) Octopus: 9, ceiling 9
- (6) Centipede: 29, ceiling 29
- (7) Serpent: 5, ceiling 5
- (8) Hub: 2, ceiling 2
- Above ceiling: 0
- Stale ceilings: 0

A Hydra has Parts 5 and Lines 1000, and Fused 0.75 or Density 0.50. A Kraken is a Serpent or Centipede that is also an Octopus or Spider. A Spider has Outgoing 25 and Incoming 25. A Chameleon has Mutable 7. An Octopus has Outgoing 40. A Centipede has Members 40. A Serpent has Lines 500. Every value is reached at the limit or above, on the merged type. A type's verdict is its worst flag, else Colony for several parts and Hermit for one. A hub is a state slot reached from 5 or more parts. It is a finding on the slot, never a verdict, so the verdict table shows the hub count beside it.

## Verdicts

| Type | Verdict | Flags | Hubs | Parts | Lines | Members | Mutable | Outgoing | Incoming |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| `Llyn.UIDeportment.QCorpus` | (1) Hydra | (1) Hydra, (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 9 | 1085 | 163 | 4 | 54 | 1 |
| `Llyn.Conduct.CDesk` | (2) Kraken | (2) Kraken, (3) Spider, (4) Chameleon, (6) Centipede | 0 | 1 | 480 | 73 | 8 | 26 | 55 |
| `Llyn.Conduct.CDisplaySound` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede | 0 | 1 | 434 | 50 | 0 | 44 | 20 |
| `Llyn.UIDeportment.QRepertoire` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 0 | 7 | 823 | 139 | 3 | 50 | 1 |
| `Llyn.UIDeportment.QWindow` | (2) Kraken | (2) Kraken, (3) Spider, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 7 | 856 | 110 | 3 | 49 | 31 |
| `Llyn.Conduct.CCatalog` | (3) Spider | (3) Spider | 0 | 1 | 265 | 20 | 0 | 28 | 41 |
| `Llyn.Conduct.CEditor` | (3) Spider | (3) Spider, (5) Octopus | 0 | 1 | 244 | 38 | 2 | 40 | 56 |
| `Llyn.ShellEngine.LEngine` | (3) Spider | (3) Spider | 0 | 1 | 176 | 39 | 3 | 37 | 42 |
| `Llyn.Conduct.CYunjing` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 435 | 65 | 7 | 39 | 2 |
| `Llyn.UIDeportment.PSentence` | (4) Chameleon | (4) Chameleon | 0 | 3 | 377 | 39 | 9 | 16 | 7 |
| `Llyn.UIDeportment.QLectern` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 348 | 54 | 25 | 22 | 15 |
| `Llyn.UIDeportment.QLecternAccent` | (4) Chameleon | (4) Chameleon | 0 | 1 | 134 | 19 | 9 | 9 | 2 |
| `Llyn.UIDeportment.QLecternCard` | (4) Chameleon | (4) Chameleon | 0 | 1 | 236 | 38 | 11 | 25 | 3 |
| `Llyn.UIDeportment.QLecternSound` | (4) Chameleon | (4) Chameleon | 0 | 1 | 235 | 37 | 13 | 19 | 3 |
| `Llyn.UIDeportment.QReflexItem` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 385 | 48 | 18 | 9 | 3 |
| `Llyn.UIDeportment.QVideoItem` | (4) Chameleon | (4) Chameleon | 0 | 1 | 212 | 22 | 7 | 10 | 5 |
| `Llyn.Conduct.CSounding` | (5) Octopus | (5) Octopus | 0 | 1 | 321 | 37 | 0 | 42 | 9 |
| `Llyn.Infrastructure.LRigFactory` | (5) Octopus | (5) Octopus | 0 | 1 | 102 | 3 | 0 | 65 | 0 |
| `Llyn.ShellEngine.LCardFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 295 | 28 | 0 | 41 | 8 |
| `Llyn.ShellEngine.LVistaFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 397 | 27 | 1 | 41 | 11 |
| `Llyn.Conduct.CAnthology` | (6) Centipede | (6) Centipede | 0 | 1 | 337 | 43 | 2 | 39 | 3 |
| `Llyn.Conduct.CCorpus` | (6) Centipede | (6) Centipede | 0 | 1 | 413 | 57 | 0 | 25 | 1 |
| `Llyn.Conduct.CDisplay` | (6) Centipede | (6) Centipede | 0 | 1 | 267 | 40 | 1 | 27 | 17 |
| `Llyn.Conduct.CGuild` | (6) Centipede | (6) Centipede | 0 | 1 | 409 | 58 | 1 | 23 | 3 |
| `Llyn.Conduct.CPanel` | (6) Centipede | (6) Centipede | 0 | 1 | 377 | 49 | 2 | 12 | 32 |
| `Llyn.Conduct.CRepertoire` | (6) Centipede | (6) Centipede | 0 | 1 | 419 | 55 | 0 | 24 | 1 |
| `Llyn.Conduct.CShelf` | (6) Centipede | (6) Centipede | 0 | 1 | 452 | 63 | 1 | 30 | 1 |
| `Llyn.Conduct.CXiesheng` | (6) Centipede | (6) Centipede | 0 | 1 | 331 | 50 | 4 | 30 | 2 |
| `Llyn.ShellEngine.LTenure` | (6) Centipede | (6) Centipede | 0 | 1 | 401 | 40 | 3 | 15 | 43 |
| `Llyn.UIDeportment.PCard` | (6) Centipede | (6) Centipede, (7) Serpent | 0 | 9 | 772 | 101 | 6 | 30 | 24 |
| `Llyn.UIDeportment.QDisplay` | (6) Centipede | (6) Centipede | 0 | 1 | 239 | 54 | 1 | 19 | 11 |
| `Llyn.UIDeportment.QFavorite` | (6) Centipede | (6) Centipede | 0 | 2 | 359 | 64 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QGuild` | (6) Centipede | (6) Centipede | 0 | 1 | 342 | 71 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QImprint` | (6) Centipede | (6) Centipede | 0 | 1 | 303 | 46 | 1 | 17 | 1 |
| `Llyn.UIDeportment.QLibrary` | (6) Centipede | (6) Centipede | 0 | 1 | 333 | 68 | 4 | 25 | 1 |
| `Llyn.UIDeportment.QPhonology` | (6) Centipede | (6) Centipede | 0 | 1 | 346 | 70 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QReference` | (6) Centipede | (6) Centipede | 0 | 1 | 380 | 77 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QTaxonomy` | (6) Centipede | (6) Centipede | 0 | 3 | 458 | 75 | 2 | 31 | 1 |
| `Llyn.UIDeportment.QTenor` | (6) Centipede | (6) Centipede | 0 | 3 | 462 | 75 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QXiesheng` | (6) Centipede | (6) Centipede | 0 | 1 | 337 | 68 | 2 | 27 | 1 |
| `Llyn.UIDeportment.QYunjing` | (6) Centipede | (6) Centipede | 0 | 1 | 397 | 81 | 2 | 27 | 1 |
| `Llyn.Infrastructure.LEntryArchive` | (7) Serpent | (7) Serpent | 0 | 2 | 643 | 33 | 0 | 10 | 2 |

Every other type (1007) is a Hermit: one part, no flag and no hub.

## Split types

| Type | Parts | Lines | Members | Mutable | Outgoing | Incoming | Shared | Crossings | Glued | Fused | Density | Verdict |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| `Llyn.UIDeportment.QCorpus` | 9 | 1085 | 163 | 4 | 54 | 1 | 1 | 163 | 1.00 | 1.00 | 1.00 | (1) Hydra |
| `Llyn.UIDeportment.QWindow` | 7 | 856 | 110 | 3 | 49 | 31 | 1 | 112 | 1.00 | 1.00 | 1.02 | (2) Kraken |
| `Llyn.UIDeportment.QRepertoire` | 7 | 823 | 139 | 3 | 50 | 1 | 0 | 116 | 1.00 | 1.00 | 0.83 | (2) Kraken |
| `Llyn.UIDeportment.PCard` | 9 | 772 | 101 | 6 | 30 | 24 | 0 | 27 | 1.00 | 1.00 | 0.27 | (6) Centipede |
| `Llyn.Infrastructure.LEntryArchive` | 2 | 643 | 33 | 0 | 10 | 2 | 0 | 14 | 1.00 | 1.00 | 0.42 | (7) Serpent |
| `Llyn.UIDeportment.QTenor` | 3 | 462 | 75 | 2 | 30 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.QTaxonomy` | 3 | 458 | 75 | 2 | 31 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.PSentence` | 3 | 377 | 39 | 9 | 16 | 7 | 0 | 3 | 0.67 | 0.67 | 0.08 | (4) Chameleon |
| `Llyn.UIDeportment.QFavorite` | 2 | 359 | 64 | 2 | 26 | 1 | 0 | 29 | 1.00 | 1.00 | 0.45 | (6) Centipede |

## (1) Hydra

- Llyn.UIDeportment.QCorpus: parts 9, lines 1085, fused 1.00, density 1.00

## (2) Kraken

- Llyn.UIDeportment.QCorpus: lines 1085, members 163, outgoing 54, incoming 1
- Llyn.UIDeportment.QWindow: lines 856, members 110, outgoing 49, incoming 31
- Llyn.UIDeportment.QRepertoire: lines 823, members 139, outgoing 50, incoming 1
- Llyn.Conduct.CDesk: lines 480, members 73, outgoing 26, incoming 55
- Llyn.Conduct.CDisplaySound: lines 434, members 50, outgoing 44, incoming 20

## (3) Spider

- Llyn.UIDeportment.QWindow: outgoing 49, incoming 31
- Llyn.Conduct.CDesk: outgoing 26, incoming 55
- Llyn.Conduct.CCatalog: outgoing 28, incoming 41
- Llyn.Conduct.CEditor: outgoing 40, incoming 56
- Llyn.ShellEngine.LEngine: outgoing 37, incoming 42

## (4) Chameleon

- Llyn.Conduct.CDesk: mutable 8
- Llyn.Conduct.CYunjing: mutable 7
- Llyn.UIDeportment.QReflexItem: mutable 18
- Llyn.UIDeportment.PSentence: mutable 9
- Llyn.UIDeportment.QLectern: mutable 25
- Llyn.UIDeportment.QLecternCard: mutable 11
- Llyn.UIDeportment.QLecternSound: mutable 13
- Llyn.UIDeportment.QVideoItem: mutable 7
- Llyn.UIDeportment.QLecternAccent: mutable 9

## (5) Octopus

- Llyn.UIDeportment.QCorpus: outgoing 54
- Llyn.UIDeportment.QWindow: outgoing 49
- Llyn.UIDeportment.QRepertoire: outgoing 50
- Llyn.Conduct.CDisplaySound: outgoing 44
- Llyn.ShellEngine.LVistaFacade: outgoing 41
- Llyn.Conduct.CSounding: outgoing 42
- Llyn.ShellEngine.LCardFacade: outgoing 41
- Llyn.Conduct.CEditor: outgoing 40
- Llyn.Infrastructure.LRigFactory: outgoing 65

## (6) Centipede

- Llyn.UIDeportment.QCorpus: members 163
- Llyn.UIDeportment.QWindow: members 110
- Llyn.UIDeportment.QRepertoire: members 139
- Llyn.UIDeportment.PCard: members 101
- Llyn.Conduct.CDesk: members 73
- Llyn.UIDeportment.QTenor: members 75
- Llyn.UIDeportment.QTaxonomy: members 75
- Llyn.Conduct.CShelf: members 63
- Llyn.Conduct.CYunjing: members 65
- Llyn.Conduct.CDisplaySound: members 50
- Llyn.Conduct.CRepertoire: members 55
- Llyn.Conduct.CCorpus: members 57
- Llyn.Conduct.CGuild: members 58
- Llyn.ShellEngine.LTenure: members 40
- Llyn.UIDeportment.QYunjing: members 81
- Llyn.UIDeportment.QReflexItem: members 48
- Llyn.UIDeportment.QReference: members 77
- Llyn.Conduct.CPanel: members 49
- Llyn.UIDeportment.QFavorite: members 64
- Llyn.UIDeportment.QLectern: members 54
- Llyn.UIDeportment.QPhonology: members 70
- Llyn.UIDeportment.QGuild: members 71
- Llyn.Conduct.CAnthology: members 43
- Llyn.UIDeportment.QXiesheng: members 68
- Llyn.UIDeportment.QLibrary: members 68
- Llyn.Conduct.CXiesheng: members 50
- Llyn.UIDeportment.QImprint: members 46
- Llyn.Conduct.CDisplay: members 40
- Llyn.UIDeportment.QDisplay: members 54

## (7) Serpent

- Llyn.UIDeportment.QCorpus: lines 1085
- Llyn.UIDeportment.QWindow: lines 856
- Llyn.UIDeportment.QRepertoire: lines 823
- Llyn.UIDeportment.PCard: lines 772
- Llyn.Infrastructure.LEntryArchive: lines 643

## (8) Hub

- Llyn.UIDeportment.QCorpus: `_cCorpus` reaches 8 parts
- Llyn.UIDeportment.QWindow: `_qWindowSurface` reaches 5 parts

## Above ceiling

## Stale ceilings
