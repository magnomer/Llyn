using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflex
{
    [Fact]
    public void EntrySave_FourRows_ReadsBackInOrderWithMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱(농)", meaning: "희롱할"),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹", romanization: "nòng"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう", true),
            ]));

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(["Korean", "Mandarin", "Japanese", "Japanese"], read.Select(row => row.LReflexLanguage));
        Assert.Equal(["", "", "Go-on", "Kan-on"], read.Select(row => row.LReflexKind));
        Assert.Equal(["희롱할", "", "", ""], read.Select(row => row.LReflexMeaning));
        Assert.Equal(["", "nòng", "", ""], read.Select(row => row.LReflexRomanization));
        Assert.Equal([false, false, false, true], read.Select(row => row.LReflexMain));
        Assert.Equal([0, 1, 2, 3], read.Select(row => row.LReflexPosition));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal(
            ["롱(농)", "nʊŋ⁵¹", "る", "ろう"],
            loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftText));
        Assert.Equal(
            ["희롱할", "", "", ""],
            loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftMeaning));
        Assert.Equal(
            ["", "nòng", "", ""],
            loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftRomanization));
        Assert.All(loaded.LEntryDraftReflexes, row => Assert.True(row.LReflexDraftId > 0));
    }

    [Fact]
    public void EntryUpdate_SwappedRows_KeepsEveryId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう"),
            ]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        long first = loaded.LEntryDraftReflexes[0].LReflexDraftId;
        long second = loaded.LEntryDraftReflexes[1].LReflexDraftId;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftReflexes =
            [
                loaded.LEntryDraftReflexes[1] with { LReflexDraftMain = true },
                loaded.LEntryDraftReflexes[0],
            ],
        });

        Assert.Equal(
            [(second, "Kan-on", true), (first, "Go-on", false)],
            engine.TEngineReflexRead(entry.LEntryId).Select(row => (row.LReflexId, row.LReflexKind, row.LReflexMain)));
    }

    [Fact]
    public void EntryUpdate_BlankRow_DropsOnlyThatRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱"),
                TInterface.TReflexDraftCreate("", "", " "),
                TInterface.TReflexDraftCreate("Japanese", "", ""),
            ]));

        Assert.Equal(
            ["Korean", "Japanese"],
            engine.TEngineReflexRead(entry.LEntryId).Select(row => row.LReflexLanguage));
    }

    [Fact]
    public void EntryUpdate_StaleReflexId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate([]));

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            TReflexDraftCreate([TInterface.TReflexDraftCreate("Korean", "", "롱", false, 99)])));

        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_ReflexAddition_MintsRowAndTakesEveryField()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Japanese", "Go-on", 0));
        LReflexDraft added = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.True(added.LReflexDraftId < 0);
        Assert.Equal(("Japanese", "Go-on"), (added.LReflexDraftLanguage, added.LReflexDraftKind));

        engine.TEngineRequestApply(TInterface.TReflexTextCreate(started.LDraftId, added.LReflexDraftId, "る"));
        engine.TEngineRequestApply(TInterface.TReflexKindCreate(started.LDraftId, added.LReflexDraftId, "Kan-on"));
        answered = engine.TEngineRequestApply(
            TInterface.TReflexMainCreate(started.LDraftId, added.LReflexDraftId, true));

        LReflexDraft filled = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.Equal(
            ("Kan-on", "る", true),
            (filled.LReflexDraftKind, filled.LReflexDraftText, filled.LReflexDraftMain));
    }

    [Fact]
    public void RequestApply_ReflexRemoval_KeepsOnlyTheRowsNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Korean", "", 0));
        answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Japanese", "", 1));
        long korean = answered.LDraftContent.LEntryDraftReflexes[0].LReflexDraftId;
        long japanese = answered.LDraftContent.LEntryDraftReflexes[1].LReflexDraftId;

        answered = engine.TEngineRequestApply(TInterface.TReflexRemovalCreate(started.LDraftId, korean));

        Assert.Equal(japanese, Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftId);
    }

    [Fact]
    public void DraftCheck_RowWithLanguageOnly_IsAChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineRequestApply(TInterface.TReflexAdditionCreate(started.LDraftId, "Korean", "", 0));

        Assert.True(engine.TEngineDraftCheck(started.LDraftId));
    }

    [Fact]
    public void MarkupFormat_ReflexRows_RoundTripsWithMark()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>弄</headword>
                <language>Classical Chinese</language>
                <reflex><language>Korean</language><text>롱(농)</text><meaning>희롱할</meaning></reflex>
                <reflex><language>Japanese</language><kind>Kan-on</kind><text>ろう</text><main /></reflex>
              </entry>
            </llyn>
            """;

        IReadOnlyList<LMarkupEntry> parsed = TInterface.TMarkupParse(
            text, out IReadOnlyList<LMarkupOmission> omissions);
        string written = TInterface.TMarkupFormat(parsed);
        IReadOnlyList<LMarkupEntry> again = TInterface.TMarkupParse(written);

        Assert.Empty(omissions);
        Assert.Equal(parsed, again);
        LMarkupEntry entry = Assert.Single(again);
        Assert.Equal(
            [("Korean", "", "롱(농)", "희롱할", false), ("Japanese", "Kan-on", "ろう", "", true)],
            entry.LMarkupEntryReflex.Select(row =>
                (row.LReflexDraftLanguage,
                 row.LReflexDraftKind,
                 row.LReflexDraftText,
                 row.LReflexDraftMeaning,
                 row.LReflexDraftMain)));
        Assert.Contains("<main />", written);
    }

    [Fact]
    public void EntrySave_RegionAndNote_ReadBackOnEveryPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate(
                    "Wu", "", "oʔ⁵⁵", romanization: "7oq", note: "literary", region: "Shanghai"),
            ]));

        LReflex stored = Assert.Single(engine.TEngineReflexRead(entry.LEntryId));
        Assert.Equal(("Shanghai", "literary"), (stored.LReflexRegion, stored.LReflexNote));
        LReflexDraft loaded = Assert.Single(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftReflexes);
        Assert.Equal(("Shanghai", "literary"), (loaded.LReflexDraftRegion, loaded.LReflexDraftNote));
    }

    [Fact]
    public void RequestApply_OnlyReflexMeaning_MarksTheRowAsOwned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Wu", "", 0));
        long row = Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftId;
        Assert.False(Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftOwned);

        LRequest[] other =
        [
            TInterface.TReflexLanguageCreate(started.LDraftId, row, "Wu"),
            TInterface.TReflexKindCreate(started.LDraftId, row, "colloquial"),
            TInterface.TReflexTextCreate(started.LDraftId, row, "oʔ⁵⁵"),
            TInterface.TReflexRespellingCreate(started.LDraftId, row, "oʔ⁵⁵"),
            TInterface.TReflexRomanizationCreate(started.LDraftId, row, "7oq"),
            TInterface.TReflexNoteCreate(started.LDraftId, row, "vernacular"),
            TInterface.TReflexMainCreate(started.LDraftId, row, true),
            TInterface.TReflexAnchorCreate(started.LDraftId, row, 42, true),
        ];
        foreach (LRequest request in other)
        {
            answered = engine.TEngineRequestApply(request);
            Assert.False(Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftOwned);
        }

        answered = engine.TEngineRequestApply(
            TInterface.TReflexMeaningCreate(started.LDraftId, row, "hostile"));

        LReflexDraft filled = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.Equal(("vernacular", "hostile"), (filled.LReflexDraftNote, filled.LReflexDraftMeaning));
        Assert.True(filled.LReflexDraftOwned);
        Assert.False(filled.LReflexDraftEmpty);
    }

    [Fact]
    public void MarkupFormat_ReflexRegionAndGloss_RoundTrip()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>惡</headword>
                <language>Classical Chinese</language>
                <reflex>
                  <language>Wu</language><text>oʔ⁵⁵</text><romanization>7oq</romanization>
                  <meaning>hostile</meaning><owned /><note>literary</note><region>Shanghai</region>
                </reflex>
              </entry>
            </llyn>
            """;

        IReadOnlyList<LMarkupEntry> parsed = TInterface.TMarkupParse(
            text, out IReadOnlyList<LMarkupOmission> omissions);
        string written = TInterface.TMarkupFormat(parsed);
        IReadOnlyList<LMarkupEntry> again = TInterface.TMarkupParse(written);

        Assert.Empty(omissions);
        Assert.Equal(parsed, again);
        LReflexDraft reflex = Assert.Single(Assert.Single(again).LMarkupEntryReflex);
        Assert.Equal(
            ("Shanghai", "literary", "hostile"),
            (reflex.LReflexDraftRegion, reflex.LReflexDraftNote, reflex.LReflexDraftMeaning));
        Assert.True(reflex.LReflexDraftOwned);
        Assert.Contains("<owned />", written);
        Assert.Contains("<region>Shanghai</region>", written);
    }

    private static LEntryDraft TReflexDraftCreate(IReadOnlyList<LReflexDraft> reflexes)
    {
        return TInterface.TEntryDraftCreate(
            "弄",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
    }
}
