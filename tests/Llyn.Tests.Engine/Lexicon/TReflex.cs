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

        IReadOnlyList<LReflexDraft> read = engine.TEntryReflexRead(entry.LEntryId);
        Assert.Equal(["Japanese", "Japanese", "Korean", "Mandarin"], read.Select(row => row.LReflexDraftLanguage));
        Assert.Equal(["Go-on", "Kan-on", "", ""], read.Select(row => row.LReflexDraftKind));
        Assert.Equal(["る", "ろう", "롱(농)", "nʊŋ⁵¹"], read.Select(row => row.LReflexDraftText));
        Assert.Equal(["", "", "희롱할", ""], read.Select(row => row.LReflexDraftMeaning));
        Assert.Equal(["", "", "", "nòng"], read.Select(row => row.LReflexDraftRomanization));
        Assert.Equal([false, true, false, false], read.Select(row => row.LReflexDraftMain));
        Assert.All(read, row => Assert.True(row.LReflexDraftId > 0));
        Assert.Equal(
            3, workspace.TWorkspaceCountRead("SELECT MAX(position) FROM reflex;"));
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
            [(first, "Go-on", false), (second, "Kan-on", true)],
            engine.TEntryReflexRead(entry.LEntryId)
                .Select(row => (row.LReflexDraftId, row.LReflexDraftKind, row.LReflexDraftMain)));
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
            ["Japanese", "Korean"],
            engine.TEntryReflexRead(entry.LEntryId).Select(row => row.LReflexDraftLanguage));
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

        LReflexDraft loaded = Assert.Single(engine.TEntryReflexRead(entry.LEntryId));
        Assert.Equal(("Shanghai", "literary"), (loaded.LReflexDraftRegion, loaded.LReflexDraftNote));

        LRig rig = workspace.TWorkspaceRigCreate();
        LReflex stored = Assert.Single(rig.TReflexRead(entry.LEntryId));
        Assert.Equal(("Shanghai", "literary"), (stored.LReflexRegion, stored.LReflexNote));

        LMarkupEntry exported = Assert.IsType<LMarkupEntry>(
            TInterface.TMarkupExportCreate(rig).TMarkupClerkLoad(entry.LEntryId));
        LReflexDraft marked = Assert.Single(exported.LMarkupEntryReflex);
        Assert.Equal(("Shanghai", "literary"), (marked.LReflexDraftRegion, marked.LReflexDraftNote));

        LPortraitPage page = TInterface.TPortraitClerkCreate(rig)
            .TPortraitClerkRead(entry.LEntryId, TInterface.TPortraitLabelRead());
        LPortraitLine shown = Assert.Single(page.LPortraitPageLine);
        Assert.Contains("Shanghai", shown.LPortraitLineLabel);
        Assert.Contains("literary", shown.LPortraitLineText);
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

    [Fact]
    public void ReflexFind_DraftRows_FindsOnlyTheRowTheIdNames()
    {
        LEntryDraft content = TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱", id: 4),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る", id: 9),
            ]);

        Assert.Equal("る", TInterface.TReflexFind(content, 9)?.LReflexDraftText);
        Assert.Null(TInterface.TReflexFind(content, 5));
        Assert.Throws<LRefusal>(() => TInterface.TReflexFind(content, 0));
    }

    private const string TReflexOrderPack =
        """
        { "order": { "language": ["Korean", "Japanese", "Mandarin"], "kind": { "Japanese": ["Kan-on", "Go-on"] } } }
        """;

    [Fact]
    public void ReflexOrderSort_UndeclaredLanguageAndKind_FollowDeclaredByName()
    {
        LReflexOrder order = TInterface.TReflexOrderCreate(
            ["Mandarin", "Japanese"],
            new Dictionary<string, IReadOnlyList<string>> { ["Japanese"] = ["Kan-on"] });

        IReadOnlyList<LReflexDraft> sorted = order.TReflexOrderSort(
            [
                TInterface.TReflexDraftCreate("Wu", "", "ŋu"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Cantonese", "", "lʊŋ⁶"),
                TInterface.TReflexDraftCreate("Japanese", "", "ろ"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ら"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう", true),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹"),
            ]);

        Assert.Equal(
            [
                ("Mandarin", "", "nʊŋ⁵¹"),
                ("Japanese", "Kan-on", "ろう"),
                ("Japanese", "Kan-on", "ら"),
                ("Japanese", "", "ろ"),
                ("Japanese", "Go-on", "る"),
                ("Cantonese", "", "lʊŋ⁶"),
                ("Wu", "", "ŋu"),
            ],
            sorted.Select(row => (row.LReflexDraftLanguage, row.LReflexDraftKind, row.LReflexDraftText)));
    }

    [Fact]
    public void PackLoad_ClassicalChinese_DeclaresOrderApartFromRules()
    {
        LLanguage pack = TInterface.TLanguageLoad("Classical Chinese");

        Assert.Equal(
            ["Korean", "Japanese", "Mandarin", "Cantonese", "Gan", "Hakka", "Jin", "Southern Min", "Wu", "Xiang"],
            pack.LLanguageReflexOrder.LReflexOrderLanguages);
        Assert.Equal(
            ["Go-on", "Kan-on", "Tō-on", "Sō-on", "Kan'yō-on"],
            pack.LLanguageReflexOrder.LReflexOrderKinds["Japanese"]);
        Assert.Equal(
            ["Korean", "Mandarin", "Cantonese", "Japanese"],
            pack.LLanguageReflexRules.Take(4).Select(rule => rule.LReflexRuleLanguage));
    }

    [Fact]
    public void ReflexRead_StoredOutOfPackOrder_ListsInDeclaredOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexOrderPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "弄",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes:
            [
                TInterface.TReflexDraftCreate("Wu", "", "ŋu"),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Cantonese", "", "lʊŋ⁶"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ら"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう", true),
                TInterface.TReflexDraftCreate("Korean", "", "롱"),
            ]));
        string[] declared = ["Korean", "Japanese", "Japanese", "Japanese", "Mandarin", "Cantonese", "Wu"];

        IReadOnlyList<LReflexDraft> shown = engine.TEntryReflexRead(entry.LEntryId);
        LDraft edited = engine.TEngineDraftStart("Input", entry.LEntryId);
        LLiveryPage livery = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LRig rig = workspace.TWorkspaceRigCreate();
        LPortraitPage portrait = TInterface.TPortraitClerkCreate(rig)
            .TPortraitClerkRead(entry.LEntryId, TInterface.TPortraitLabelRead());
        LMarkupEntry exported = Assert.IsType<LMarkupEntry>(
            TInterface.TMarkupExportCreate(rig).TMarkupClerkLoad(entry.LEntryId));

        Assert.Equal(declared, shown.Select(row => row.LReflexDraftLanguage));
        Assert.Equal(["롱", "ろう", "ら", "る", "nʊŋ⁵¹", "lʊŋ⁶", "ŋu"], shown.Select(row => row.LReflexDraftText));
        Assert.Equal(
            shown.Select(row => row.LReflexDraftId),
            edited.LDraftContent.LEntryDraftReflexes.Select(row => row.LReflexDraftId));
        Assert.Equal(declared, livery.LLiveryPageDraft.LEntryDraftReflexes.Select(row => row.LReflexDraftLanguage));
        Assert.Equal(declared.Length, portrait.LPortraitPageLine.Count);
        Assert.All(
            declared.Zip(portrait.LPortraitPageLine),
            pair => Assert.StartsWith(pair.First, pair.Second.LPortraitLineLabel));
        Assert.Equal(declared, exported.LMarkupEntryReflex.Select(row => row.LReflexDraftLanguage));
        Assert.False(engine.TEngineDraftCheck(edited.LDraftId));
        Assert.Equal(
            0, workspace.TWorkspaceCountRead("SELECT position FROM reflex WHERE language = 'Wu';"));
    }

    [Fact]
    public void EntryUpdate_SameRowsInAnotherOrder_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
            ]));
        const string changes = "SELECT COUNT(*) FROM revision_change WHERE target_type = 'reflex';";
        long logged = workspace.TWorkspaceCountRead(changes);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        Assert.Equal(["Japanese", "Korean"], loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftLanguage));

        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(logged, workspace.TWorkspaceCountRead(changes));
        Assert.Equal(
            0, workspace.TWorkspaceCountRead("SELECT position FROM reflex WHERE language = 'Korean';"));
    }

    [Fact]
    public void DraftMatch_SameRowsInAnotherOrder_IsNoChange()
    {
        LEntryDraft content = TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱", id: 4),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る", id: 9),
            ]);

        Assert.True(TInterface.TDraftMatch(
            content, content with { LEntryDraftReflexes = [.. content.LEntryDraftReflexes.Reverse()] }));
        Assert.False(TInterface.TDraftMatch(
            content,
            content with
            {
                LEntryDraftReflexes = [content.LEntryDraftReflexes[1], content.LEntryDraftReflexes[1]],
            }));
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
