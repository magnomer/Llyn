using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTally
{
    private const string TTallyLanguage = "Classical Chinese";

    [Fact]
    public void TallyRead_InitialOverTwoDivisions_CountsCharactersPerPart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate(
            "林",
            [
                TInterface.TReflexDraftCreate("Korean", "", "림"),
                TInterface.TReflexDraftCreate("Mandarin", "", "lin³⁵", respelling: "lin³⁵"),
                TInterface.TReflexDraftCreate("Xiang", "", "lin¹³", respelling: "lin¹³"),
            ]));
        engine.TEngineEntrySave(TTallyDraftCreate(
            "爛",
            [
                TInterface.TReflexDraftCreate("Korean", "", "란(난)"),
                TInterface.TReflexDraftCreate("Mandarin", "", "län⁵¹", respelling: "lan⁵¹"),
            ]));
        engine.TEngineEntrySave(TTallyDraftCreate(
            "弄",
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱(농)"),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹", respelling: "nuŋ⁵¹"),
                TInterface.TReflexDraftCreate("Mandarin", "", "lʊŋ⁵¹", respelling: "luŋ⁵¹"),
            ]));
        LDiwei lai = TTallyDiweiPlace(
            workspace,
            [("林", "侵", "三"), ("爛", "寒", "一"), ("弄", "送", "一")]);
        TTallyAnchorApply(workspace, engine, "林", "爛", "弄");

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(lai);

        Assert.Equal(["三", "一"], tallies.Select(tally => tally.LTallyHeading));
        Assert.Equal(
            [("Korean", "ㄹ(1)"), ("Mandarin", "l(1)"), ("Xiang", "l(1)")],
            tallies[0].LTallyLines.Select(line => (line.LTallyLineLanguage, TTallyMarkFormat(line.LTallyLineIpa))));
        Assert.Equal(
            [("Korean", "ㄹ(2)"), ("Mandarin", "l(2), n(1)")],
            tallies[1].LTallyLines.Select(line => (line.LTallyLineLanguage, TTallyMarkFormat(line.LTallyLineIpa))));
        Assert.Equal(
            ["ㄹ(2)", "l(2), n(1)"],
            tallies[1].LTallyLines.Select(line => TTallyMarkFormat(line.LTallyLineRespelling)));
        Assert.Equal(["爛", "弄"], tallies[1].LTallyLines[1].LTallyLineIpa[0].LTallyMarkCharacters);
        Assert.Equal(["弄"], tallies[1].LTallyLines[1].LTallyLineIpa[1].LTallyMarkCharacters);
    }

    [Fact]
    public void TallyRead_CharacterWithoutAnchor_LeavesDivisionOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate("林", [TInterface.TReflexDraftCreate("Korean", "", "림")]));
        LDiwei lai = TTallyDiweiPlace(workspace, [("林", "侵", "三"), ("爛", "寒", "一")]);
        TTallyAnchorApply(workspace, engine, "林");

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(lai);

        Assert.Equal(["三"], tallies.Select(tally => tally.LTallyHeading));
        Assert.Single(tallies[0].LTallyLines);
        Assert.Equal(["林"], engine.TEngineFanqieRead(lai).Select(row => row.LFanqieRowCharacter));
    }

    [Fact]
    public void TallyRead_UnanchoredReflex_CountsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate("林", [TInterface.TReflexDraftCreate("Korean", "", "림")]));
        LDiwei lai = TTallyDiweiPlace(workspace, [("林", "侵", "三")]);

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(lai);

        Assert.Empty(tallies);
        Assert.Empty(engine.TEngineFanqieRead(lai));
        Assert.Equal(0, Assert.IsType<LDiwei>(
            engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiInitial, "來")).LDiweiCount);
    }

    [Fact]
    public void TallyRead_RowAnchoredToOnePlacement_CountsOnlyOnThatPage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry wan = engine.TEngineEntrySave(TTallyDraftCreate(
            "完",
            [
                TInterface.TReflexDraftCreate("Korean", "", "환"),
                TInterface.TReflexDraftCreate("Korean", "", "관"),
                TInterface.TReflexDraftCreate("Mandarin", "", "wan³⁵", respelling: "wan³⁵"),
            ]));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(
            TTallyLanguage,
            "完",
            [
                TInterface.TFanqieRowCreate("完", 0, "匣", "寒", "一", "平"),
                TInterface.TFanqieRowCreate("完", 1, "溪", "寒", "一", "平"),
            ]);
        archive.TDiweiApply(TTallyLanguage, "完", null);
        IReadOnlyList<LFanqieRow> rows = fanqie.TFanqieRead(TTallyLanguage, "完");
        IReadOnlyList<LReflex> reflexes = engine.TEngineReflexRead(wan.LEntryId);
        engine.TEngineReflexSet(
            wan.LEntryId,
            [
                reflexes[0] with { LReflexAnchors = [rows[0].LFanqieRowId] },
                reflexes[1] with { LReflexAnchors = [rows[1].LFanqieRowId] },
                reflexes[2] with { LReflexAnchors = [rows[0].LFanqieRowId] },
            ]);
        LDiwei xia = Assert.IsType<LDiwei>(engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiInitial, "匣"));
        LDiwei xi = Assert.IsType<LDiwei>(engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiInitial, "溪"));

        LTally onXia = Assert.Single(engine.TEngineTallyRead(xia));
        LTally onXi = Assert.Single(engine.TEngineTallyRead(xi));

        Assert.Equal(
            [("Korean", "ㅎ(1)"), ("Mandarin", "w(1)")],
            onXia.LTallyLines.Select(line => (line.LTallyLineLanguage, TTallyMarkFormat(line.LTallyLineIpa))));
        Assert.Equal(
            [("Korean", "ㄱ(1)")],
            onXi.LTallyLines.Select(line => (line.LTallyLineLanguage, TTallyMarkFormat(line.LTallyLineIpa))));
        Assert.Equal((1, 1), (xia.LDiweiCount, xi.LDiweiCount));
        Assert.Equal([wan.LEntryId], archive.TDiweiEntryScan(TTallyLanguage, [xi.LDiweiId]));
    }

    [Fact]
    public void TallyRead_RimeKind_CountsVowelAndCodaPerKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate(
            "林",
            [
                TInterface.TReflexDraftCreate("Korean", "", "림"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "りむ"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "りん"),
            ]));
        TTallyDiweiPlace(workspace, [("林", "侵", "三")]);
        TTallyAnchorApply(workspace, engine, "林");
        LDiwei qin = Assert.IsType<LDiwei>(
            engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiRime, "侵 III"));

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(qin);

        LTally single = Assert.Single(tallies);
        Assert.Equal(
            [("Korean", "", "ㅣㅁ(1)"), ("Japanese", "Go-on", "imu(1)"), ("Japanese", "Kan-on", "in(1)")],
            single.LTallyLines.Select(line =>
                (line.LTallyLineLanguage, line.LTallyLineKind, TTallyMarkFormat(line.LTallyLineIpa))));
    }

    [Fact]
    public void TallyRead_RimeKind_SectionsByArticulatoryPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate("林", [TInterface.TReflexDraftCreate("Korean", "", "림")]));
        engine.TEngineEntrySave(TTallyDraftCreate("金", [TInterface.TReflexDraftCreate("Korean", "", "김")]));
        engine.TEngineEntrySave(TTallyDraftCreate("心", [TInterface.TReflexDraftCreate("Korean", "", "심")]));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);
        foreach ((string character, string initial) in new[] { ("林", "來"), ("金", "見"), ("心", "心") })
        {
            fanqie.TFanqieSave(
                TTallyLanguage,
                character,
                [TInterface.TFanqieRowCreate(character, 0, initial, "侵", "三", "平")]);
            archive.TDiweiApply(TTallyLanguage, character, null);
        }

        TTallyAnchorApply(workspace, engine, "林", "金", "心");
        LDiwei qin = Assert.IsType<LDiwei>(
            engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiRime, "侵 III"));

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(qin);

        Assert.Equal(["other", "velar", "dental"], tallies.Select(tally => tally.LTallyHeading));
        Assert.Equal(
            ["ㅣㅁ(1)", "ㅣㅁ(1)", "ㅣㅁ(1)"],
            tallies.Select(tally => TTallyMarkFormat(Assert.Single(tally.LTallyLines).LTallyLineIpa)));
        Assert.Equal(["金"], tallies[1].LTallyLines[0].LTallyLineIpa[0].LTallyMarkCharacters);
    }

    private static LDiwei TTallyDiweiPlace(
        TWorkspace workspace, IReadOnlyList<(string, string, string)> placements)
    {
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);
        foreach ((string character, string rime, string division) in placements)
        {
            fanqie.TFanqieSave(
                TTallyLanguage,
                character,
                [TInterface.TFanqieRowCreate(character, 0, "來", rime, division, "平")]);
            archive.TDiweiApply(TTallyLanguage, character, null);
        }

        return Assert.IsType<LDiwei>(archive.TDiweiFind(TTallyLanguage, LDiwei.LDiweiInitial, "來"));
    }

    private static void TTallyAnchorApply(TWorkspace workspace, LEngine engine, params string[] characters)
    {
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        foreach (string character in characters)
        {
            IReadOnlyList<long> anchors = fanqie.TFanqieRead(TTallyLanguage, character)
                .Select(row => row.LFanqieRowId)
                .ToList();
            foreach (LEntry entry in entries.TEntryHeadwordFind(TTallyLanguage, character))
            {
                engine.TEngineReflexSet(
                    entry.LEntryId,
                    engine.TEngineReflexRead(entry.LEntryId)
                        .Select(reflex => reflex with { LReflexAnchors = anchors })
                        .ToList());
            }
        }
    }

    private static string TTallyMarkFormat(IReadOnlyList<LTallyMark> marks) =>
        string.Join(", ", marks.Select(mark => mark.LTallyMarkText + "(" + mark.LTallyMarkCount + ")"));

    private static LEntryDraft TTallyDraftCreate(string headword, IReadOnlyList<LReflexDraft> reflexes)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            TTallyLanguage,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
    }
}
