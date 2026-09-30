using System;
using System.Collections.Generic;
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
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFontRead"] = args =>
                {
                    asked.Add(((string)args![0]!, (LFontRole)args[1]!));
                    return TInterfaceConduct.TFontCreate("Noto Serif", 21);
                },
            }));

        CFont font = atelier.CAtelierCatalog.CCatalogFontRead("Korean", CFontRole.CFontRoleGlyph);

        Assert.Equal(new CFont("Noto Serif", 21, null), font);
        Assert.Equal([("Korean", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void CatalogFontRead_UnsizedFont_CarriesNoSize()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFontRead"] = _ => TInterfaceConduct.TFontCreate("Noto Serif", 0),
            }));

        CFont font = atelier.CAtelierCatalog.CCatalogFontRead("Korean", CFontRole.CFontRoleHeadword);

        Assert.Equal("Noto Serif", font.CFontFamily);
        Assert.Null(font.CFontSize);
    }

    [Fact]
    public void CatalogFontRead_BlankLanguage_AnswersNothingSetWithoutAsking()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFontRead"] = args =>
                {
                    asked.Add((string)args![0]!);
                    return TInterfaceConduct.TFontCreate("Noto Serif", 21);
                },
            }));

        CFont font = atelier.CAtelierCatalog.CCatalogFontRead(" ", CFontRole.CFontRoleHeadword);

        Assert.Equal(new CFont(null, null, null), font);
        Assert.Empty(asked);
    }

    [Fact]
    public void CatalogFontRead_RefusedRead_AnswersNothingSet()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFontRead"] = _ => throw new InvalidOperationException("no pack"),
            }));

        CFont font = atelier.CAtelierCatalog.CCatalogFontRead("Korean", CFontRole.CFontRoleExample);

        Assert.Equal(new CFont(null, null, null), font);
    }

    [Fact]
    public void CatalogFontRead_TwoRoles_AnswersEachInTheGivenOrder()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFontRead"] = args => (LFontRole)args![1]! == LFontRole.LFontRoleGloss
                    ? TInterfaceConduct.TFontCreate("Gloss Sans", 12)
                    : TInterfaceConduct.TFontCreate("Example Serif", 18),
            }));

        IReadOnlyList<CFont> fonts = atelier.CAtelierCatalog.CCatalogFontRead(
            "Korean", [CFontRole.CFontRoleExample, CFontRole.CFontRoleGloss]);

        Assert.Equal(["Example Serif", "Gloss Sans"], fonts.Select(font => font.CFontFamily));
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
