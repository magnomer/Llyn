using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFellow
{
    [Fact]
    public void FellowFind_TwoSharedWorks_CountsTwo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor read = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Read"));
        LAuthor fellow = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Fellow"));
        TFellowReferenceCreate(engine, "First", read, fellow);
        TFellowReferenceCreate(engine, "Second", read, fellow);

        IReadOnlyList<LFellow> fellows = engine.TEngineFellowFind(read.LAuthorId);

        Assert.Equal([TInterface.TFellowCreate(fellow.LAuthorId, "Fellow", 2)], fellows);
    }

    [Fact]
    public void FellowFind_SharedOnTwoSources_WordsTheCountAfterTheClerkRaisedIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor read = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Read"));
        LAuthor fellow = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Fellow"));
        TFellowReferenceCreate(engine, "First", read, fellow);
        TFellowReferenceCreate(engine, "Second", read, fellow);

        IReadOnlyList<LFellow> fellows = engine.TEngineFellowFind(read.LAuthorId);

        Assert.Equal(["2"], fellows.Select(row => row.LFellowCount));
    }

    [Fact]
    public void FellowFind_SortsBySharedThenName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor read = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Read"));
        LAuthor once = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Zed"));
        LAuthor twice = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Yan"));
        LAuthor alone = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Abe"));
        TFellowReferenceCreate(engine, "First", read, twice, once);
        TFellowReferenceCreate(engine, "Second", read, twice, alone);
        TFellowReferenceCreate(engine, "Third", alone);

        IReadOnlyList<LFellow> fellows = engine.TEngineFellowFind(read.LAuthorId);

        Assert.Equal(["Yan", "Abe", "Zed"], fellows.Select(row => row.LFellowName));
        Assert.Equal([2, 1, 1], fellows.Select(row => row.LFellowShared));
    }

    [Fact]
    public void UnionRead_NoDraftAndMissingKept_ReadsBothEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        (string dropped, string kept) = engine.TEngineUnionRead(null, 999);

        Assert.Equal(string.Empty, dropped);
        Assert.Equal(string.Empty, kept);
    }

    [Fact]
    public void UnionRead_PaddedKeptName_ReadsItTrimmed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor kept = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "  Adam "));

        Assert.Equal("Adam", engine.TEngineUnionRead(null, kept.LAuthorId).LUnionKept);
    }

    private static void TFellowReferenceCreate(LEngine engine, string title, params LAuthor[] credited)
    {
        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        for (int position = 0; position < credited.Length; position++)
        {
            engine.TRequestCreditApply(reference.LReferenceId, credited[position].LAuthorId, position);
        }
    }
}
