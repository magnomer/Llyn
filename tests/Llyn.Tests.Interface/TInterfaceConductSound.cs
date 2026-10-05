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

    internal static CDisplay TDisplayChosenCreate(LEngine engine, LEntryPort entries, long chosen)
    {
        CDisplay display = new(
            new LDraftOutlet(engine),
            entries,
            new LPhonologyOutlet(engine),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TEnvoyFake.TEnvoyCreate(false, []),
            new CLedgerNoticed());
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        display.LDisplayVistaRestore(vista);
        vista.LVistaSelect(chosen);
        return display;
    }

    internal static CFrequency? TDisplayFrequencyRead(this CDisplay display, long? entry, string once) =>
        display.LDisplayRule.LDisplayFrequencyRead(entry, once);

    internal static int TDisplayGraspRead(this CDisplay display, long? entry) =>
        display.LDisplayRule.LDisplayGraspRead(entry);

    internal static bool TDisplayFavoriteRead(this CDisplay display, long? entry) =>
        display.LDisplayRule.LDisplayFavoriteRead(entry);

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

    internal static IReadOnlyList<CContour> TSoundingContourRead(
        IReadOnlyList<LContour> syllables, IReadOnlyList<int> scale) =>
        CSounding.LSoundingContourRead(syllables, scale);

    internal static CFold TFoldCreate(LSettingsPort settings, CEnvoy envoy) => new(settings, envoy);

    internal static IReadOnlyList<CReflex> TRespellingReflexScan(
        LPhonologyPort phonology, string language, IReadOnlyList<CReflexDraft> reflexes) =>
        CRespelling.LRespellingReflexScan(phonology, language, reflexes);

    internal static string TRespellingResolve(CRespellingMark mark, string phonetic, string? respelling) =>
        CRespelling.LRespellingResolve(mark, phonetic, respelling);

    internal static CTimbre TTimbreCreate(CEditor editor, LPhonologyPort phonology, LDraftPort drafts) =>
        new(
            editor.CEditorDesk,
            phonology,
            editor.CEditorDisplay.LDisplayRule,
            drafts,
            TInterfaceConduct.TSettingsCreate(),
            TEnvoyFake.TEnvoyCreate(false, []));
}
