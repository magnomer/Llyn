using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineXiaoyun
{
    [Fact]
    public void XiaoyunFind_QueryFiltersHeadword()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunDiweiPlace(engine, workspace, language, "蘭", "來");
        TXiaoyunDiweiPlace(engine, workspace, language, "孤", "見");
        long onset = engine.TEngineDiweiFind(language, LDiwei.LDiweiInitial, "來")!.LDiweiId;

        IReadOnlyList<LVistaRow> whole = engine.TEngineXiaoyunFind(language, [onset], string.Empty);
        IReadOnlyList<LVistaRow> narrowed = engine.TEngineXiaoyunFind(language, [onset], "蘭");

        Assert.Equal(["爛", "蘭"], whole.Select(row => row.LVistaRowHeadword));
        Assert.Equal(["蘭"], narrowed.Select(row => row.LVistaRowName));
    }

    private static void TXiaoyunDiweiPlace(
        LEngine engine, TWorkspace workspace, string language, string character, string initial)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            character,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", "", "a")]));

        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, character, [TInterface.TFanqieRowCreate(character, 0, initial, "寒", "一", "平")]);
        TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase).TDiweiApply(language, character, null);

        IReadOnlyList<long> anchors =
            fanqie.TFanqieRead(language, character).Select(row => row.LFanqieRowId).ToList();
        engine.TEntryAnchorApply(entry.LEntryId, anchors);
    }
}
