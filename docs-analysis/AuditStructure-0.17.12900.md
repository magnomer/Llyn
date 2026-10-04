# Structure audit - Llyn 0.17.12900

- Generation: 19
- Files audited: 1122
- Files outside a declared ring: 1
- Above ceiling: 0
- Stale ceilings: 0
- Stale exemptions: 0
- Poaching: 0
- Squatting: 0
- Smuggling: 0
- Hollowing: 0
- Rerouting: 0
- Backdooring: 0
- Hits held by ceilings: 23

The audit binds the source and reads the kind of every type one ring names from another. A
ring names the one neighbour the table gives it and carries the data of any ring inside it;
a class or an interface named from deeper than the neighbour is Leapfrogging, and a name from
an outer ring is Trespassing. A pure ring names only the framework namespaces its frame lists
and never touches an eavesdropping member. The project files hold the ring table edge by edge.

## Rings

| Ring | Folder | Neighbour | Pure | Cut |
|---|---|---|---|---|
| `Llyn.Core` | `src/Llyn.Core` | nothing | yes | no |
| `Llyn.Application` | `src/Llyn.Application` | `Llyn.Core` | yes | no |
| `Llyn.ShellEngine` | `src/Llyn.ShellEngine` | `Llyn.Application` | yes | no |
| `Llyn.Conduct` | `src/Llyn.Conduct` | `Llyn.ShellEngine` | yes | no |
| `Llyn.UIDeportment` | `src/Llyn.UIDeportment` | `Llyn.Conduct` | no | yes |
| `Llyn.UIVeneer` | `src/Llyn.UIVeneer` | `Llyn.UIDeportment` | no | yes |
| `Llyn.UIDemeanor` | `src/Llyn.UIDemeanor` | `Llyn.Conduct` | no | yes |
| `Llyn.UITerminal` | `src/Llyn.UITerminal` | `Llyn.UIDemeanor` | no | yes |
| `Llyn.Infrastructure` | `src/Llyn.Infrastructure` | `Llyn.Core` | no | no |
| `Llyn.UIDeportment.Capsule` | `src/Llyn.UIDeportment.Capsule` | nothing | no | yes |
| `Llyn.Core.Windows` | `src/Llyn.Core.Windows` | `Llyn.Core` | no | no |
| `Llyn.Host` (host) | `src/Llyn.Host` | `Llyn.Core`, `Llyn.Application`, `Llyn.Infrastructure`, `Llyn.Core.Windows`, `Llyn.ShellEngine`, `Llyn.Conduct`, `Llyn.UIDeportment`, `Llyn.UIVeneer`, `Llyn.UIDemeanor`, `Llyn.UITerminal` | no | no |

## Kinds

| Kind | Family | Meaning | Gate | Hits |
|---|---|---|---|---|
| `Trespassing` | Border | Outer ring named from an inner ring | held by ceiling | 0 |
| `Leapfrogging` | Border | Behaviour reached past the neighbour ring | held by ceiling | 15 |
| `Undercutting` | Border | Name from below the cut inside a UI ring | held by ceiling | 0 |
| `Leaking` | Border | Offered signature naming a type from below its neighbour | held by ceiling | 4 |
| `Unsealing` | Border | Sealed member naming a type from below its neighbour | held by ceiling | 0 |
| `Foraging` | Purity | Framework namespace outside the frame of a pure ring | held by ceiling | 0 |
| `Eavesdropping` | Purity | Eavesdropping member touched from a pure ring | held by ceiling | 0 |
| `Piggybacking` | Charter | Cut project compiling against rings past its neighbour | held by ceiling | 4 |
| `Poaching` | Border | Neighbour name outside the offers the ring may name | none allowed | 0 |
| `Squatting` | Census | Type declared in a ring that may not hold it | none allowed | 0 |
| `Smuggling` | Census | Listed word inside a folder | none allowed | 0 |
| `Hollowing` | Census | Ring holding fewer source files than its floor | none allowed | 0 |
| `Rerouting` | Charter | Project edge differing from the ring table | none allowed | 0 |
| `Backdooring` | Charter | Reference or source link bypassing the ring table | none allowed | 0 |
| `Ferrying` | Border | Data carried from a deeper ring | reported only | 2182 |
| `Commuting` | Border | Neighbour ring named | reported only | 12235 |
| Total | | | | 14440 |

## Pairs

| Pair | Files | Hits | Ceiling | Standing |
|---|---|---|---|---|
| `Leapfrogging:Llyn.Conduct>Llyn.Core` | 0 | 0 | 0 | at ceiling |
| `Leapfrogging:Llyn.ShellEngine>Llyn.Core` | 9 | 15 | 15 | at ceiling |
| `Undercutting:Llyn.UIDeportment>Llyn.Application` | 0 | 0 | 0 | at ceiling |
| `Undercutting:Llyn.UIDeportment>Llyn.Core` | 0 | 0 | 0 | at ceiling |
| `Undercutting:Llyn.UIDeportment>Llyn.ShellEngine` | 0 | 0 | 0 | at ceiling |
| `Leaking:Llyn.UIDemeanor>Llyn.Core` | 1 | 1 | 1 | at ceiling |
| `Leaking:Llyn.UIDemeanor>Llyn.ShellEngine` | 1 | 1 | 1 | at ceiling |
| `Leaking:Llyn.UIDeportment>Llyn.Core` | 1 | 1 | 1 | at ceiling |
| `Leaking:Llyn.UIDeportment>Llyn.ShellEngine` | 1 | 1 | 1 | at ceiling |
| `Unsealing:Llyn.UIDeportment>Llyn.Application` | 0 | 0 | 0 | at ceiling |
| `Unsealing:Llyn.UIDeportment>Llyn.Core` | 0 | 0 | 0 | at ceiling |
| `Unsealing:Llyn.UIDeportment>Llyn.ShellEngine` | 0 | 0 | 0 | at ceiling |
| `Piggybacking` | 4 | 4 | 4 | at ceiling |

## Violations

Every violation below is a move until the user says otherwise. A ring that reaches past its
neighbour makes the ring between them optional: the engine that calls a vault is a second
use-case layer, the view that names the engine is a second presenter. Resolve Leapfrogging
by handing the work to the neighbour ring, Foraging by moving the framework call behind a
port, and Eavesdropping by taking the clock or the file through a port. Never resolve one by
turning a class into a record so it reads as data.

**Only the user grants an exemption** by adding a row to the `exempt` block of the audit
configuration, and **only the user lowers a ceiling**, never raises one. The audit reads the
binder alone, so a finding it raises may still be correct code; say why, and cite the
`file:line` that was read.

No structural violation was found.

## Held by ceilings

These hits sit within the ceiling of their pair. They pass today, and each one fixed lowers a ceiling.

| Location | Kind | Ring | Target | Name | Source |
|---|---|---|---|---|---|
| `src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LGlyph` | `&& !LGlyph.LGlyphOtherCheck(glyph, draft.LEntryDraftTranscriptions))` |
| `src/Llyn.ShellEngine/Engine/LDraftFacade.cs:71` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LGlyph` | `if (glyph is not null && !LGlyph.LGlyphRowCheck(glyph, draft.LEntryDraftTranscriptions))` |
| `src/Llyn.ShellEngine/Engine/LDraftFacade.cs:156` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LDraft` | `saved = LEngineDraftLoad(id).LDraftNormalize();` |
| `src/Llyn.ShellEngine/Port/LEntryOutlet.cs:70` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LGlyph` | `_lEntryOutletEngine.LEngineEntry.LEngineGlyphRead(draft.LEntryDraftLanguage)?.LGlyphDivide(draft) ?? [];` |
| `src/Llyn.ShellEngine/Tenure/LPosture.cs:154` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LBulletin` | `if (bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))` |
| `src/Llyn.ShellEngine/Tenure/LPosture.cs:160` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LBulletin` | `if (!bulletin.LBulletinMatch(LSubject.LSubjectVista) \|\|` |
| `src/Llyn.ShellEngine/Tenure/LPosture.cs:202` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LLayout` | `if (record.LLayoutTabMatch(tab))` |
| `src/Llyn.ShellEngine/Tenure/LTenureObserver.cs:154` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LDraft` | `return LTenureRead()?.LDraftEtymologyFind(span);` |
| `src/Llyn.ShellEngine/Tenure/LTenureObserver.cs:161` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LDraft` | `return LTenureKeptRead()?.LDraftMentionFind(cardId, sentenceId, span);` |
| `src/Llyn.ShellEngine/Tool/LFanqieFacade.cs:175` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LDiwei` | `if (vista.LVistaChosen is long held && LDiwei.LDiweiFind(sorted, held) is null)` |
| `src/Llyn.ShellEngine/Tool/LPronunciationFacade.cs:39` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LCatalogFilter` | `return filter.LCatalogFilterApply(` |
| `src/Llyn.ShellEngine/Tool/LStemFacade.cs:89` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LStem` | `if (vista.LVistaChosen is long chosen && LStem.LStemFind(sorted, chosen) is null)` |
| `src/Llyn.ShellEngine/Vista/LReferenceFacade.cs:29` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LDraft` | `.LDraftExampleRead(cardId, sentenceId)?.LExampleDraftReference.LStateAnchorShow() ?? 0;` |
| `src/Llyn.ShellEngine/Vista/LReferenceFacade.cs:29` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LStateAnchor` | `.LDraftExampleRead(cardId, sentenceId)?.LExampleDraftReference.LStateAnchorShow() ?? 0;` |
| `src/Llyn.ShellEngine/Vista/LVistaFacade.cs:107` | `Leapfrogging` | `Llyn.ShellEngine` | `Llyn.Core` | `LCatalogFilter` | `entries = LEntryClerk.LEntryClerkMatch(parent.LVistaFilter.LCatalogFilterApply(entries,` |
| `src/Llyn.Conduct/Display/LDisplaySound.cs:39` | `Leaking` | `Llyn.UIDemeanor` | `Llyn.Core` | `LEntryDraft` | `public LEntryDraft? LDisplayShown => _lDisplaySoundDraft;` |
| `src/Llyn.Conduct/Display/LDisplay.cs:67` | `Leaking` | `Llyn.UIDemeanor` | `Llyn.ShellEngine` | `LVista` | `public void LDisplayVistaRestore(LVista vista)` |
| `src/Llyn.Conduct/Display/LDisplaySound.cs:39` | `Leaking` | `Llyn.UIDeportment` | `Llyn.Core` | `LEntryDraft` | `public LEntryDraft? LDisplayShown => _lDisplaySoundDraft;` |
| `src/Llyn.Conduct/Display/LDisplay.cs:67` | `Leaking` | `Llyn.UIDeportment` | `Llyn.ShellEngine` | `LVista` | `public void LDisplayVistaRestore(LVista vista)` |
| `src/Llyn.UIDemeanor/Llyn.UIDemeanor.csproj` | `Piggybacking` | `Llyn.UIDemeanor` | `Llyn.UIDemeanor` | `Llyn.UIDemeanor compiles against rings past its neighbour` | `` |
| `src/Llyn.UIDeportment/Llyn.UIDeportment.csproj` | `Piggybacking` | `Llyn.UIDeportment` | `Llyn.UIDeportment` | `Llyn.UIDeportment compiles against rings past its neighbour` | `` |
| `src/Llyn.UITerminal/Llyn.UITerminal.csproj` | `Piggybacking` | `Llyn.UITerminal` | `Llyn.UITerminal` | `Llyn.UITerminal compiles against rings past its neighbour` | `` |
| `src/Llyn.UIVeneer/Llyn.UIVeneer.csproj` | `Piggybacking` | `Llyn.UIVeneer` | `Llyn.UIVeneer` | `Llyn.UIVeneer compiles against rings past its neighbour` | `` |

## Names

| Name | Target | Hits | Files | Heaviest kind |
|---|---|---|---|---|
| `LDraft` | `Llyn.Core` | 4 | 3 | `Leapfrogging` |
| `LGlyph` | `Llyn.Core` | 3 | 2 | `Leapfrogging` |
| `LBulletin` | `Llyn.Core` | 2 | 1 | `Leapfrogging` |
| `LCatalogFilter` | `Llyn.Core` | 2 | 2 | `Leapfrogging` |
| `LEntryDraft` | `Llyn.Core` | 2 | 1 | `Leaking` |
| `LVista` | `Llyn.ShellEngine` | 2 | 1 | `Leaking` |
| `LDiwei` | `Llyn.Core` | 1 | 1 | `Leapfrogging` |
| `LLayout` | `Llyn.Core` | 1 | 1 | `Leapfrogging` |
| `LStateAnchor` | `Llyn.Core` | 1 | 1 | `Leapfrogging` |
| `LStem` | `Llyn.Core` | 1 | 1 | `Leapfrogging` |

## Files

| File | Hits |
|---|---|
| `src/Llyn.ShellEngine/Engine/LDraftFacade.cs` | 3 |
| `src/Llyn.ShellEngine/Tenure/LPosture.cs` | 3 |
| `src/Llyn.Conduct/Display/LDisplay.cs` | 2 |
| `src/Llyn.Conduct/Display/LDisplaySound.cs` | 2 |
| `src/Llyn.ShellEngine/Tenure/LTenureObserver.cs` | 2 |
| `src/Llyn.ShellEngine/Vista/LReferenceFacade.cs` | 2 |
| `src/Llyn.ShellEngine/Port/LEntryOutlet.cs` | 1 |
| `src/Llyn.ShellEngine/Tool/LFanqieFacade.cs` | 1 |
| `src/Llyn.ShellEngine/Tool/LPronunciationFacade.cs` | 1 |
| `src/Llyn.ShellEngine/Tool/LStemFacade.cs` | 1 |
| `src/Llyn.ShellEngine/Vista/LVistaFacade.cs` | 1 |

## Exemptions

The configuration declares no exemption.

## Stale exemptions

Every declared exemption cleared a finding.
