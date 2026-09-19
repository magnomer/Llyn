using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TImprintCredit
{
    [Fact]
    public void CreditRead_FreshDraft_OpensBlankRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);

        Assert.Empty(shelf.LShelfImprint.TImprintCreditRead());
        Assert.Equal(-1, shelf.LShelfImprint.LImprintBlankAt);

        shelf.TShelfFreshStart();

        Assert.Empty(shelf.LShelfImprint.TImprintCreditRead());
        Assert.Equal(0, shelf.LShelfImprint.LImprintBlankAt);
    }

    [Fact]
    public void CreditApply_AddThenRemoveBlank_OpensAndClosesRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorAttach(book.LReferenceId, ada.LAuthorId, 0);
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfScribeSet(true);
        LImprint imprint = shelf.LShelfImprint;
        int focused = 0;
        imprint.LImprintFocused += () => focused++;

        imprint.TImprintCreditApply("Add", 0, ada.LAuthorId);

        Assert.Equal(1, focused);
        Assert.Equal(
            [(ada.LAuthorId, 0)],
            imprint.TImprintCreditRead().Select(row => (row.LAuthorRowId, row.LAuthorRowPosition)));
        Assert.Equal(1, imprint.LImprintBlankAt);

        imprint.TImprintCreditApply("Remove", 1, 0);

        Assert.Equal(["Ada"], imprint.TImprintCreditRead().Select(row => row.LAuthorRowName));
        Assert.Equal(-1, imprint.LImprintBlankAt);
        Assert.False(imprint.LImprintDesk.TDeskChangeCheck());
    }

    [Fact]
    public void CreditApply_Later_ShiftsCredit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TEngineAuthorAttach(book.LReferenceId, ada.LAuthorId, 0);
        engine.TEngineAuthorAttach(book.LReferenceId, bob.LAuthorId, 1);
        shelf.TShelfRowSelect(book.LReferenceId);
        shelf.TShelfScribeSet(true);
        LImprint imprint = shelf.LShelfImprint;

        imprint.TImprintCreditApply("Later", 0, ada.LAuthorId);

        Assert.Equal(["Bob", "Ada"], imprint.TImprintCreditRead().Select(row => row.LAuthorRowName));
        Assert.Equal([false, true], imprint.TImprintCreditRead().Select(row => row.LAuthorRowEarlier));
        Assert.True(imprint.LImprintDesk.TDeskChangeCheck());
    }

    [Fact]
    public void KeyApply_EnterUnmatchedName_CreatesCredit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LShelf shelf = TShelfPrepare(engine);
        shelf.TShelfFreshStart();
        LImprint imprint = shelf.LShelfImprint;
        int reverted = 0;
        imprint.LImprintReverted += () => reverted++;

        Assert.True(imprint.TImprintKeyApply("Enter", 0, 0, " ", null));
        Assert.Equal(1, reverted);
        Assert.True(imprint.TImprintKeyApply("Enter", 0, 0, " Ada ", null));

        IReadOnlyList<LAuthorRow> rows = imprint.TImprintCreditRead();
        Assert.Equal(["Ada"], rows.Select(row => row.LAuthorRowName));
        Assert.Equal(-1, imprint.LImprintBlankAt);
        Assert.False(imprint.TImprintKeyApply("A", 0, rows[0].LAuthorRowId, "Ada", null));
        Assert.True(imprint.TImprintKeyApply("Enter", 0, rows[0].LAuthorRowId, "Ada", null));
        Assert.Equal(["Ada"], imprint.TImprintCreditRead().Select(row => row.LAuthorRowName));

        imprint.TImprintTitleSet("Book");
        imprint.TImprintSave();

        Assert.Equal(["Ada"], engine.TEngineAuthorRead().Select(author => author.LAuthorName));
    }

    private static LShelf TShelfPrepare(LEngine engine)
    {
        LShelf shelf = TInterfaceDeportment.TShelfCreate(engine, () => true);
        shelf.TShelfVistaRestore(
            engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword));
        return shelf;
    }
}
