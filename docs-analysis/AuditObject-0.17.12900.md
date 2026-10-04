# Object audit 0.17.12900

- Generation: 19
- Enforced: True
- Types: 954, declared in several parts: 13
- Verdicts: (1) Hydra 1, (2) Kraken 7, (3) Spider 2, (4) Chameleon 10, (5) Octopus 7, (6) Centipede 24, (7) Serpent 2, (9) Colony 2, Hermit 899
- (1) Hydra: 1, ceiling 1
- (2) Kraken: 8, ceiling 8
- (3) Spider: 6, ceiling 6
- (4) Chameleon: 11, ceiling 11
- (5) Octopus: 14, ceiling 14
- (6) Centipede: 36, ceiling 36
- (7) Serpent: 7, ceiling 7
- (8) Hub: 3, ceiling 3
- Above ceiling: 0
- Stale ceilings: 0

A Hydra has Parts 5 and Lines 1000, and Fused 0.75 or Density 0.50. A Kraken is a Serpent or Centipede that is also an Octopus or Spider. A Spider has Outgoing 25 and Incoming 25. A Chameleon has Mutable 7. An Octopus has Outgoing 40. A Centipede has Members 40. A Serpent has Lines 500. Every value is reached at the limit or above, on the merged type. A type's verdict is its worst flag, else Colony for several parts and Hermit for one. A hub is a state slot reached from 5 or more parts. It is a finding on the slot, never a verdict, so the verdict table shows the hub count beside it.

## Verdicts

| Type | Verdict | Flags | Hubs | Parts | Lines | Members | Mutable | Outgoing | Incoming |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| `Llyn.UIDeportment.QCorpus` | (1) Hydra | (1) Hydra, (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 9 | 1078 | 162 | 3 | 54 | 1 |
| `Llyn.Application.LEntryClerk` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede | 0 | 1 | 345 | 42 | 0 | 40 | 11 |
| `Llyn.ShellEngine.LEngine` | (2) Kraken | (2) Kraken, (3) Spider, (5) Octopus, (6) Centipede | 0 | 1 | 269 | 51 | 5 | 41 | 32 |
| `Llyn.ShellEngine.LEntryOutlet` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede | 0 | 1 | 146 | 49 | 0 | 42 | 0 |
| `Llyn.ShellEngine.LEntryPort` | (2) Kraken | (2) Kraken, (3) Spider, (6) Centipede | 0 | 1 | 140 | 52 | 0 | 34 | 26 |
| `Llyn.ShellEngine.LTenure` | (2) Kraken | (2) Kraken, (3) Spider, (4) Chameleon, (5) Octopus, (6) Centipede, (7) Serpent | 0 | 4 | 929 | 92 | 9 | 56 | 35 |
| `Llyn.UIDeportment.QRepertoire` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 7 | 818 | 138 | 2 | 50 | 1 |
| `Llyn.UIDeportment.QWindow` | (2) Kraken | (2) Kraken, (3) Spider, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 7 | 856 | 110 | 3 | 48 | 30 |
| `Llyn.Conduct.CAtelier` | (3) Spider | (3) Spider | 0 | 1 | 190 | 33 | 0 | 25 | 50 |
| `Llyn.Conduct.CEditor` | (3) Spider | (3) Spider | 0 | 1 | 197 | 37 | 2 | 32 | 55 |
| `Llyn.Conduct.CDesk` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 436 | 62 | 7 | 16 | 56 |
| `Llyn.Conduct.CYunjing` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 402 | 63 | 7 | 34 | 2 |
| `Llyn.UIDeportment.PSentence` | (4) Chameleon | (4) Chameleon | 0 | 3 | 377 | 39 | 9 | 16 | 7 |
| `Llyn.UIDeportment.QCardDrag` | (4) Chameleon | (4) Chameleon | 0 | 1 | 189 | 17 | 7 | 5 | 2 |
| `Llyn.UIDeportment.QLectern` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 330 | 52 | 23 | 22 | 15 |
| `Llyn.UIDeportment.QLecternAccent` | (4) Chameleon | (4) Chameleon | 0 | 1 | 137 | 19 | 9 | 10 | 2 |
| `Llyn.UIDeportment.QLecternCard` | (4) Chameleon | (4) Chameleon | 0 | 1 | 228 | 36 | 11 | 23 | 3 |
| `Llyn.UIDeportment.QLecternSound` | (4) Chameleon | (4) Chameleon | 0 | 1 | 235 | 37 | 13 | 19 | 3 |
| `Llyn.UIDeportment.QReflexItem` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 383 | 48 | 18 | 9 | 3 |
| `Llyn.UIDeportment.QVideoItem` | (4) Chameleon | (4) Chameleon | 0 | 1 | 212 | 22 | 7 | 10 | 5 |
| `Llyn.Application.LDraftClerk` | (5) Octopus | (5) Octopus | 0 | 1 | 277 | 22 | 0 | 101 | 3 |
| `Llyn.Application.LMarkupClerk` | (5) Octopus | (5) Octopus | 0 | 1 | 358 | 22 | 0 | 53 | 3 |
| `Llyn.Application.LPortraitClerk` | (5) Octopus | (5) Octopus | 0 | 1 | 355 | 34 | 0 | 41 | 3 |
| `Llyn.Infrastructure.LEntryLoader` | (5) Octopus | (5) Octopus | 0 | 1 | 332 | 18 | 0 | 55 | 1 |
| `Llyn.Infrastructure.LRigFactory` | (5) Octopus | (5) Octopus | 0 | 1 | 96 | 3 | 0 | 60 | 0 |
| `Llyn.ShellEngine.LEngineStaff` | (5) Octopus | (5) Octopus | 0 | 1 | 105 | 1 | 0 | 47 | 22 |
| `Llyn.ShellEngine.LQuill` | (5) Octopus | (5) Octopus | 0 | 1 | 304 | 38 | 0 | 50 | 11 |
| `Llyn.Conduct.CCorpus` | (6) Centipede | (6) Centipede | 0 | 1 | 487 | 68 | 0 | 31 | 1 |
| `Llyn.Conduct.CDisplay` | (6) Centipede | (6) Centipede | 0 | 1 | 361 | 50 | 2 | 33 | 5 |
| `Llyn.Conduct.CDisplaySound` | (6) Centipede | (6) Centipede | 0 | 1 | 397 | 43 | 0 | 35 | 19 |
| `Llyn.Conduct.CGuild` | (6) Centipede | (6) Centipede | 0 | 1 | 400 | 58 | 1 | 21 | 3 |
| `Llyn.Conduct.CPanel` | (6) Centipede | (6) Centipede | 0 | 1 | 473 | 54 | 2 | 15 | 38 |
| `Llyn.Conduct.CRepertoire` | (6) Centipede | (6) Centipede | 0 | 1 | 477 | 64 | 0 | 30 | 1 |
| `Llyn.Conduct.CShelf` | (6) Centipede | (6) Centipede | 0 | 1 | 433 | 63 | 1 | 28 | 1 |
| `Llyn.Conduct.CXiesheng` | (6) Centipede | (6) Centipede | 0 | 1 | 309 | 49 | 4 | 26 | 2 |
| `Llyn.ShellEngine.LPhonologyOutlet` | (6) Centipede | (6) Centipede | 0 | 1 | 129 | 44 | 0 | 25 | 0 |
| `Llyn.ShellEngine.LPhonologyPort` | (6) Centipede | (6) Centipede | 0 | 1 | 109 | 47 | 0 | 22 | 17 |
| `Llyn.ShellEngine.LVista` | (6) Centipede | (6) Centipede | 0 | 1 | 341 | 42 | 5 | 19 | 45 |
| `Llyn.UIDeportment.PCard` | (6) Centipede | (6) Centipede, (7) Serpent | 0 | 9 | 779 | 102 | 6 | 30 | 24 |
| `Llyn.UIDeportment.PContour` | (6) Centipede | (6) Centipede | 0 | 1 | 361 | 56 | 0 | 1 | 3 |
| `Llyn.UIDeportment.QDisplay` | (6) Centipede | (6) Centipede | 0 | 1 | 236 | 53 | 1 | 19 | 11 |
| `Llyn.UIDeportment.QFavorite` | (6) Centipede | (6) Centipede | 0 | 2 | 358 | 64 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QGuild` | (6) Centipede | (6) Centipede | 0 | 1 | 338 | 70 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QImprint` | (6) Centipede | (6) Centipede | 0 | 1 | 302 | 46 | 1 | 17 | 1 |
| `Llyn.UIDeportment.QLibrary` | (6) Centipede | (6) Centipede | 0 | 1 | 332 | 68 | 4 | 25 | 1 |
| `Llyn.UIDeportment.QPhonology` | (6) Centipede | (6) Centipede | 0 | 1 | 345 | 70 | 2 | 26 | 1 |
| `Llyn.UIDeportment.QReference` | (6) Centipede | (6) Centipede | 0 | 1 | 380 | 77 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QTaxonomy` | (6) Centipede | (6) Centipede | 0 | 3 | 457 | 75 | 2 | 31 | 1 |
| `Llyn.UIDeportment.QTenor` | (6) Centipede | (6) Centipede | 0 | 3 | 461 | 75 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QXiesheng` | (6) Centipede | (6) Centipede | 0 | 1 | 337 | 68 | 2 | 27 | 1 |
| `Llyn.UIDeportment.QYunjing` | (6) Centipede | (6) Centipede | 0 | 1 | 397 | 81 | 2 | 27 | 1 |
| `Llyn.Infrastructure.LEntryArchive` | (7) Serpent | (7) Serpent | 0 | 2 | 640 | 33 | 0 | 9 | 2 |
| `Llyn.Infrastructure.LSituationArchive` | (7) Serpent | (7) Serpent | 0 | 3 | 568 | 28 | 0 | 15 | 2 |
| `Llyn.UIDeportment.PSwath` | (9) Colony | none | 0 | 2 | 430 | 34 | 5 | 3 | 1 |
| `Llyn.UIDeportment.QArticulation` | (9) Colony | none | 0 | 3 | 219 | 22 | 1 | 4 | 1 |

Every other type (899) is a Hermit: one part, no flag and no hub.

## Split types

| Type | Parts | Lines | Members | Mutable | Outgoing | Incoming | Shared | Crossings | Glued | Fused | Density | Verdict |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| `Llyn.UIDeportment.QCorpus` | 9 | 1078 | 162 | 3 | 54 | 1 | 1 | 163 | 1.00 | 1.00 | 1.01 | (1) Hydra |
| `Llyn.ShellEngine.LTenure` | 4 | 929 | 92 | 9 | 56 | 35 | 0 | 110 | 1.00 | 1.00 | 1.20 | (2) Kraken |
| `Llyn.UIDeportment.QWindow` | 7 | 856 | 110 | 3 | 48 | 30 | 1 | 112 | 1.00 | 1.00 | 1.02 | (2) Kraken |
| `Llyn.UIDeportment.QRepertoire` | 7 | 818 | 138 | 2 | 50 | 1 | 1 | 116 | 1.00 | 1.00 | 0.84 | (2) Kraken |
| `Llyn.UIDeportment.PCard` | 9 | 779 | 102 | 6 | 30 | 24 | 0 | 27 | 1.00 | 1.00 | 0.26 | (6) Centipede |
| `Llyn.Infrastructure.LEntryArchive` | 2 | 640 | 33 | 0 | 9 | 2 | 0 | 14 | 1.00 | 1.00 | 0.42 | (7) Serpent |
| `Llyn.Infrastructure.LSituationArchive` | 3 | 568 | 28 | 0 | 15 | 2 | 0 | 12 | 1.00 | 1.00 | 0.43 | (7) Serpent |
| `Llyn.UIDeportment.QTenor` | 3 | 461 | 75 | 2 | 30 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.QTaxonomy` | 3 | 457 | 75 | 2 | 31 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.PSwath` | 2 | 430 | 34 | 5 | 3 | 1 | 0 | 20 | 1.00 | 1.00 | 0.59 | (9) Colony |
| `Llyn.UIDeportment.PSentence` | 3 | 377 | 39 | 9 | 16 | 7 | 0 | 3 | 0.67 | 0.67 | 0.08 | (4) Chameleon |
| `Llyn.UIDeportment.QFavorite` | 2 | 358 | 64 | 2 | 26 | 1 | 0 | 29 | 1.00 | 1.00 | 0.45 | (6) Centipede |
| `Llyn.UIDeportment.QArticulation` | 3 | 219 | 22 | 1 | 4 | 1 | 0 | 12 | 1.00 | 1.00 | 0.55 | (9) Colony |

## (1) Hydra

- Llyn.UIDeportment.QCorpus: parts 9, lines 1078, fused 1.00, density 1.01

## (2) Kraken

- Llyn.UIDeportment.QCorpus: lines 1078, members 162, outgoing 54, incoming 1
- Llyn.ShellEngine.LTenure: lines 929, members 92, outgoing 56, incoming 35
- Llyn.UIDeportment.QWindow: lines 856, members 110, outgoing 48, incoming 30
- Llyn.UIDeportment.QRepertoire: lines 818, members 138, outgoing 50, incoming 1
- Llyn.Application.LEntryClerk: lines 345, members 42, outgoing 40, incoming 11
- Llyn.ShellEngine.LEngine: lines 269, members 51, outgoing 41, incoming 32
- Llyn.ShellEngine.LEntryOutlet: lines 146, members 49, outgoing 42, incoming 0
- Llyn.ShellEngine.LEntryPort: lines 140, members 52, outgoing 34, incoming 26

## (3) Spider

- Llyn.ShellEngine.LTenure: outgoing 56, incoming 35
- Llyn.UIDeportment.QWindow: outgoing 48, incoming 30
- Llyn.ShellEngine.LEngine: outgoing 41, incoming 32
- Llyn.Conduct.CEditor: outgoing 32, incoming 55
- Llyn.Conduct.CAtelier: outgoing 25, incoming 50
- Llyn.ShellEngine.LEntryPort: outgoing 34, incoming 26

## (4) Chameleon

- Llyn.ShellEngine.LTenure: mutable 9
- Llyn.Conduct.CDesk: mutable 7
- Llyn.Conduct.CYunjing: mutable 7
- Llyn.UIDeportment.QReflexItem: mutable 18
- Llyn.UIDeportment.PSentence: mutable 9
- Llyn.UIDeportment.QLectern: mutable 23
- Llyn.UIDeportment.QLecternSound: mutable 13
- Llyn.UIDeportment.QLecternCard: mutable 11
- Llyn.UIDeportment.QVideoItem: mutable 7
- Llyn.UIDeportment.QCardDrag: mutable 7
- Llyn.UIDeportment.QLecternAccent: mutable 9

## (5) Octopus

- Llyn.UIDeportment.QCorpus: outgoing 54
- Llyn.ShellEngine.LTenure: outgoing 56
- Llyn.UIDeportment.QWindow: outgoing 48
- Llyn.UIDeportment.QRepertoire: outgoing 50
- Llyn.Application.LMarkupClerk: outgoing 53
- Llyn.Application.LPortraitClerk: outgoing 41
- Llyn.Application.LEntryClerk: outgoing 40
- Llyn.Infrastructure.LEntryLoader: outgoing 55
- Llyn.ShellEngine.LQuill: outgoing 50
- Llyn.Application.LDraftClerk: outgoing 101
- Llyn.ShellEngine.LEngine: outgoing 41
- Llyn.ShellEngine.LEntryOutlet: outgoing 42
- Llyn.ShellEngine.LEngineStaff: outgoing 47
- Llyn.Infrastructure.LRigFactory: outgoing 60

## (6) Centipede

- Llyn.UIDeportment.QCorpus: members 162
- Llyn.ShellEngine.LTenure: members 92
- Llyn.UIDeportment.QWindow: members 110
- Llyn.UIDeportment.QRepertoire: members 138
- Llyn.UIDeportment.PCard: members 102
- Llyn.Conduct.CCorpus: members 68
- Llyn.Conduct.CRepertoire: members 64
- Llyn.Conduct.CPanel: members 54
- Llyn.UIDeportment.QTenor: members 75
- Llyn.UIDeportment.QTaxonomy: members 75
- Llyn.Conduct.CDesk: members 62
- Llyn.Conduct.CShelf: members 63
- Llyn.Conduct.CYunjing: members 63
- Llyn.Conduct.CGuild: members 58
- Llyn.Conduct.CDisplaySound: members 43
- Llyn.UIDeportment.QYunjing: members 81
- Llyn.UIDeportment.QReflexItem: members 48
- Llyn.UIDeportment.QReference: members 77
- Llyn.Conduct.CDisplay: members 50
- Llyn.UIDeportment.PContour: members 56
- Llyn.UIDeportment.QFavorite: members 64
- Llyn.Application.LEntryClerk: members 42
- Llyn.UIDeportment.QPhonology: members 70
- Llyn.ShellEngine.LVista: members 42
- Llyn.UIDeportment.QGuild: members 70
- Llyn.UIDeportment.QXiesheng: members 68
- Llyn.UIDeportment.QLibrary: members 68
- Llyn.UIDeportment.QLectern: members 52
- Llyn.Conduct.CXiesheng: members 49
- Llyn.UIDeportment.QImprint: members 46
- Llyn.ShellEngine.LEngine: members 51
- Llyn.UIDeportment.QDisplay: members 53
- Llyn.ShellEngine.LEntryOutlet: members 49
- Llyn.ShellEngine.LEntryPort: members 52
- Llyn.ShellEngine.LPhonologyOutlet: members 44
- Llyn.ShellEngine.LPhonologyPort: members 47

## (7) Serpent

- Llyn.UIDeportment.QCorpus: lines 1078
- Llyn.ShellEngine.LTenure: lines 929
- Llyn.UIDeportment.QWindow: lines 856
- Llyn.UIDeportment.QRepertoire: lines 818
- Llyn.UIDeportment.PCard: lines 779
- Llyn.Infrastructure.LEntryArchive: lines 640
- Llyn.Infrastructure.LSituationArchive: lines 568

## (8) Hub

- Llyn.UIDeportment.QCorpus: `_cCorpus` reaches 9 parts
- Llyn.UIDeportment.QWindow: `_qWindowSurface` reaches 5 parts
- Llyn.UIDeportment.QRepertoire: `_cRepertoire` reaches 6 parts

## Above ceiling

## Stale ceilings
