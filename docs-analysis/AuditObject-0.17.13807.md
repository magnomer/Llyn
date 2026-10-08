# Object audit 0.17.13807

- Generation: 21
- Enforced: True
- Types: 1066, declared in several parts: 9
- Verdicts: (1) Hydra 1, (2) Kraken 2, (3) Spider 2, (4) Chameleon 7, (5) Octopus 3, (6) Centipede 13, (7) Serpent 1, (9) Colony 0, Hermit 1037
- (1) Hydra: 1, ceiling 1
- (2) Kraken: 3, ceiling 3
- (3) Spider: 3, ceiling 3
- (4) Chameleon: 7, ceiling 7
- (5) Octopus: 6, ceiling 6
- (6) Centipede: 18, ceiling 18
- (7) Serpent: 5, ceiling 5
- (8) Hub: 2, ceiling 2
- Above ceiling: 0
- Stale ceilings: 0

A Hydra has Parts 5 and Lines 1000, and Fused 0.75 or Density 0.50. A Kraken is a Serpent or Centipede that is also an Octopus or Spider. A Spider has Outgoing 25 and Incoming 25. A Chameleon has Mutable 7. An Octopus has Outgoing 40. A Centipede has Members 40. A Serpent has Lines 500. Every value is reached at the limit or above, on the merged type. A type's verdict is its worst flag, else Colony for several parts and Hermit for one. A hub is a state slot reached from 5 or more parts. It is a finding on the slot, never a verdict, so the verdict table shows the hub count beside it.

## Verdicts

| Type | Verdict | Flags | Hubs | Parts | Lines | Members | Mutable | Outgoing | Incoming |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| `Llyn.UIDeportment.QCorpus` | (1) Hydra | (1) Hydra, (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 9 | 1093 | 163 | 4 | 56 | 1 |
| `Llyn.UIDeportment.QRepertoire` | (2) Kraken | (2) Kraken, (5) Octopus, (6) Centipede, (7) Serpent | 0 | 7 | 833 | 139 | 3 | 52 | 1 |
| `Llyn.UIDeportment.QWindow` | (2) Kraken | (2) Kraken, (3) Spider, (5) Octopus, (6) Centipede, (7) Serpent | 1 | 7 | 856 | 110 | 3 | 49 | 31 |
| `Llyn.Conduct.CEditor` | (3) Spider | (3) Spider | 0 | 1 | 171 | 29 | 2 | 33 | 56 |
| `Llyn.ShellEngine.LEngine` | (3) Spider | (3) Spider | 0 | 1 | 176 | 39 | 3 | 37 | 42 |
| `Llyn.UIDeportment.PSentence` | (4) Chameleon | (4) Chameleon | 0 | 3 | 377 | 39 | 9 | 16 | 7 |
| `Llyn.UIDeportment.QLectern` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 349 | 54 | 25 | 23 | 15 |
| `Llyn.UIDeportment.QLecternAccent` | (4) Chameleon | (4) Chameleon | 0 | 1 | 134 | 19 | 9 | 9 | 2 |
| `Llyn.UIDeportment.QLecternCard` | (4) Chameleon | (4) Chameleon | 0 | 1 | 236 | 38 | 11 | 25 | 3 |
| `Llyn.UIDeportment.QLecternSound` | (4) Chameleon | (4) Chameleon | 0 | 1 | 235 | 37 | 13 | 19 | 3 |
| `Llyn.UIDeportment.QReflexItem` | (4) Chameleon | (4) Chameleon, (6) Centipede | 0 | 1 | 385 | 48 | 18 | 9 | 3 |
| `Llyn.UIDeportment.QVideoItem` | (4) Chameleon | (4) Chameleon | 0 | 1 | 212 | 22 | 7 | 10 | 5 |
| `Llyn.Infrastructure.LRigFactory` | (5) Octopus | (5) Octopus | 0 | 1 | 102 | 3 | 0 | 65 | 0 |
| `Llyn.ShellEngine.LCardFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 295 | 28 | 0 | 41 | 8 |
| `Llyn.ShellEngine.LVistaFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 397 | 27 | 1 | 41 | 11 |
| `Llyn.ShellEngine.LTenure` | (6) Centipede | (6) Centipede | 0 | 1 | 401 | 40 | 3 | 15 | 46 |
| `Llyn.UIDeportment.PCard` | (6) Centipede | (6) Centipede, (7) Serpent | 0 | 9 | 772 | 101 | 6 | 30 | 24 |
| `Llyn.UIDeportment.QDisplay` | (6) Centipede | (6) Centipede | 0 | 1 | 239 | 54 | 1 | 19 | 11 |
| `Llyn.UIDeportment.QFavorite` | (6) Centipede | (6) Centipede | 0 | 2 | 359 | 64 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QGuild` | (6) Centipede | (6) Centipede | 0 | 1 | 344 | 71 | 2 | 28 | 1 |
| `Llyn.UIDeportment.QImprint` | (6) Centipede | (6) Centipede | 0 | 1 | 303 | 46 | 1 | 18 | 1 |
| `Llyn.UIDeportment.QLibrary` | (6) Centipede | (6) Centipede | 0 | 1 | 334 | 68 | 4 | 29 | 1 |
| `Llyn.UIDeportment.QPhonology` | (6) Centipede | (6) Centipede | 0 | 1 | 347 | 70 | 2 | 30 | 1 |
| `Llyn.UIDeportment.QReference` | (6) Centipede | (6) Centipede | 0 | 1 | 382 | 77 | 2 | 33 | 1 |
| `Llyn.UIDeportment.QTaxonomy` | (6) Centipede | (6) Centipede | 0 | 3 | 460 | 75 | 2 | 35 | 1 |
| `Llyn.UIDeportment.QTenor` | (6) Centipede | (6) Centipede | 0 | 3 | 463 | 75 | 2 | 34 | 1 |
| `Llyn.UIDeportment.QXiesheng` | (6) Centipede | (6) Centipede | 0 | 1 | 340 | 68 | 2 | 32 | 1 |
| `Llyn.UIDeportment.QYunjing` | (6) Centipede | (6) Centipede | 0 | 1 | 399 | 81 | 2 | 32 | 1 |
| `Llyn.Infrastructure.LEntryArchive` | (7) Serpent | (7) Serpent | 0 | 2 | 643 | 33 | 0 | 10 | 2 |

Every other type (1037) is a Hermit: one part, no flag and no hub.

## Split types

| Type | Parts | Lines | Members | Mutable | Outgoing | Incoming | Shared | Crossings | Glued | Fused | Density | Verdict |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| `Llyn.UIDeportment.QCorpus` | 9 | 1093 | 163 | 4 | 56 | 1 | 1 | 163 | 1.00 | 1.00 | 1.00 | (1) Hydra |
| `Llyn.UIDeportment.QWindow` | 7 | 856 | 110 | 3 | 49 | 31 | 1 | 112 | 1.00 | 1.00 | 1.02 | (2) Kraken |
| `Llyn.UIDeportment.QRepertoire` | 7 | 833 | 139 | 3 | 52 | 1 | 0 | 116 | 1.00 | 1.00 | 0.83 | (2) Kraken |
| `Llyn.UIDeportment.PCard` | 9 | 772 | 101 | 6 | 30 | 24 | 0 | 27 | 1.00 | 1.00 | 0.27 | (6) Centipede |
| `Llyn.Infrastructure.LEntryArchive` | 2 | 643 | 33 | 0 | 10 | 2 | 0 | 14 | 1.00 | 1.00 | 0.42 | (7) Serpent |
| `Llyn.UIDeportment.QTenor` | 3 | 463 | 75 | 2 | 34 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.QTaxonomy` | 3 | 460 | 75 | 2 | 35 | 1 | 0 | 44 | 1.00 | 1.00 | 0.59 | (6) Centipede |
| `Llyn.UIDeportment.PSentence` | 3 | 377 | 39 | 9 | 16 | 7 | 0 | 3 | 0.67 | 0.67 | 0.08 | (4) Chameleon |
| `Llyn.UIDeportment.QFavorite` | 2 | 359 | 64 | 2 | 30 | 1 | 0 | 29 | 1.00 | 1.00 | 0.45 | (6) Centipede |

## (1) Hydra

- Llyn.UIDeportment.QCorpus: parts 9, lines 1093, fused 1.00, density 1.00

## (2) Kraken

- Llyn.UIDeportment.QCorpus: lines 1093, members 163, outgoing 56, incoming 1
- Llyn.UIDeportment.QWindow: lines 856, members 110, outgoing 49, incoming 31
- Llyn.UIDeportment.QRepertoire: lines 833, members 139, outgoing 52, incoming 1

## (3) Spider

- Llyn.UIDeportment.QWindow: outgoing 49, incoming 31
- Llyn.ShellEngine.LEngine: outgoing 37, incoming 42
- Llyn.Conduct.CEditor: outgoing 33, incoming 56

## (4) Chameleon

- Llyn.UIDeportment.QReflexItem: mutable 18
- Llyn.UIDeportment.PSentence: mutable 9
- Llyn.UIDeportment.QLectern: mutable 25
- Llyn.UIDeportment.QLecternCard: mutable 11
- Llyn.UIDeportment.QLecternSound: mutable 13
- Llyn.UIDeportment.QVideoItem: mutable 7
- Llyn.UIDeportment.QLecternAccent: mutable 9

## (5) Octopus

- Llyn.UIDeportment.QCorpus: outgoing 56
- Llyn.UIDeportment.QWindow: outgoing 49
- Llyn.UIDeportment.QRepertoire: outgoing 52
- Llyn.ShellEngine.LVistaFacade: outgoing 41
- Llyn.ShellEngine.LCardFacade: outgoing 41
- Llyn.Infrastructure.LRigFactory: outgoing 65

## (6) Centipede

- Llyn.UIDeportment.QCorpus: members 163
- Llyn.UIDeportment.QWindow: members 110
- Llyn.UIDeportment.QRepertoire: members 139
- Llyn.UIDeportment.PCard: members 101
- Llyn.UIDeportment.QTenor: members 75
- Llyn.UIDeportment.QTaxonomy: members 75
- Llyn.ShellEngine.LTenure: members 40
- Llyn.UIDeportment.QYunjing: members 81
- Llyn.UIDeportment.QReflexItem: members 48
- Llyn.UIDeportment.QReference: members 77
- Llyn.UIDeportment.QFavorite: members 64
- Llyn.UIDeportment.QLectern: members 54
- Llyn.UIDeportment.QPhonology: members 70
- Llyn.UIDeportment.QGuild: members 71
- Llyn.UIDeportment.QXiesheng: members 68
- Llyn.UIDeportment.QLibrary: members 68
- Llyn.UIDeportment.QImprint: members 46
- Llyn.UIDeportment.QDisplay: members 54

## (7) Serpent

- Llyn.UIDeportment.QCorpus: lines 1093
- Llyn.UIDeportment.QWindow: lines 856
- Llyn.UIDeportment.QRepertoire: lines 833
- Llyn.UIDeportment.PCard: lines 772
- Llyn.Infrastructure.LEntryArchive: lines 643

## (8) Hub

- Llyn.UIDeportment.QCorpus: `_cCorpus` reaches 8 parts
- Llyn.UIDeportment.QWindow: `_qWindowSurface` reaches 5 parts

## Above ceiling

## Stale ceilings
