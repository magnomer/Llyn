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
    public void CatalogConsonantRead_PulmonicChart_KeysEveryHeaderAndSplitsEveryCell()
    {
        CArticulation chart = CCatalog.CCatalogConsonantRead();

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
    public void CatalogVowelRead_TrapezoidChart_KeysEveryHeaderAndSplitsEveryCell()
    {
        CArticulation chart = CCatalog.CCatalogVowelRead();

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
    public void CatalogFontRead_LanguageAndRole_ReadsThePortFont()
    {
        List<(string, LFontRole)> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add(((string)args![0]!, (LFontRole)args[1]!));
                return TInterfaceFont.TFontCreate("Noto Serif", 21);
            },
        });

        CFont font = TInterfaceFont.TCatalogFontRead(settings, "Korean", CFontRole.CFontRoleGlyph);

        Assert.Equal(new CFont("Noto Serif", 21, null), font);
        Assert.Equal([("Korean", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void CatalogFontRead_UnsizedFont_CarriesNoSize()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = _ => TInterfaceFont.TFontCreate("Noto Serif", 0),
        });

        CFont font = TInterfaceFont.TCatalogFontRead(settings, "Korean", CFontRole.CFontRoleHeadword);

        Assert.Equal("Noto Serif", font.CFontFamily);
        Assert.Null(font.CFontSize);
    }

    [Fact]
    public void CatalogFontRead_BlankLanguage_AnswersNothingSetWithoutAsking()
    {
        List<string> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add((string)args![0]!);
                return TInterfaceFont.TFontCreate("Noto Serif", 21);
            },
        });

        CFont font = TInterfaceFont.TCatalogFontRead(settings, " ", CFontRole.CFontRoleHeadword);

        Assert.Equal(new CFont(null, null, null), font);
        Assert.Empty(asked);
    }

    [Fact]
    public void CatalogFontRead_RefusedRead_RaisesTheFailure()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = _ => throw new InvalidOperationException("no pack"),
        });

        InvalidOperationException failure = Assert.Throws<InvalidOperationException>(
            () => TInterfaceFont.TCatalogFontRead(settings, "Korean", CFontRole.CFontRoleExample));

        Assert.Equal("no pack", failure.Message);
    }

    [Fact]
    public void CatalogFontRead_SlantInAnyCaseOrUnknown_ArrivesLowerCaseOrBlank()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args => (LFontRole)args![1]! switch
            {
                LFontRole.LFontRoleGloss => TInterfaceFont.TFontCreate("Gloss Sans", 12, "Italic"),
                LFontRole.LFontRoleExample => TInterfaceFont.TFontCreate("Example Serif", 18, "OBLIQUE"),
                _ => TInterfaceFont.TFontCreate("Head Sans", 40, "slanted"),
            },
        });

        Assert.Equal(
            ["italic", "oblique", null],
            new[] { CFontRole.CFontRoleGloss, CFontRole.CFontRoleExample, CFontRole.CFontRoleHeadword }
                .Select(role => TInterfaceFont.TCatalogFontRead(settings, "Korean", role).CFontStyle));
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
            settings,
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
    public async Task CatalogEnsignLoad_FailedFill_LeavesTheFailureToTheCaller()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineEnsignLoad"] = _ =>
                Task.FromException<IReadOnlyList<string>>(new IOException("disk full")),
            ["LEngineLanguageRead"] = _ => (IReadOnlyList<string>)["Mandarin", "Welsh"],
        });

        IOException failure = await Assert.ThrowsAsync<IOException>(
            () => TInterfaceEnsign.TCatalogEnsignLoad(
                settings, static (_, _) => static () => { }, static () => "rows"));

        Assert.Equal("disk full", failure.Message);
    }

    [Fact]
    public async Task CatalogEnsignLoad_BrokenFill_RaisesTheFailure()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineEnsignLoad"] = _ =>
                Task.FromException<IReadOnlyList<string>>(new InvalidOperationException("broken")),
            ["LEngineLanguageRead"] = _ => (IReadOnlyList<string>)["Mandarin", "Welsh"],
        });

        InvalidOperationException failure = await Assert.ThrowsAsync<InvalidOperationException>(
            () => TInterfaceEnsign.TCatalogEnsignLoad(
                settings, static (_, _) => static () => { }, static () => "rows"));

        Assert.Equal("broken", failure.Message);
    }

    [Fact]
    public async Task CatalogEnsignLoad_Workspace_AnswersTheCatalogLanguages()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        IReadOnlyList<string> languages =
            await atelier.CAtelierCatalog.CCatalogEnsignLoad(static (_, _) => static () => { });

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

        IReadOnlyList<string> languages = await atelier.CAtelierCatalog.CCatalogEnsignLoad((rows, _) =>
        {
            stored.AddRange(rows);
            return static () => { };
        });

        Assert.Equal(["English", "French"], languages);
        Assert.Equal([new CEnsignRow("French", "C:/flags/fr.svg")], stored);
    }

    [Fact]
    public void CatalogFontRead_EveryEngineRole_CastsToTheSameNamedMirror()
    {
        LFontRole[] roles = Enum.GetValues<LFontRole>();

        Assert.Equal(roles.Length, Enum.GetValues<CFontRole>().Length);
        foreach (LFontRole role in roles)
        {
            Assert.Equal(role.ToString()[1..], ((CFontRole)role).ToString()[1..]);
        }
    }
}
