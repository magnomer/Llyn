using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRespellingDraft
{
    [Fact]
    public void RequestApply_MandarinIpa_DerivesRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Mandarin"));

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "ʈ͡ʂʊŋ⁵⁵"));

        LPronunciationDraft primary = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal("ʈ͡ʂʊŋ⁵⁵", primary.LPronunciationDraftIpa);
        Assert.Equal("tʂuŋ⁵⁵", primary.LPronunciationDraftRespelling);
    }

    [Fact]
    public void RequestApply_Respelling_StandsUntilIpaChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Mandarin"));
        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "xän⁵¹"));

        LDraft written = engine.TEngineRequestApply(TInterface.TRequestRespellingCreate(started.LDraftId, "hän"));
        LPronunciationDraft held = Assert.Single(written.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal("xän⁵¹", held.LPronunciationDraftIpa);
        Assert.Equal("hän", held.LPronunciationDraftRespelling);

        LDraft derived = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "xɤ³⁵"));
        Assert.Equal(
            "hə³⁵", Assert.Single(derived.LDraftContent.LEntryDraftPronunciations).LPronunciationDraftRespelling);
    }

    [Fact]
    public void RequestApply_RowIpaAndVariety_DerivesPerRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "English"));

        LDraft added = engine.TEngineRequestApply(
            TInterface.TPronunciationAdditionCreate(started.LDraftId, "ˈhæpi", 0));
        long rowId = Assert.Single(added.LDraftContent.LEntryDraftPronunciations).LPronunciationDraftId;
        Assert.Equal("ˈhæpi", added.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftRespelling);

        LDraft tagged = engine.TEngineRequestApply(
            TInterface.TPronunciationVarietyCreate(started.LDraftId, rowId, "British"));
        Assert.Equal("ˈhapij", tagged.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftRespelling);

        LDraft edited = engine.TEngineRequestApply(
            TInterface.TPronunciationIpaCreate(started.LDraftId, rowId, "feɪs"));
        Assert.Equal("fejs", edited.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftRespelling);

        LDraft overwritten = engine.TEngineRequestApply(
            TInterface.TPronunciationRespellingCreate(started.LDraftId, rowId, "fays"));
        Assert.Equal("feɪs", overwritten.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftIpa);
        Assert.Equal("fays", overwritten.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftRespelling);
    }

    [Fact]
    public void RequestApply_Language_RefreshesEveryRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Mandarin"));
        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "ɕi⁵⁵"));

        LDraft plain = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Spanish"));
        Assert.Equal(
            string.Empty, Assert.Single(plain.LDraftContent.LEntryDraftPronunciations).LPronunciationDraftRespelling);

        LDraft back = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Mandarin"));
        Assert.Equal("si⁵⁵", Assert.Single(back.LDraftContent.LEntryDraftPronunciations).LPronunciationDraftRespelling);
    }

    [Fact]
    public void EntrySave_Respelling_ReadsBackBesideIpa()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRespellingDraftCreate(
            [
                TInterface.TPronunciationDraftCreate("ʈ͡ʂʊŋ⁵⁵", respelling: "tʂuŋ⁵⁵"),
                TInterface.TPronunciationDraftCreate("ʈ͡ʂʊŋ⁵¹"),
            ]));

        Assert.Equal(
            [("ʈ͡ʂʊŋ⁵⁵", "tʂuŋ⁵⁵"), ("ʈ͡ʂʊŋ⁵¹", "tʂuŋ⁵¹")],
            engine.TEntryPronunciationRead(entry.LEntryId)
                .Select(row => (row.LPronunciationDraftIpa, row.LPronunciationDraftRespelling)));
    }

    [Fact]
    public void EntryUpdate_RespellingOnlyChange_IsStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TRespellingDraftCreate(
            [TInterface.TPronunciationDraftCreate("ʈ͡ʂʊŋ⁵⁵", respelling: "tʂuŋ⁵⁵")]));
        long stored = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId)).LPronunciationDraftId;
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);

        engine.TEngineRequestApply(TInterface.TPronunciationRespellingCreate(started.LDraftId, stored, "tʂuŋ"));
        engine.TEngineDraftCommit(started.LDraftId);

        LPronunciationDraft kept = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId));
        Assert.Equal("ʈ͡ʂʊŋ⁵⁵", kept.LPronunciationDraftIpa);
        Assert.Equal("tʂuŋ", kept.LPronunciationDraftRespelling);
    }

    [Fact]
    public void RequestApply_ReflexText_DerivesBareRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "Classical Chinese"));

        LDraft added = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Mandarin", "", 0));
        long rowId = Assert.Single(added.LDraftContent.LEntryDraftReflexes).LReflexDraftId;
        LDraft typed = engine.TEngineRequestApply(
            TInterface.TReflexTextCreate(started.LDraftId, rowId, "ʈ͡ʂɤŋ²¹⁴"));
        LReflexDraft row = Assert.Single(typed.LDraftContent.LEntryDraftReflexes);
        Assert.Equal("ʈ͡ʂɤŋ²¹⁴", row.LReflexDraftText);
        Assert.Equal("tʂəŋ²¹⁴", row.LReflexDraftRespelling);

        LDraft overwritten = engine.TEngineRequestApply(
            TInterface.TReflexRespellingCreate(started.LDraftId, rowId, "tʂəŋ"));
        Assert.Equal("tʂəŋ", Assert.Single(overwritten.LDraftContent.LEntryDraftReflexes).LReflexDraftRespelling);

        LDraft korean = engine.TEngineRequestApply(
            TInterface.TReflexLanguageCreate(started.LDraftId, rowId, "Korean"));
        Assert.Equal(string.Empty, Assert.Single(korean.LDraftContent.LEntryDraftReflexes).LReflexDraftRespelling);
    }

    [Fact]
    public void EntrySave_RowsWithoutRespelling_StoresBothKinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TRespellingDraftCreate(
            [TInterface.TPronunciationDraftCreate("ʈ͡ʂʊŋ⁵⁵")]) with
        {
            LEntryDraftReflexes =
            [
                TInterface.TReflexDraftCreate("Cantonese", "", "t͡sɪŋ³⁵"),
                TInterface.TReflexDraftCreate("Korean", "", "중"),
            ],
        });

        Assert.Equal(
            ["tsiŋ³⁵", ""],
            engine.TEntryReflexRead(entry.LEntryId).Select(row => row.LReflexDraftRespelling));
        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal("tʂuŋ⁵⁵", Assert.Single(loaded.LEntryDraftPronunciations).LPronunciationDraftRespelling);
        Assert.Equal(
            ["tsiŋ³⁵", ""],
            loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftRespelling));
    }

    [Fact]
    public void RespellingCheck_SwitchAndPack_AnswerTogether()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.False(engine.TEngineRespellingCheck("Mandarin"));

        engine.TEngineRespellingSave(true);

        Assert.True(engine.TEngineRespellingCheck("Mandarin"));
        Assert.True(engine.TEngineRespellingCheck("English"));
        Assert.False(engine.TEngineRespellingCheck("Spanish"));
        Assert.False(engine.TEngineRespellingCheck(string.Empty));
        Assert.True(engine.TEnginePhonemicCheck("Mandarin"));
        Assert.True(engine.TEnginePhonemicCheck("Cantonese"));
        Assert.False(engine.TEnginePhonemicCheck("English"));
        Assert.False(engine.TEnginePhonemicCheck(string.Empty));
    }

    [Fact]
    public void RespellingSave_Flip_RaisesSettingsBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<LSubject> raised = [];
        engine.TEngineObserverAttach(bulletin => raised.Add(bulletin.LBulletinSubject));

        engine.TEngineRespellingSave(true);

        Assert.Contains(raised, subject => subject == LSubject.LSubjectSettings);
    }

    private static LEntryDraft TRespellingDraftCreate(IReadOnlyList<LPronunciationDraft> pronunciations)
    {
        return TInterface.TEntryDraftCreate(
            "中",
            "Mandarin",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "middle", [], [], [], [], [], 1)],
            []) with
        {
            LEntryDraftPronunciations = pronunciations,
        };
    }
}
