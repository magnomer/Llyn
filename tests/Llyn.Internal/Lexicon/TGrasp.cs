using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGrasp
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void GraspCheck_HalfStepCount_AcceptsZeroToTen(int grasp, bool expected)
    {
        Assert.Equal(expected, TInterface.TGraspCheck(grasp));
    }

    [Fact]
    public void GraspSave_RatedEntry_ReadsBackAndRaisesBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TGraspEntryCreate(engine, "word");
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);

        Assert.Equal(0, engine.TEngineGraspRead(entry.LEntryId));

        engine.TEngineGraspSave(entry.LEntryId, 7);

        Assert.Equal(7, engine.TEngineGraspRead(entry.LEntryId));
        Assert.Equal(7, engine.TEngineEntryRead(entry.LEntryId)!.LEntryGrasp);
        LBulletin bulletin = Assert.Single(bulletins);
        Assert.Equal(LSubject.LSubjectGrasp, bulletin.LBulletinSubject);
        Assert.Equal(entry.LEntryId, bulletin.LBulletinId);
    }

    [Fact]
    public void GraspSave_Zero_ClearsTheRating()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TGraspEntryCreate(engine, "word");
        engine.TEngineGraspSave(entry.LEntryId, 3);

        engine.TEngineGraspSave(entry.LEntryId, 0);

        Assert.Equal(0, engine.TEngineGraspRead(entry.LEntryId));
    }

    [Fact]
    public void EntryUpdate_RatedEntry_KeepsTheRating()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TGraspEntryCreate(engine, "word");
        engine.TEngineGraspSave(entry.LEntryId, 9);

        LEntryDraft draft = engine.TEngineEntryLoad(entry.LEntryId)!;
        LEntry updated = engine.TEngineEntryUpdate(
            entry.LEntryId, draft with { LEntryDraftHeadword = "words" });

        Assert.Equal("words", updated.LEntryHeadword);
        Assert.Equal(9, engine.TEngineGraspRead(entry.LEntryId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void GraspSave_OutOfRange_ThrowsBeforeWriting(int grasp)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TGraspEntryCreate(engine, "word");
        engine.TEngineGraspSave(entry.LEntryId, 4);
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);

        Assert.Throws<ArgumentOutOfRangeException>(() => engine.TEngineGraspSave(entry.LEntryId, grasp));

        Assert.Equal(4, engine.TEngineGraspRead(entry.LEntryId));
        Assert.Empty(bulletins);
    }

    [Fact]
    public void GraspSave_MissingEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<InvalidOperationException>(() => engine.TEngineGraspSave(404, 2));
        Assert.Equal(0, engine.TEngineGraspRead(404));
    }

    [Fact]
    public void FavoriteFind_RatedEntry_CarriesTheRating()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TGraspEntryCreate(engine, "word");
        engine.TEngineFavoriteSave(entry.LEntryId);
        engine.TEngineGraspSave(entry.LEntryId, 5);

        LCatalogFavorite favorite =
            Assert.Single(engine.TEngineFavoriteFind(string.Empty, LCatalogOrder.LCatalogOrderRecent));

        Assert.Equal(5, favorite.LCatalogFavoriteEntry.LEntryGrasp);
    }

    private static LEntry TGraspEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            []));
    }
}
