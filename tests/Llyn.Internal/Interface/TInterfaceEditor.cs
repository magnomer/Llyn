using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEditor
{
    internal static CEditor TEditorCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        TEnvoyFake.TEnvoyCreate(false, []));

    internal static CEditor TEditorCreate(LEngine engine, LPhonologyPort phonology) =>
        TEditorCreate(
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            phonology,
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>());

    internal static CEditor TEditorCreate(
        LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media) =>
        TEditorCreate(drafts, entries, phonology, settings, media, TEnvoyFake.TEnvoyCreate(false, []));

    internal static CEditor TEditorCreate(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy) =>
        new(drafts, entries, phonology, settings, media, envoy);

    internal static void TEditorVistaRestore(this CEditor editor, LVista vista) => editor.LEditorVistaRestore(vista);

    internal static CEntryDraft? TEditorDraftRead(this CEditor editor) =>
        editor.CEditorDesk.CDeskTenure is LTenure held && held.LTenureRead() is { } draft
            ? CFolio.CFolioEntryRead(
                draft.LDraftContent,
                held.LTenureTranslationRead(draft.LDraftContent),
                editor.CEditorDisplay.LDisplayMediaPort)
            : null;

    internal static bool TEditorFinish(this CEditor editor, bool store) => editor.LEditorFinish(store);
}
