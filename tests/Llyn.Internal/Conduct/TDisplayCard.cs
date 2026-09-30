using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayCard
{
    [Fact]
    public void DisplayCardRead_ShownEntry_AnswersReadyCardsAndSections()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry eau = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardCreate("a liquid", 1) with
                {
                    LCardDraftTitle = LStateValue.LStateValueUnknown,
                    LCardDraftTranslation = [eau.LEntryId],
                    LCardDraftTag = TInterface.TTagDraftCreate("literal"),
                },
            ],
            []));
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(water.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        CLeaf leaf = Assert.Single(card.CLecternCardMeanings);
        Assert.Equal(1, leaf.CLeafPosition);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"), leaf.CLeafTitle);
        Assert.Equal(new CStateWording(string.Empty, null, true, null), leaf.CLeafExpression);
        Assert.Equal(new CStateWording("a liquid", null, false, null), leaf.CLeafMeaning);
        CTranslationTarget target = Assert.Single(leaf.CLeafTranslation);
        Assert.Equal((eau.LEntryId, "eau", "French"), (
            target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage));
        CLeafChip tag = Assert.Single(leaf.CLeafTag);
        Assert.Equal(new CStateWording("literal", null, false, null), tag.CLeafChipWording);
        Assert.Equal((CSubject.CSubjectTag, true), (tag.CLeafChipSubject, tag.CLeafChipStored));
        Assert.Empty(card.CLecternCardCollocations);
        Assert.True(card.CLecternCardDefined);
        Assert.False(card.CLecternCardCollocated);
    }

    [Fact]
    public void DisplayCardRead_SentenceRow_AnswersTheReadyLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnknown) with
        {
            LSentenceDraftExample = TInterface.TExampleDraftCreate(
                "the water runs", 0, LStateAnchor.LStateAnchorUnspecified, "English"),
        };
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1) with { LCardDraftSentence = [sentence] }],
            []));
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(water.LEntryId);

        CLeaf leaf = Assert.Single(wing.CWingDisplay.CDisplayArea.CDisplayCardRead().CLecternCardMeanings);

        CLeafLine line = Assert.Single(leaf.CLeafSentence);
        Assert.Equal("(+" + TInterface.TLocalizationTextRead("Display.Unknown") + ")", line.CLeafLineHead);
        Assert.Equal("the water runs", line.CLeafLineText);
        Assert.Equal("English", line.CLeafLineLanguage);
        Assert.Equal(string.Empty, line.CLeafLineCitation);
        Assert.Empty(line.CLeafLineMention);
        Assert.Empty(line.CLeafLineGloss);
    }

    [Fact]
    public void DisplayCardRead_NothingShown_AnswersNoCardsAndNoSections()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        CLecternCard card = wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Empty(card.CLecternCardMeanings);
        Assert.Empty(card.CLecternCardCollocations);
        Assert.False(card.CLecternCardDefined);
        Assert.False(card.CLecternCardCollocated);
    }

    [Fact]
    public void FolioImageRead_RowNobodyLocated_CarriesTheEmptyVerdict()
    {
        IReadOnlyList<CImageDraft> images = TInterfaceConduct.TCardImageRead(
            [TInterface.TImageDraftCreate(string.Empty, 3), TInterface.TImageDraftCreate("still.png", 4)],
            TInterfaceConduct.TMediaCreate());
        IReadOnlyList<CVideoDraft> videos = TInterfaceConduct.TCardVideoRead(
            [TInterface.TVideoDraftCreate(string.Empty, id: 5), TInterface.TVideoDraftCreate("clip.mp4", "0:01", 6)],
            TInterfaceConduct.TMediaCreate());

        Assert.Equal([true, false], images.Select(static image => image.CImageDraftEmpty));
        Assert.Equal([true, false], videos.Select(static video => video.CVideoDraftEmpty));
    }

    [Fact]
    public void FolioImageRead_LocatedRow_CarriesTheAddressTheEngineResolved()
    {
        Uri still = new("https://example.org/still.png");
        List<string?> asked = [];
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLocationRead"] = args =>
            {
                asked.Add((string?)args![0]);
                return (string?)args[0] == "still.png" ? still : null;
            },
        });

        IReadOnlyList<CImageDraft> images = TInterfaceConduct.TCardImageRead(
            [TInterface.TImageDraftCreate(string.Empty, 3), TInterface.TImageDraftCreate("still.png", 4)], media);

        Assert.Equal([string.Empty, "still.png"], asked);
        Assert.Equal([null, still], images.Select(static image => image.CImageDraftAddress));
    }

    [Fact]
    public void FolioVideoRead_LocatedRow_CarriesTheScreenTheEngineResolved()
    {
        CScreen reel = new(new Uri("https://example.org/reel.mp4"), null);
        List<string?> asked = [];
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineScreenRead"] = args =>
            {
                asked.Add((string?)args![0]);
                return (string?)args[0] == "reel.mp4" ? (reel.CScreenAddress, reel.CScreenFilm) : null;
            },
        });

        IReadOnlyList<CVideoDraft> videos = TInterfaceConduct.TCardVideoRead(
            [TInterface.TVideoDraftCreate(string.Empty, id: 5), TInterface.TVideoDraftCreate("reel.mp4", "0:01", 6)],
            media);

        Assert.Equal([string.Empty, "reel.mp4"], asked);
        Assert.Equal([null, reel], videos.Select(static video => video.CVideoDraftScreen));
    }

    [Fact]
    public void FolioVideoRead_WrittenSpan_CarriesTheMomentsTheEngineRead()
    {
        IReadOnlyList<CVideoDraft> videos = TInterfaceConduct.TCardVideoRead(
            [TInterface.TVideoDraftCreate("reel.mp4", "0:10 - 0:40"), TInterface.TVideoDraftCreate("reel.mp4")],
            TInterfaceConduct.TMediaCreate());

        Assert.Equal(
            [(TimeSpan.FromSeconds(10), (TimeSpan?)TimeSpan.FromSeconds(40)), (TimeSpan.Zero, null)],
            videos.Select(static video => (video.CVideoDraftFrom, video.CVideoDraftUntil)));
    }

    [Fact]
    public void FolioVideoRead_HostedOrOtherLocation_CarriesAddressAndEngineFilmId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<CVideoDraft> videos = TInterfaceConduct.TCardVideoRead(
            [
                TInterface.TVideoDraftCreate(" https://youtu.be/dQw4w9WgXcQ "),
                TInterface.TVideoDraftCreate("https://example.com/reel.mp4"),
                TInterface.TVideoDraftCreate("media/dQw4w9WgXcQ.mp4"),
                TInterface.TVideoDraftCreate(string.Empty),
            ],
            TInterfaceConduct.TMediaCreate(engine));

        Assert.Equal(new CScreen(new Uri("https://youtu.be/dQw4w9WgXcQ"), "dQw4w9WgXcQ"), videos[0].CVideoDraftScreen);
        Assert.Equal(new CScreen(new Uri("https://example.com/reel.mp4"), null), videos[1].CVideoDraftScreen);
        Assert.Null(videos[2].CVideoDraftScreen);
        Assert.Null(videos[3].CVideoDraftScreen);
    }

    [Fact]
    public void DisplayIncomingRead_NothingChosen_AnswersNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        Assert.Empty(wing.CWingDisplay.CDisplayArea.CDisplayIncomingRead());
    }

    [Fact]
    public void DisplayEtymologyRead_SourceLink_ShowsTheFieldWithItsNamedLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry rinnan = TDisplayEntrySave(engine, "rinnan");
        LEntry run = TDisplayEntrySave(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", run.LEntryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, rinnan.LEntryId, 0));
        engine.TEngineDraftCommit(held.LDraftId);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.Equal("English", etymology.CLecternEtymologyLanguage);
        Assert.Equal(rinnan.LEntryId, Assert.Single(etymology.CLecternEtymologyTargets).CTranslationTargetId);
        Assert.True(etymology.CLecternEtymologyShown);
        Assert.True(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayEtymologyRead_Narrative_ShowsTheFieldWithItsText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", run.LEntryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineDraftCommit(held.LDraftId);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.Equal("From rinnan.", etymology.CLecternEtymologyText);
        Assert.Empty(etymology.CLecternEtymologyTargets);
        Assert.True(etymology.CLecternEtymologyShown);
        Assert.True(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayEtymologyRead_NoEtymology_HidesTheFieldAndTheSection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.False(etymology.CLecternEtymologyShown);
        Assert.False(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayChipOpen_StoredRecord_RaisesTheTabItsKindPicks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplay area = TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplayArea;
        List<string> opened = [];

        bool situation = TDisplayChipOpen(area, TDisplayChipCreate(7, CSubject.CSubjectSituation, true), null, opened);
        bool register = TDisplayChipOpen(area, TDisplayChipCreate(8, CSubject.CSubjectRegister, true), null, opened);
        bool tag = TDisplayChipOpen(area, TDisplayChipCreate(9, CSubject.CSubjectTag, true), null, opened);
        bool link = TDisplayChipOpen(area, null, 12, opened);

        Assert.Equal((true, true, true, true), (situation, register, tag, link));
        Assert.Equal(["Repertoire 7", "Tenor 8", "Taxonomy 9", "Library 12"], opened);
    }

    [Fact]
    public void DisplayChipOpen_UnsavedRecord_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplay area = TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplayArea;
        List<string> opened = [];

        bool situation = TDisplayChipOpen(area, TDisplayChipCreate(0, CSubject.CSubjectSituation, false), null, opened);
        bool link = TDisplayChipOpen(area, null, 0, opened);
        bool none = TDisplayChipOpen(area, null, null, opened);

        Assert.Equal((false, false, false), (situation, link, none));
        Assert.Empty(opened);
    }

    [Fact]
    public void DisplayCardFind_ShownCards_AnswersTheListAndThePlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1), TInterface.TCardCreate("a sea", 2)],
            [TInterface.TCardCreate("still water", 1)]));
        CWing wing = TDisplayWingPrepare(atelier, []);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        Assert.Null(area.CDisplayCardFind(1));
        wing.CWingEntryOpen(water.LEntryId);
        LEntryDraft shown = wing.CWingDisplay.LDisplaySound.LDisplayShown!;

        (CCompassPart, int)? sea = area.CDisplayCardFind(shown.LEntryDraftMeanings[1].LCardDraftId);
        (CCompassPart, int)? still = area.CDisplayCardFind(shown.LEntryDraftCollocations[0].LCardDraftId);

        Assert.Equal((CCompassPart.CCompassPartMeaning, 1), sea);
        Assert.Equal((CCompassPart.CCompassPartCollocation, 0), still);
        Assert.Null(area.CDisplayCardFind(987654));
    }

    private static CLeafChip TDisplayChipCreate(long id, CSubject subject, bool stored) =>
        new(id, new CStateWording("a chip", null, false, null), subject, stored);

    private static bool TDisplayChipOpen(CDisplay area, CLeafChip? chip, long? link, List<string> opened)
    {
        Action<string, long> chosen = (tab, id) => opened.Add(tab + " " + id);
        area.CDisplayRowChosen += chosen;
        bool asked = area.CDisplayChipOpen(chip, link);
        area.CDisplayRowChosen -= chosen;
        return asked;
    }

    private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        return wing;
    }

    private static LEntry TDisplayEntrySave(LEngine engine, string headword) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
}
