using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TSituation
{
    [Fact]
    public void ASituationIsRewrittenOnceAndEveryCardReadsTheNewWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at a funeral", null, null));
        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(
            collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.LEngineSituationUpdate(situation with
        {
            LSituationTitle = "at a memorial",
            LSituationDescription = "spoken to the bereaved",
            LSituationKind = "register",
        });

        LSituation read = Assert.Single(engine.LEngineSituationRead(senseId, LOwner.LOwnerSense));
        Assert.Equal("at a memorial", read.LSituationTitle);
        Assert.Equal(
            "at a memorial",
            Assert.Single(engine.LEngineSituationRead(collocationId, LOwner.LOwnerCollocation))
                .LSituationTitle);
        Assert.Equal("spoken to the bereaved", engine.LEngineSituationRead(
            situation.LSituationId)?.LSituationDescription);
    }

    [Fact]
    public void TheOrderACardHoldsIsTheOrderTheSituationsComeBackIn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LSituation first = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null));
        LSituation second = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at home", null, null));

        engine.LEngineSituationAttach(senseId, first.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(senseId, second.LSituationId, 0, LOwner.LOwnerSense);

        Assert.Equal(
            ["at home", "in court"],
            engine.LEngineSituationRead(senseId, LOwner.LOwnerSense)
                .Select(row => row.LSituationTitle));
    }

    [Fact]
    public void ARemovedLastReferenceTakesTheSituationWhileADeleteIsRefusedBeforeThat()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null));
        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);

        Assert.Throws<InvalidOperationException>(() =>
            engine.LEngineSituationDelete(situation.LSituationId));

        engine.LEngineSituationDetach(senseId, situation.LSituationId, LOwner.LOwnerSense);
        Assert.NotNull(engine.LEngineSituationRead(situation.LSituationId));

        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationRemove(senseId, situation.LSituationId, LOwner.LOwnerSense);
        Assert.Null(engine.LEngineSituationRead(situation.LSituationId));
    }

    private static LEntry TSituationEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [])]));
    }
}
