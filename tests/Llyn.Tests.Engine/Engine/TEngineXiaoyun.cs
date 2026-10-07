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
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "蘭", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "孤", "見");
        long onset = engine.TEngineDiweiFind(language, LDiwei.LDiweiInitial, "來")!.LDiweiId;

        IReadOnlyList<LVistaRow> whole = engine.TEngineXiaoyunFind(language, [onset], string.Empty);
        IReadOnlyList<LVistaRow> narrowed = engine.TEngineXiaoyunFind(language, [onset], "蘭");

        Assert.Equal(["爛", "蘭"], whole.Select(row => row.LVistaRowHeadword));
        Assert.Equal(["蘭"], narrowed.Select(row => row.LVistaRowName));
    }

    [Fact]
    public void XiaoyunFind_StoredOutOfHeadwordOrder_ListsInThePanelOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "蘭", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        long onset = engine.TEngineDiweiFind(language, LDiwei.LDiweiInitial, "來")!.LDiweiId;
        LVista reverse = engine.TEngineVistaStart("xiaoyun", LCatalogOrder.LCatalogOrderReverse);

        IReadOnlyList<LVistaRow> plain = engine.TEngineXiaoyunFind(language, [onset], string.Empty);
        IReadOnlyList<LVistaRow> reversed = engine.TEngineXiaoyunFind(language, [onset], string.Empty, reverse);

        Assert.Equal(["爛", "蘭"], plain.Select(row => row.LVistaRowHeadword));
        Assert.Equal(["蘭", "爛"], reversed.Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void DiweiFind_StoredKey_AnswersTheCellAndItsColumn()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        long onset = engine.TEngineDiweiFind(language, LDiwei.LDiweiInitial, "來")!.LDiweiId;
        long rime = engine.TEngineDiweiFind(language, LDiwei.LDiweiRime, "寒 I")!.LDiweiId;

        Assert.Equal((onset, false), engine.TFanqieDiweiFind(language, LDiwei.LDiweiInitial, "來"));
        Assert.Equal((rime, true), engine.TFanqieDiweiFind(language, LDiwei.LDiweiRime, "寒 I"));
        Assert.Null(engine.TFanqieDiweiFind(language, LDiwei.LDiweiInitial, "見"));
    }
}
