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
