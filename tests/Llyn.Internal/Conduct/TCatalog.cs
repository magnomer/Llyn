using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalog
{
    [Fact]
    public void CatalogMeaningRead_ReadyRows_MapsEachRowAndChoosesTheFallbackKey()
    {
        List<string> keys = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineMeaningRead"] = args =>
                {
                    keys.Add((string)args![1]!);
                    return new List<(long LMeaningId, string LMeaningName, int LMeaningDepth)>
                    {
                        (1, "first", 0),
                        (2, "first-a", 1),
                        (3, "second", 0),
                    };
                },
            });
        List<string> asked = [];

        IReadOnlyList<CMeaning>? rows =
            atelier.CAtelierCatalog.CCatalogMeaningRead(7, TInterfaceConduct.TEnvoyCreate(false, asked));

        Assert.Equal(
            [(1L, "first", 0), (2L, "first-a", 1), (3L, "second", 0)],
            rows!.Select(static row => (row.CMeaningId, row.CMeaningName, row.CMeaningDepth)).ToList());
        Assert.Equal(["Display.Unknown"], keys);
        Assert.Empty(asked);
    }

    [Fact]
    public void CatalogMeaningRead_ReadFails_ShowsTheFailureAndAnswersNothing()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineMeaningRead"] = _ => throw new InvalidOperationException("gone"),
            });
        List<string> asked = [];

        IReadOnlyList<CMeaning>? rows =
            atelier.CAtelierCatalog.CCatalogMeaningRead(7, TInterfaceConduct.TEnvoyCreate(false, asked));

        Assert.Null(rows);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    [Fact]
    public void CatalogEntryLoad_StoredEntry_CountsMeaningAndCollocationCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "tally", "English", string.Empty, string.Empty,
            [TInterface.TCardCreate("one", 1), TInterface.TCardCreate("two", 2)],
            [TInterface.TCardCreate("three", 1)]));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal((2, 1), atelier.CAtelierCatalog.CCatalogEntryLoad(entry.LEntryId));
    }

    [Fact]
    public void CatalogEntryLoad_MissingEntry_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Null(atelier.CAtelierCatalog.CCatalogEntryLoad(987654));
    }

    [Fact]
    public void CatalogSpeechAdd_EngineDeclines_ReadsNothing()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineSpeechAdd"] = _ => null,
            });

        Assert.Null(atelier.CAtelierCatalog.CCatalogSpeechAdd("English", "Noun"));
    }

    [Fact]
    public void CatalogSpeechRead_Values_ReadsIdAndName()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineSpeechRead"] = _ => new List<LSpeechValue> { new(5, "English", 1, "Noun", 1) },
            });

        Assert.Equal([new CSpeechValue(5, "Noun")], atelier.CAtelierCatalog.CCatalogSpeechRead("English"));
    }

    [Fact]
    public void CatalogMarkupFind_Matches_ReadsEntryIds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "found", "English", string.Empty, string.Empty, [TInterface.TCardCreate("one", 1)], []));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal([entry.LEntryId], atelier.CAtelierCatalog.CCatalogMarkupFind("found", "English"));
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
