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

    internal static IReadOnlyList<CStemMember> TStemMemberRead(IReadOnlyList<LStemMember> members) =>
        CStemMember.LStemMemberRead(members);

    internal static CVoice TDisplaySoundCreate(LEngine engine) => new(
        engine.LEngineEntry,
        engine.LEngineLanguage,
        engine.LEngineReflex,
        engine.LEngineFanqie,
        engine.LEngineScript,
        engine.LEngineVocabulary,
        new LMediaOutlet(engine),
        new LSettingsOutlet(engine));

    internal static bool TDisplaySpreadCheck(this CVoice sound, long? id) => sound.CVoiceSpreadCheck(id);

    internal static void TDisplaySoundShow(this CVoice sound, long? id, LEntryDraft draft) =>
        sound.CVoiceShow(id, draft);

    internal static void TDisplaySoundClear(this CVoice sound) => sound.CVoiceClear();

    internal static void TDisplayReflexLoad(this CVoice sound) => sound.CVoiceReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this CVoice sound) =>
        sound.CVoiceReflexRead();

    internal static CDisplay TDisplayChosenCreate(
        LEngine engine,
        CEntryBundle entries,
        long chosen,
        CPhonologyBundle? phonology = null,
        List<string>? asked = null)
    {
        CDisplay display = new(
            new LDraftOutlet(engine),
            entries,
            phonology ?? TInterfaceConduct.TPhonologyBundleCreate(engine),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TEnvoyFake.TEnvoyCreate(false, asked ?? []),
            new CLedgerNoticed());
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        display.LDisplayVistaRestore(vista);
        vista.LVistaSelect(chosen);
        return display;
    }

    internal static CFrequency? TDisplayFrequencyRead(this CDisplay display, long? entry, string once) =>
        display.LDisplayRule.CRuleFrequencyRead(entry, once);

    internal static int TDisplayGraspRead(this CDisplay display, long? entry) =>
        display.LDisplayRule.CRuleGraspRead(entry);

    internal static bool TDisplayFavoriteRead(this CDisplay display, long? entry) =>
        display.LDisplayRule.CRuleFavoriteRead(entry);

    internal static Task<CLecternAccent?> TDisplayAccentLoad(
        this CDisplayAccent accent, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        accent.CDisplayAccentLoad(store);

    internal static CSounding TSoundingCreate(
        CDesk desk, CPhonologyBundle phonology, CEnvoy envoy, LSettingsPort? pack = null)
    {
        LSettingsPort settings = pack ?? TInterfaceConduct.TSettingsCreate();
        return new(
            desk,
            phonology.CPhonologyBundleFanqie,
            phonology.CPhonologyBundleDiwei,
            phonology.CPhonologyBundleScript,
            phonology.CPhonologyBundleParadigm,
            settings,
            new CRule(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                TEngineFake.TEngineStubCreate<LFavoritePort>(),
                TEngineFake.TEngineStubCreate<LVistaPort>(),
                TEngineFake.TEngineStubCreate<LGraspPort>(),
                TEngineFake.TEngineStubCreate<LPronunciationPort>(),
                phonology.CPhonologyBundleLanguage,
                phonology.CPhonologyBundleReflex,
                phonology.CPhonologyBundleFanqie,
                phonology.CPhonologyBundleScript,
                phonology.CPhonologyBundleParadigm,
                settings,
                TEngineFake.TEngineStubCreate<LMediaPort>(),
                envoy,
                new CLedgerNoticed()),
            envoy);
    }

    internal static IReadOnlyList<CContour> TContourRead(
        IReadOnlyList<LContour> syllables, IReadOnlyList<int> scale) =>
        CContour.CContourRead(syllables, scale);

    internal static CFold TFoldCreate(TEditorFixture editor, CPhonologyBundle phonology, List<string> asked) =>
        new(
            editor.TEditorFixtureDesk.CDeskStoredRead,
            phonology.CPhonologyBundleReflex,
            TInterfaceConduct.TSettingsCreate(),
            TEnvoyFake.TEnvoyCreate(false, asked),
            new CLedgerNoticed());

    internal static IReadOnlyList<CReflex> TRespellingReflexScan(
        LReflexPort port, string language, IReadOnlyList<LReflexDraft> reflexes) =>
        CRespelling.LRespellingReflexScan(port, language, reflexes);

    internal static CKindred TKindredCreate(
        TEditorFixture editor, CPhonologyBundle phonology, LDraftPort drafts, List<string>? asked = null) =>
        new(
            editor.TEditorFixtureDesk,
            phonology.CPhonologyBundleReflex,
            editor.TEditorFixtureDisplay.LDisplayRule,
            drafts,
            TInterfaceConduct.TSettingsCreate(),
            TEnvoyFake.TEnvoyCreate(false, asked ?? []),
            new CLedgerNoticed());
}
