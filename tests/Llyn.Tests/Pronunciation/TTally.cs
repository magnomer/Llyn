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

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(lai);

        Assert.Equal(["三", "一"], tallies.Select(tally => tally.LTallyDivision));
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
    public void TallyRead_CharacterWithoutEntry_LeavesDivisionWithoutLines()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TTallyDraftCreate("林", [TInterface.TReflexDraftCreate("Korean", "", "림")]));
        LDiwei lai = TTallyDiweiPlace(workspace, [("林", "侵", "三"), ("爛", "寒", "一")]);

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(lai);

        Assert.Equal(["三", "一"], tallies.Select(tally => tally.LTallyDivision));
        Assert.Single(tallies[0].LTallyLines);
        Assert.Empty(tallies[1].LTallyLines);
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
        LDiwei qin = Assert.IsType<LDiwei>(
            engine.TEngineDiweiFind(TTallyLanguage, LDiwei.LDiweiRime, "侵"));

        IReadOnlyList<LTally> tallies = engine.TEngineTallyRead(qin);

        LTally single = Assert.Single(tallies);
        Assert.Equal(
            [("Korean", "", "ㅣㅁ(1)"), ("Japanese", "Go-on", "imu(1)"), ("Japanese", "Kan-on", "in(1)")],
            single.LTallyLines.Select(line =>
                (line.LTallyLineLanguage, line.LTallyLineKind, TTallyMarkFormat(line.LTallyLineIpa))));
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
