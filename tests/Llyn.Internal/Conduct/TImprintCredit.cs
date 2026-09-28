using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TImprintCredit
{
    [Fact]
    public void ImprintCreditRead_FreshDraft_OpensBlankRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);

        Assert.Empty(imprint.CImprintCreditRead());
        Assert.Equal(-1, imprint.CImprintBlankAt);

        imprint.CImprintOpen(null);

        Assert.Empty(imprint.CImprintCreditRead());
        Assert.Equal(0, imprint.CImprintBlankAt);
    }

    [Fact]
    public void ImprintAuthorAdd_ThenRemoveBlank_OpensAndClosesRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor ada = TImprintOpen(engine, imprint, "Ada").Single();
        int focused = 0;
        imprint.CImprintFocused += () => focused++;

        imprint.CImprintAuthorAdd(0, ada.LAuthorId);

        Assert.Equal(1, focused);
        Assert.Equal(
            [(ada.LAuthorId, 0)],
            imprint.CImprintCreditRead().Select(row => (row.CAuthorRowId, row.CAuthorRowPosition)));
        Assert.Equal(1, imprint.CImprintBlankAt);

        imprint.CImprintAuthorRemove(0);

        Assert.Equal(["Ada"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));
        Assert.Equal(-1, imprint.CImprintBlankAt);
        Assert.False(imprint.CImprintDesk.CDeskChangeCheck());
    }

    [Fact]
    public void ImprintAuthorMove_FirstCreditLater_ShiftsItBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor ada = TImprintOpen(engine, imprint, "Ada", "Bob")[0];

        imprint.CImprintAuthorMove(0, ada.LAuthorId, 1);

        Assert.Equal(["Bob", "Ada"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));
        Assert.Equal([false, true], imprint.CImprintCreditRead().Select(row => row.CAuthorRowEarlier));
        Assert.True(imprint.CImprintDesk.CDeskChangeCheck());

        imprint.CImprintAuthorMove(1, ada.LAuthorId, -1);

        Assert.Equal(["Ada", "Bob"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));
    }

    [Fact]
    public void ImprintAuthorMove_BlankRow_SendsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        TImprintOpen(engine, imprint, "Ada", "Bob");

        imprint.CImprintAuthorMove(0, 0, 1);
        imprint.CImprintAuthorMove(null, 1, 1);

        Assert.False(imprint.CImprintDesk.CDeskChangeCheck());
    }

    [Fact]
    public void ImprintAuthorFinish_UnmatchedName_CreatesCredit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        imprint.CImprintOpen(null);
        int reverted = 0;
        imprint.CImprintReverted += () => reverted++;

        Assert.True(imprint.CImprintAuthorFinish(0, 0, " ", null));
        Assert.Equal(1, reverted);
        Assert.True(imprint.CImprintAuthorFinish(0, 0, " Ada ", null));

        IReadOnlyList<CAuthorRow> rows = imprint.CImprintCreditRead();
        Assert.Equal(["Ada"], rows.Select(row => row.CAuthorRowName));
        Assert.Equal(-1, imprint.CImprintBlankAt);
        Assert.True(imprint.CImprintAuthorFinish(0, rows[0].CAuthorRowId, "Ada", null));
        Assert.Equal(["Ada"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));

        imprint.CImprintTitleSet("Book");
        imprint.CImprintSave();

        Assert.Equal(["Ada"], engine.TEngineAuthorFind(string.Empty).Select(author => author.LAuthorName));
    }

    [Fact]
    public void ImprintAuthorFinish_BlankNameOnBlankRow_KeepsTheBlankRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        LAuthor ada = TImprintOpen(engine, imprint, "Ada").Single();
        imprint.CImprintAuthorAdd(0, ada.LAuthorId);
        int reverted = 0;
        imprint.CImprintReverted += () => reverted++;

        Assert.True(imprint.CImprintAuthorFinish(1, 0, " ", null));

        Assert.Equal(1, reverted);
        Assert.Equal(1, imprint.CImprintBlankAt);
        Assert.False(imprint.CImprintDesk.CDeskChangeCheck());

        Assert.True(imprint.CImprintAuthorFinish(1, 0, "Bob", null));

        Assert.Equal(1, reverted);
        Assert.Equal(["Ada", "Bob"], imprint.CImprintCreditRead().Select(row => row.CAuthorRowName));
        Assert.Equal(-1, imprint.CImprintBlankAt);
    }

    [Fact]
    public void ImprintAuthorFinish_NoRowOrNoDraft_IsNotHandled()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);

        Assert.False(imprint.CImprintAuthorFinish(0, 0, "Ada", null));
        Assert.False(imprint.CImprintAuthorCancel(0, 0));

        imprint.CImprintOpen(null);

        Assert.False(imprint.CImprintAuthorFinish(null, 0, "Ada", null));
        Assert.False(imprint.CImprintAuthorCancel(0, null));
    }

    [Fact]
    public void ImprintAuthorCancel_NoBylineOffered_RevertsTheField()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprint.TImprintPrepare(engine, atelier);
        imprint.CImprintOpen(null);
        int reverted = 0;
        imprint.CImprintReverted += () => reverted++;

        Assert.True(imprint.CImprintAuthorCancel(0, 0));

        Assert.Equal(1, reverted);
        Assert.False(imprint.CImprintDesk.CDeskChangeCheck());
    }

    private static List<LAuthor> TImprintOpen(LEngine engine, CImprint imprint, params string[] names)
    {
        LReference book = engine.TEngineCitationCreate("Book");
        List<LAuthor> authors = [];
        foreach (string name in names)
        {
            LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, name));
            engine.TRequestCreditApply(book.LReferenceId, author.LAuthorId, authors.Count);
            authors.Add(author);
        }

        imprint.CImprintOpen(book.LReferenceId);
        return authors;
    }
}
