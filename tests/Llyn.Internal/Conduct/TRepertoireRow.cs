using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoireRow
{
    [Fact]
    public void RepertoireRowsRead_UnknownKind_WordsTheKindWithTheUnknownWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "in court", null, "formal"));
        engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, LStateValue.LStateValueUnknown));
        engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "by the shore", null, null));
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        CCatalogSituation[] rows = [.. repertoire.CRepertoireRowsRead("?", "-")];

        Assert.Equal(
            [("at home", "?"), ("by the shore", string.Empty), ("in court", "formal")],
            rows.Select(row => (row.CCatalogSituationTitle, row.CCatalogSituationKind)));
    }

    [Fact]
    public void RepertoireRowsRead_UsedAndUnusedSituations_WordsTheCountBlankWhenUnused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        TRepertoire.TRepertoireSituationSave(engine, "by the shore");
        TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        CCatalogSituation[] rows = [.. repertoire.CRepertoireRowsRead("?", "-")];

        Assert.Equal(
            [("at home", "1"), ("by the shore", string.Empty)],
            rows.Select(row => (row.CCatalogSituationTitle, row.CCatalogSituationCount)));
    }
}
