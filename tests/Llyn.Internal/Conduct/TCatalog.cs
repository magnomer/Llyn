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
    public void CatalogMeaningSort_NestedMeanings_ReadsDepthFirstByPosition()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineMeaningRead"] = _ => new List<LMeaning>
                {
                    TInterface.TMeaningCreate(3, 7, null, 2, TInterface.TStateValueCreate("second"), blank),
                    TInterface.TMeaningCreate(1, 7, null, 1, TInterface.TStateValueCreate("first"), blank),
                    TInterface.TMeaningCreate(4, 7, 1, 2, TInterface.TStateValueCreate("first-b"), blank),
                    TInterface.TMeaningCreate(2, 7, 1, 1, TInterface.TStateValueCreate("first-a"), blank),
                },
            });

        IReadOnlyList<CMeaning> rows = atelier.CAtelierCatalog.CCatalogMeaningSort(7);

        Assert.Equal(
            [(1L, "first", 0), (2L, "first-a", 1), (4L, "first-b", 1), (3L, "second", 0)],
            rows.Select(static row => (row.CMeaningId, row.CMeaningName, row.CMeaningDepth)).ToList());
    }

    [Fact]
    public void CatalogMeaningSort_BlankName_ReadsTheUnknownText()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineMeaningRead"] = _ => new List<LMeaning>
                {
                    TInterface.TMeaningCreate(1, 7, null, 1, blank, blank),
                },
            });

        IReadOnlyList<CMeaning> rows = atelier.CAtelierCatalog.CCatalogMeaningSort(7);

        Assert.Equal(engine.TEngineTextRead("Display.Unknown"), Assert.Single(rows).CMeaningName);
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
    public void CatalogAnchorMatch_SameAndOtherAnchors_MatchOnlyTheSame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        Assert.True(atelier.CAtelierCatalog.CCatalogAnchorMatch([1, 2], [1, 2]));
        Assert.False(atelier.CAtelierCatalog.CCatalogAnchorMatch([1, 2], [3]));
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
