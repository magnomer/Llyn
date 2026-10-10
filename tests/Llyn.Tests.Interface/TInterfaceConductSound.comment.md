# TInterfaceConductSound.cs
Hash: `2217399691392ca6`

## `internal static class TInterfaceConductSound`

The relays for Conduct's sound area and the display's sound section.
They reach the reflex lead rule, the display sound and its flag load.
They reach the display's favorite, grasp and frequency reads, so a fact can drive their failure routes.
One builds a display over a port bundle a fact fakes in part, with an entry chosen.
They also reach the sound sheet, the entry-stored box folds, the respelling rule and the sound facts.
Each relay is transparent and carries no test logic of its own.

## `internal static IReadOnlyList<bool> TReflexLeadRead(IReadOnlyList<string> languages)`

Relays the reflex lead rule over the given languages.

## `internal static IReadOnlyList<CStemMember> TStemMemberRead(IReadOnlyList<LStemMember> members)`

Relays the mapping of engine series members to Conduct members.

## `internal static LDisplaySound TDisplaySoundCreate(LEngine engine)`

Builds the display sound over real facades and outlets on `engine`.

## `internal static bool TDisplaySpreadCheck(this LDisplaySound sound, long? id)`

Relays whether entry `id` has "More readings" opened, as the display sound reads it from the engine.

## `internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft)`

Relays the display sound's show of an entry draft under its id.

## `internal static void TDisplaySoundClear(this LDisplaySound sound)`

Relays the display sound's clear.

## `internal static void TDisplayReflexLoad(this LDisplaySound sound)`

Relays the display sound's load of the reflex rows.

## `internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound)`

Reads the reflex rows the display sound holds.

## `internal static CDisplay TDisplayChosenCreate(LEngine engine, CEntryBundle entries, long chosen, CPhonologyBundle? phonology = null, List<string>? asked = null)`

Builds a display over `entries` and real outlets on `engine`, bound to a fresh library vista.
The vista chooses `chosen`, so the display's own reads ask `entries` about that entry.
A fake port in `entries` lets a fact answer the grasp and frequency reads with hostile values.
A given `phonology` replaces the real sound ports, so a fact can fault one.
The envoy notes every failure in `asked` when given.
Its envoy answers no and records failures in the supplied or discarded list.
Its media port is a bare stub.

## `internal static CFrequency? TDisplayFrequencyRead(this CDisplay display, long? entry, string once)`

Relays the frequency read of the area's rules for an entry, so a fact can drive its failure route.

## `internal static int TDisplayGraspRead(this CDisplay display, long? entry)`

Relays the grasp read of the area's rules for an entry, so a fact can drive its failure route.

## `internal static bool TDisplayFavoriteRead(this CDisplay display, long? entry)`

Relays the favorite read of the area's rules for an entry, so a fact can drive its failure route.

## `internal static Task<CLecternAccent?> TDisplayAccentLoad(this CDisplayAccent accent, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Relays the accent area's flag load, so a fact hands it a store and awaits the block it answers.

## `internal static CSounding TSoundingCreate(CDesk desk, CPhonologyBundle phonology, CEnvoy envoy, LSettingsPort? pack = null)`

Builds the editor's sound sheet over a real desk and ports a test may fake.
The fake ports let a test refuse an engine call and watch the envoy.
The settings port is `pack`, or else `TInterfaceConduct.TSettingsCreate`, so a refusal reaches the envoy with a notice.
The waiting checks go through a display over the same phonology bundle, with a fresh repaint memory.

## `internal static IReadOnlyList<CContour> TContourRead(IReadOnlyList<LContour> syllables, IReadOnlyList<int> scale)`

Relays the contour map over a given scale, so a fact feeds levels outside it.

## `internal static CFold TFoldCreate(TEditorFixture editor, CPhonologyBundle phonology, List<string> asked)`

Builds the editor's box folds over the stored entry of `editor`'s desk and the reflex port of `phonology`.
A fault bundle whose box write throws lets a test watch the failure reach the envoy.
The envoy notes every failure in `asked`.
It holds its own repaint memory, so a refused read shows once per fold.

## `internal static IReadOnlyList<CReflex> TRespellingReflexScan(LReflexPort port, string language, IReadOnlyList<CReflexDraft> reflexes)`

Relays the shared reflex scan's rows, so a fact can hand it a fake reflex port and ready rows.

## `internal static string TRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)`

Relays the respelling rule, so a fact can read which form a mark shows.

## `internal static CKindred TKindredCreate(TEditorFixture editor, CPhonologyBundle phonology, LDraftPort drafts, List<string>? asked = null)`

Builds the editor's reflex block over the desk and display facets of `editor`, with ports a test may fake.
The fake draft port lets a test answer the anchor rule, and the fake bundle's reflex port the reflex guises.
Its settings port is `TInterfaceConduct.TSettingsCreate`, so a refused anchor read reaches the envoy with a notice.
The envoy notes every failure in `asked` when given.
Its envoy answers no and uses a discarded list when `asked` is absent.
