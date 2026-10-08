using System;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEditor
{
    internal static CEditor TEditorCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        TInterfaceConduct.TEntryBundleCreate(engine),
        TInterfaceConduct.TPhonologyBundleCreate(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        TEnvoyFake.TEnvoyCreate(false, []),
        new CLedgerNoticed(),
        static run => run());

    internal static CEditor TEditorCreate(LEngine engine, Action<Action> marshal) => new(
        new LDraftOutlet(engine),
        TInterfaceConduct.TEntryBundleCreate(engine),
        TInterfaceConduct.TPhonologyBundleCreate(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        TEnvoyFake.TEnvoyCreate(false, []),
        new CLedgerNoticed(),
        marshal);

    internal static CEditor TEditorCreate(LEngine engine, CEnvoy envoy) => new(
        new LDraftOutlet(engine),
        TInterfaceConduct.TEntryBundleCreate(engine),
        TInterfaceConduct.TPhonologyBundleCreate(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        envoy,
        new CLedgerNoticed(),
        static run => run());

    internal static CEditor TEditorCreate(LEngine engine, CPhonologyBundle phonology) =>
        TEditorCreate(
            new LDraftOutlet(engine),
            TInterfaceConduct.TEntryBundleCreate(engine),
            phonology,
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>());

    internal static CEditor TEditorCreate(
        LDraftPort drafts,
        CEntryBundle entries,
        CPhonologyBundle phonology,
        LSettingsPort settings,
        LMediaPort media) =>
        TEditorCreate(drafts, entries, phonology, settings, media, TEnvoyFake.TEnvoyCreate(false, []));

    internal static CEditor TEditorCreate(
        LDraftPort drafts,
        CEntryBundle entries,
        CPhonologyBundle phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy) =>
        new(drafts, entries, phonology, settings, media, envoy, new CLedgerNoticed(), static run => run());

    internal static void TEditorVistaRestore(this CEditor editor, LVista vista) => editor.LEditorVistaRestore(vista);

    internal static CEntryDraft? TEditorDraftRead(this CEditor editor) =>
        editor.CEditorDesk.CDeskDraft.CDeskDraftTenure is LTenure held
        && editor.CEditorDesk.CDeskDraft.CDeskDraftChip is LQuillChip chip
        && held.LTenureRead() is { } draft
            ? CFolio.CFolioEntryRead(
                draft.LDraftContent,
                chip.LQuillTranslationRead(draft.LDraftContent),
                editor.CEditorDisplay.LDisplayRule.LDisplayMediaPort)
            : null;

    internal static bool TEditorFinish(this CEditor editor, bool store) => editor.LEditorFinish(store);
}
