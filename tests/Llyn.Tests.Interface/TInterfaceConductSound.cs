using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductSound
{
    internal static IReadOnlyList<bool> TReflexLeadRead(IReadOnlyList<string> languages) =>
        CReflex.LReflexLeadRead(languages);

    internal static LDisplaySound TDisplaySoundCreate(LEngine engine) => new(
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LMediaOutlet(engine),
        new LSettingsOutlet(engine));

    internal static void TDisplayFoldSet(this LDisplaySound sound, bool opened) => sound.LDisplayFoldSet(opened);

    internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft) =>
        sound.LDisplaySoundShow(id, draft);

    internal static void TDisplaySoundClear(this LDisplaySound sound) => sound.LDisplaySoundClear();

    internal static void TDisplayReflexLoad(this LDisplaySound sound) => sound.LDisplayReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound) =>
        sound.LDisplayReflexRead();

    internal static CFrequency? TDisplayFrequencyRead(this LDisplay display, long? entry, string once) =>
        display.LDisplayFrequencyRead(entry, once);

    internal static int TDisplayGraspRead(this LDisplay display, long? entry) => display.LDisplayGraspRead(entry);

    internal static bool TDisplayFavoriteRead(this LDisplay display, long? entry) =>
        display.LDisplayFavoriteRead(entry);

    internal static Task<CLecternAccent?> TDisplayEnsignLoad(
        this CDisplaySound sound, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        sound.CDisplayEnsignLoad(store);

    internal static CSounding TSoundingCreate(
        CDesk desk, LPhonologyPort phonology, CEnvoy envoy, LSettingsPort? pack = null)
    {
        LSettingsPort settings = pack ?? TInterfaceConduct.TSettingsCreate();
        return new(
            desk,
            phonology,
            settings,
            new LDisplay(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                phonology,
                settings,
                TEngineFake.TEngineStubCreate<LMediaPort>(),
                envoy,
                new CLedgerNoticed()),
            envoy);
    }

    internal static IReadOnlyList<CReflex> TRespellingReflexScan(
        LPhonologyPort phonology, string language, IReadOnlyList<CReflexDraft> reflexes) =>
        CRespelling.LRespellingReflexScan(phonology, language, reflexes);

    internal static string TRespellingResolve(CRespellingMark mark, string phonetic, string? respelling) =>
        CRespelling.LRespellingResolve(mark, phonetic, respelling);

    internal static CTimbre TTimbreCreate(CEditor editor, LPhonologyPort phonology, LDraftPort drafts) =>
        new(
            editor.CEditorDesk,
            phonology,
            editor.CEditorDisplay,
            drafts,
            TInterfaceConduct.TSettingsCreate(),
            TEnvoyFake.TEnvoyCreate(false, []));
}
