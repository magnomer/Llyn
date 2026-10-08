using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalog
{
    [Fact]
    public void CatalogAidRead_PulmonicChart_KeysEveryHeaderAndSplitsEveryCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CArticulation chart = atelier.CAtelierCatalog.CCatalogAidRead().CArticulationAidConsonant;

        Assert.Equal(11, chart.CArticulationHeaders.Count);
        Assert.Equal("Articulation.Bilabial", chart.CArticulationHeaders[0]);
        Assert.Equal("Articulation.Glottal", chart.CArticulationHeaders[^1]);
        Assert.Equal(8, chart.CArticulationSides.Count);
        Assert.Equal("Articulation.Plosive", chart.CArticulationSides[0]);
        Assert.Equal("Articulation.LateralApproximant", chart.CArticulationSides[^1]);
        Assert.Equal(8, chart.CArticulationCells.Count);
        Assert.All(chart.CArticulationCells, row => Assert.Equal(11, row.Count));
        Assert.Equal(["p", "b"], chart.CArticulationCells[0][0]);
        Assert.Empty(chart.CArticulationCells[0][1]);
        Assert.Equal(["ʔ"], chart.CArticulationCells[0][10]);
        Assert.Equal(["h", "ɦ"], chart.CArticulationCells[4][10]);
        Assert.Equal(["ʟ"], chart.CArticulationCells[7][7]);
    }

    [Fact]
    public void CatalogAidRead_TrapezoidChart_KeysEveryHeaderAndSplitsEveryCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CArticulation chart = atelier.CAtelierCatalog.CCatalogAidRead().CArticulationAidVowel;

        Assert.Equal(["Articulation.Front", "Articulation.Central", "Articulation.Back"], chart.CArticulationHeaders);
        Assert.Equal(7, chart.CArticulationSides.Count);
        Assert.Equal("Articulation.Close", chart.CArticulationSides[0]);
        Assert.Equal("Articulation.NearClose", chart.CArticulationSides[1]);
        Assert.Equal("Articulation.Open", chart.CArticulationSides[^1]);
        Assert.Equal(7, chart.CArticulationCells.Count);
        Assert.All(chart.CArticulationCells, row => Assert.Equal(3, row.Count));
        Assert.Equal(["i", "y"], chart.CArticulationCells[0][0]);
        Assert.Equal(["ɯ", "u"], chart.CArticulationCells[0][2]);
        Assert.Empty(chart.CArticulationCells[1][1]);
        Assert.Equal(["ə"], chart.CArticulationCells[3][1]);
        Assert.Equal(["ɑ", "ɒ"], chart.CArticulationCells[6][2]);
    }

    [Fact]
    public void CatalogArticulationRead_EmptyChart_AnswersAnEmptyGrid()
    {
        CArticulation chart = TInterfaceConductPanel.TCatalogArticulationRead([], [], []);

        Assert.Empty(chart.CArticulationHeaders);
        Assert.Empty(chart.CArticulationSides);
        Assert.Empty(chart.CArticulationCells);
    }

    [Fact]
    public void CatalogArticulationRead_FewerRowsThanSides_PadsEachSide()
    {
        CArticulation chart = TInterfaceConductPanel.TCatalogArticulationRead(
            ["Front", "Back"], ["Close", "Mid", "Open"], [[["i"], ["u"]]]);

        Assert.Equal(["Articulation.Close", "Articulation.Mid", "Articulation.Open"], chart.CArticulationSides);
        Assert.Equal(chart.CArticulationSides.Count, chart.CArticulationCells.Count);
        Assert.Equal(["i"], chart.CArticulationCells[0][0]);
        Assert.Equal(["u"], chart.CArticulationCells[0][1]);
        Assert.Empty(chart.CArticulationCells[1]);
        Assert.Empty(chart.CArticulationCells[2]);
    }

    [Fact]
    public void CatalogArticulationRead_RowsWiderThanGrid_CutsEachRow()
    {
        CArticulation chart = TInterfaceConductPanel.TCatalogArticulationRead(
            ["Front", "Back"],
            ["Close"],
            [[["i"], ["u"], ["x"], ["y"]], [["e"], ["o"], ["z"]], [["a"]]]);

        Assert.Equal(["Articulation.Front", "Articulation.Back"], chart.CArticulationHeaders);
        IReadOnlyList<IReadOnlyList<string>> row = Assert.Single(chart.CArticulationCells);
        Assert.Equal(2, row.Count);
        Assert.Equal(["i"], row[0]);
        Assert.Equal(["u"], row[1]);
        Assert.All(
            TInterfaceConductPanel.TCatalogArticulationRead([], ["Close", "Open"], [[["i"]]])
                .CArticulationCells,
            static cells => Assert.Empty(cells));
    }

    [Fact]
    public async Task CatalogEnsignLoad_PendingFill_ReadsTheRowsOnlyAfterTheFill()
    {
        TaskCompletionSource<IReadOnlyList<string>> fill = new();
        List<CEnsignRow> stored = [];
        int read = 0;
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineEnsignLoad"] = args =>
            {
                Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store =
                    (Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action>)args![0]!;
                store([TInterface.TEnsignRowCreate("French", "C:/flags/fr.svg")], static (_, _) => { })();
                return fill.Task;
            },
        });

        Task<CEnsignSheet<int>> sheet = TInterfaceEnsign.TCatalogEnsignLoad(
            TEnvoyFake.TEnvoyCreate(false, []),
            settings,
            "Tag.LoadFailed",
            (rows, _) =>
            {
                stored.AddRange(rows);
                return static () => { };
            },
            () => ++read);
        int before = read;
        fill.SetResult(["English", "French"]);
        CEnsignSheet<int> answered = await sheet;

        Assert.Equal(0, before);
        Assert.Equal(1, read);
        Assert.Equal(["English", "French"], answered.CEnsignSheetLanguages);
        Assert.Equal(1, answered.CEnsignSheetRows);
        Assert.Equal([new CEnsignRow("French", "C:/flags/fr.svg")], stored);
    }

    [Fact]
    public async Task CatalogEnsignLoad_FailedFill_ShowsTheFailureAndAnswersNoLanguages()
    {
        List<string> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineEnsignLoad"] = _ =>
                Task.FromException<IReadOnlyList<string>>(new IOException("disk full")),
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
        });

        CEnsignSheet<string> answered = await TInterfaceEnsign.TCatalogEnsignLoad(
            TEnvoyFake.TEnvoyCreate(false, asked),
            settings,
            "Tag.LoadFailed",
            static (_, _) => static () => { },
            static () => "rows");

        Assert.Equal(["Tag.LoadFailed"], asked);
        Assert.Empty(answered.CEnsignSheetLanguages);
        Assert.Equal("rows", answered.CEnsignSheetRows);
    }

    [Fact]
    public async Task CatalogEnsignLoad_ThrownFill_ShowsTheFailureAndAnswersNoLanguages()
    {
        List<string> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineEnsignLoad"] = _ => throw new InvalidOperationException("broken"),
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
        });

        CEnsignSheet<string> answered = await TInterfaceEnsign.TCatalogEnsignLoad(
            TEnvoyFake.TEnvoyCreate(false, asked),
            settings,
            "Register.LoadFailed",
            static (_, _) => static () => { },
            static () => "rows");

        Assert.Equal(["Register.LoadFailed"], asked);
        Assert.Empty(answered.CEnsignSheetLanguages);
        Assert.Equal("rows", answered.CEnsignSheetRows);
    }

    [Fact]
    public async Task CatalogEnsignLoad_Workspace_AnswersTheCatalogLanguages()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        IReadOnlyList<string> languages =
            await atelier.CAtelierCatalog.CCatalogEnsignLoad(
                TEnvoyFake.TEnvoyCreate(false, []), static (_, _) => static () => { });

        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), languages);
    }

    [Fact]
    public async Task CatalogEnsignLoad_FlagsLoaded_StoresTheRowsAndAnswersTheLanguages()
    {
        List<CEnsignRow> stored = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineEnsignLoad"] = args =>
                {
                    Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store =
                        (Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action>)args![0]!;
                    store([TInterface.TEnsignRowCreate("French", "C:/flags/fr.svg")], static (_, _) => { })();
                    return Task.FromResult<IReadOnlyList<string>>(["English", "French"]);
                },
            }));

        IReadOnlyList<string> languages = await atelier.CAtelierCatalog.CCatalogEnsignLoad(
            TEnvoyFake.TEnvoyCreate(false, []),
            (rows, _) =>
            {
                stored.AddRange(rows);
                return static () => { };
            });

        Assert.Equal(["English", "French"], languages);
        Assert.Equal([new CEnsignRow("French", "C:/flags/fr.svg")], stored);
    }
}
