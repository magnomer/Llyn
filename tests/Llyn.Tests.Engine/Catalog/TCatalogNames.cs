using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogNames
{
    [Fact]
    public void MarkupRead_DuplicateHeadwords_CarriesSeparateDisplayNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = workspace.TWorkspaceMarkupSave(TInterface.TMarkupFormat([
            TInterface.TMarkupEntryCreate("water", "English"),
            TInterface.TMarkupEntryCreate("water", "English"),
        ]));
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        Assert.Equal(["water (1)", "water (2)"], cargo.LMarkupCargoEntry.Select(row => row.LMarkupEntryName));
        Assert.All(cargo.LMarkupCargoEntry, row => Assert.Equal("water", row.LMarkupEntryHeadword));
    }

    [Fact]
    public void MarkupRead_HeadwordAcrossLanguages_KeepsBareNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = workspace.TWorkspaceMarkupSave(TInterface.TMarkupFormat([
            TInterface.TMarkupEntryCreate("pain", "English"),
            TInterface.TMarkupEntryCreate("pain", "French"),
        ]));
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        Assert.Equal(["pain", "pain"], cargo.LMarkupCargoEntry.Select(row => row.LMarkupEntryName));
    }

    [Fact]
    public void IncomingRead_DuplicateHeadwords_CarriesTwinnedNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry target = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
        LCardDraft card = TInterface.TCardCreate("liquid", 1) with { LCardDraftTranslation = [target.LEntryId] };
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [card], []));
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [card], []));
        IReadOnlyList<LUsage> rows = engine.TEngineIncomingRead(target.LEntryId);
        Assert.Equal(["water (1)", "water (2)"], rows.Select(row => row.LUsageName));
        Assert.All(rows, row => Assert.Equal("water", row.LUsageHeadword));
    }

    [Fact]
    public void IncomingRead_HeadwordAcrossLanguages_KeepsBareNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry target = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("Brot", "German", "", "", [], []));
        LCardDraft card = TInterface.TCardCreate("bread", 1) with { LCardDraftTranslation = [target.LEntryId] };
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("pain", "English", "", "", [card], []));
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("pain", "French", "", "", [card], []));
        IReadOnlyList<LUsage> rows = engine.TEngineIncomingRead(target.LEntryId);
        Assert.Equal(["pain", "pain"], rows.Select(row => row.LUsageName));
    }
}
