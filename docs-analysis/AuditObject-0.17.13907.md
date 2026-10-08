# Object audit 0.17.13907

- Generation: 21
- Enforced: True
- Types: 1109, declared in several parts: 1
- Verdicts: (1) Hydra 0, (2) Kraken 0, (3) Spider 1, (4) Chameleon 0, (5) Octopus 3, (6) Centipede 1, (7) Serpent 1, (9) Colony 0, Hermit 1103
- (1) Hydra: 0, ceiling 0
- (2) Kraken: 0, ceiling 0
- (3) Spider: 1, ceiling 1
- (4) Chameleon: 0, ceiling 0
- (5) Octopus: 3, ceiling 3
- (6) Centipede: 1, ceiling 1
- (7) Serpent: 1, ceiling 1
- (8) Hub: 0, ceiling 0
- Above ceiling: 0
- Stale ceilings: 0

A Hydra has Parts 5 and Lines 1000, and Fused 0.75 or Density 0.50. A Kraken is a Serpent or Centipede that is also an Octopus or Spider. A Spider has Outgoing 25 and Incoming 25. A Chameleon has Mutable 7. An Octopus has Outgoing 40. A Centipede has Members 40. A Serpent has Lines 500. Every value is reached at the limit or above, on the merged type. A type's verdict is its worst flag, else Colony for several parts and Hermit for one. A hub is a state slot reached from 5 or more parts. It is a finding on the slot, never a verdict, so the verdict table shows the hub count beside it.

## Verdicts

| Type | Verdict | Flags | Hubs | Parts | Lines | Members | Mutable | Outgoing | Incoming |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| `Llyn.ShellEngine.LEngine` | (3) Spider | (3) Spider | 0 | 1 | 176 | 39 | 3 | 37 | 42 |
| `Llyn.Infrastructure.LRigFactory` | (5) Octopus | (5) Octopus | 0 | 1 | 102 | 3 | 0 | 65 | 0 |
| `Llyn.ShellEngine.LCardFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 295 | 28 | 0 | 41 | 8 |
| `Llyn.ShellEngine.LVistaFacade` | (5) Octopus | (5) Octopus | 0 | 1 | 397 | 27 | 1 | 41 | 11 |
| `Llyn.ShellEngine.LTenure` | (6) Centipede | (6) Centipede | 0 | 1 | 401 | 40 | 3 | 15 | 46 |
| `Llyn.Infrastructure.LEntryArchive` | (7) Serpent | (7) Serpent | 0 | 2 | 643 | 33 | 0 | 10 | 2 |

Every other type (1103) is a Hermit: one part, no flag and no hub.

## Split types

| Type | Parts | Lines | Members | Mutable | Outgoing | Incoming | Shared | Crossings | Glued | Fused | Density | Verdict |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| `Llyn.Infrastructure.LEntryArchive` | 2 | 643 | 33 | 0 | 10 | 2 | 0 | 14 | 1.00 | 1.00 | 0.42 | (7) Serpent |

## (1) Hydra

## (2) Kraken

## (3) Spider

- Llyn.ShellEngine.LEngine: outgoing 37, incoming 42

## (4) Chameleon

## (5) Octopus

- Llyn.ShellEngine.LVistaFacade: outgoing 41
- Llyn.ShellEngine.LCardFacade: outgoing 41
- Llyn.Infrastructure.LRigFactory: outgoing 65

## (6) Centipede

- Llyn.ShellEngine.LTenure: members 40

## (7) Serpent

- Llyn.Infrastructure.LEntryArchive: lines 643

## (8) Hub

## Above ceiling

## Stale ceilings
