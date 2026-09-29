using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelfImprint
{
    [Fact]
    public void ShelfClose_BylineOffered_ClosesTheBylineWhenTheAtelierCloses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        shelf.CShelfReferenceCreate();
        CByline byline = shelf.CShelfImprint.CImprintByline;
        byline.CBylineWordSet("Ad", true);
        Assert.Single(byline.CBylineRowsRead());
        int changed = 0;
        byline.CBylineChanged += () => changed++;

        atelier.CAtelierClose();

        Assert.False(byline.CBylineShown);
        Assert.Equal(-1, byline.CBylineIndex);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void ShelfCreate_DraftNotice_ShowsTheSourceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CShelf shelf = CShelf.CShelfCreate(
            atelier,
            static () => true,
            TInterfaceConduct.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        shelf.CShelfVistaRestore();
        shelf.CShelfReferenceCreate();
        CImprint imprint = shelf.CShelfImprint;
        imprint.CImprintTitleSet("Book");
        imprint.CImprintDesk.CDeskPersist();
        List<string> shown = [];
        imprint.CImprintReferenceChanged += reference => shown.Add(reference.CReferenceTitle);
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, imprint.CImprintDesk.CDeskId);

        Assert.Equal(1, marshalled);
        Assert.Equal(["Book"], shown);
    }
}
