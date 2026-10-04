# TInterfaceConductSound.cs
Hash: `2e10a915b2fca571`

## `internal static class TInterfaceConductSound`

The relays for Conduct's sound area and the display's sound section.
They reach the reflex lead rule, the display sound and its flag load.
They reach the display's favorite, grasp and frequency reads, so a fact can drive their failure routes.
They also reach the sound sheet, the remembered folds, the respelling rule and the sound facts.
Each relay is transparent and carries no test logic of its own.

## `internal static bool TDisplayFavoriteRead(this CDisplay display, long? entry)`

Relays the favorite read of the area's rules for an entry, so a fact can drive its failure route.

## `internal static Task<CLecternAccent?> TDisplayEnsignLoad(this CDisplaySound sound, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Relays the display's flag load, so a fact hands it a store and awaits the block it answers.

## `internal static CSounding TSoundingCreate(CDesk desk, LPhonologyPort phonology, CEnvoy envoy, LSettingsPort? pack = null)`

Builds the editor's sound sheet over a real desk and ports a test may fake.
The fake ports let a test refuse an engine call and watch the envoy.
The settings port is `pack`, or else `TInterfaceConduct.TSettingsCreate`, so a refusal reaches the envoy with a notice.
The waiting checks go through a display over the same phonology port, with a fresh repaint memory.

## `internal static CFold TFoldCreate(LSettingsPort settings, CEnvoy envoy)`

Builds the editor's remembered folds over a settings port and an envoy a test may fake.
A fake port whose saves throw lets a test watch the save failure reach the envoy.

## `internal static IReadOnlyList<CReflex> TRespellingReflexScan(LPhonologyPort phonology, string language, IReadOnlyList<CReflexDraft> reflexes)`

Relays the shared reflex scan, so a fact can hand it a fake phonology port and ready rows.

## `internal static string TRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)`

Relays the respelling rule, so a fact can read which form a mark shows.

## `internal static CTimbre TTimbreCreate(CEditor editor, LPhonologyPort phonology, LDraftPort drafts)`

Builds the editor's sound facts over the desk and display of `editor`, with ports a test may fake.
The fake draft port lets a test answer the anchor rule, and the fake phonology port the reflex guises.
Its settings port is `TInterfaceConduct.TSettingsCreate`, so a refused anchor read reaches the envoy with a notice.
Its envoy is a fake that records each notice and answers no.
