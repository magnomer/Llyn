using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnatomy
{
    private const string TAnatomyLanguage = "Classical Chinese";

    private static IReadOnlyList<LAnatomyRule> TAnatomyRuleRead() =>
        TInterface.TLanguageLoad(TAnatomyLanguage).LLanguageAnatomies;

    [Fact]
    public void LanguageLoaderLoad_ClassicalChinesePack_ReadsAnatomyRules()
    {
        IReadOnlyList<LAnatomyRule> rules = TAnatomyRuleRead();

        Assert.NotEmpty(rules);
        Assert.Contains(rules, rule => rule.TAnatomyRuleMatch("Korean"));
        Assert.Contains(rules, rule => rule.TAnatomyRuleMatch("Japanese"));
        Assert.Contains(rules, rule => rule.TAnatomyRuleMatch("Xiang"));
        Assert.Empty(TInterface.TLanguageLoad("Korean").LLanguageAnatomies);
    }

    [Fact]
    public void AnatomyScan_KoreanReading_SplitsJamo()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Korean", "림(임)", string.Empty);

        Assert.Equal(("ㄹ", "ㅣ", "ㅁ", ""), TAnatomyIpaRead(anatomy));
        Assert.Equal(("ㄹ", "ㅣ", "ㅁ", ""), TAnatomyRespellingRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_KoreanOpenSyllable_LeavesCodaEmpty()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Korean", "가", string.Empty);

        Assert.Equal(("ㄱ", "ㅏ", "", ""), TAnatomyIpaRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_JapaneseReading_KeepsMoraAsCoda()
    {
        IReadOnlyList<LAnatomyRule> rules = TAnatomyRuleRead();

        Assert.Equal(
            ("r", "i", "n", ""),
            TAnatomyIpaRead(TInterface.TAnatomyScan(rules, "Japanese", "りん", string.Empty)));
        Assert.Equal(
            ("r", "i", "mu", ""),
            TAnatomyIpaRead(TInterface.TAnatomyScan(rules, "Japanese", "りむ", string.Empty)));
        Assert.Equal(
            ("ry", "a", "ku", ""),
            TAnatomyIpaRead(TInterface.TAnatomyScan(rules, "Japanese", "りゃく", string.Empty)));
        Assert.Equal(
            ("sh", "o", "u", ""),
            TAnatomyRespellingRead(TInterface.TAnatomyScan(rules, "Japanese", "しょう", string.Empty)));
    }

    [Fact]
    public void AnatomyScan_XiangReading_ReadsToneDigits()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Xiang", "lin¹³", "lin¹³");

        Assert.Equal(("l", "i", "n", "13"), TAnatomyIpaRead(anatomy));
        Assert.Equal(("l", "i", "n", "13"), TAnatomyRespellingRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_MandarinAffricate_SplitsIpaAndRespellingApart()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Mandarin", "ʈ͡ʂɤŋ²¹⁴", "tʂəŋ²¹⁴");

        Assert.Equal(("ʈ͡ʂ", "ɤ", "ŋ", "214"), TAnatomyIpaRead(anatomy));
        Assert.Equal(("tʂ", "ə", "ŋ", "214"), TAnatomyRespellingRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_CantoneseCheckedTone_KeepsUnreleasedCoda()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Cantonese", "ɔːk̚³", "ok̚³");

        Assert.Equal(("", "ɔː", "k̚", "3"), TAnatomyIpaRead(anatomy));
        Assert.Equal(("", "o", "k̚", "3"), TAnatomyRespellingRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_ToneSandhi_KeepsBothTonesJoined()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Mandarin", "i⁵⁵⁻³⁵", string.Empty);

        Assert.Equal(("", "i", "", "55-35"), TAnatomyIpaRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_NasalizedOnset_KeepsMarkWithOnset()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Xiang", "l̃an²¹", string.Empty);

        Assert.Equal(("l̃", "a", "n", "21"), TAnatomyIpaRead(anatomy));
    }

    [Fact]
    public void AnatomyScan_UnknownLanguage_ReturnsEmpty()
    {
        LAnatomy anatomy = TInterface.TAnatomyScan(TAnatomyRuleRead(), "Vietnamese", "lâm", string.Empty);

        Assert.True(anatomy.LAnatomyBlank);
        Assert.Equal(LAnatomy.LAnatomyEmpty, anatomy);
    }

    [Fact]
    public void EntrySave_ClassicalChineseReflexes_StoresAnatomyPerRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TAnatomyDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "림(임)", note: "수풀"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "りむ"),
                TInterface.TReflexDraftCreate("Xiang", "", "lin¹³", respelling: "lin¹³"),
            ]));

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(
            [("ㄹ", "ㅣ", "ㅁ", ""), ("r", "i", "mu", ""), ("l", "i", "n", "13")],
            read.Select(row => TAnatomyIpaRead(row.LReflexAnatomy)));
        Assert.Equal(
            [("ㄹ", "ㅣ", "ㅁ", ""), ("r", "i", "mu", ""), ("l", "i", "n", "13")],
            read.Select(row => TAnatomyRespellingRead(row.LReflexAnatomy)));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal(
            read.Select(row => row.LReflexAnatomy),
            loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftAnatomy));
    }

    [Fact]
    public void EntrySave_EnglishEntry_LeavesAnatomyBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "弄",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", "", "림")]));

        LReflex row = Assert.Single(engine.TEngineReflexRead(entry.LEntryId));
        Assert.True(row.LReflexAnatomy.LAnatomyBlank);
    }

    [Fact]
    public void RequestApply_ReflexTextChange_ResolvesAnatomy()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, TAnatomyLanguage));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Japanese", "Go-on", 0));
        LReflexDraft added = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.True(added.LReflexDraftAnatomy.LAnatomyBlank);

        answered = engine.TEngineRequestApply(
            TInterface.TReflexTextCreate(started.LDraftId, added.LReflexDraftId, "りん"));

        LReflexDraft filled = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.Equal(("r", "i", "n", ""), TAnatomyIpaRead(filled.LReflexDraftAnatomy));
    }

    [Fact]
    public void RequestApply_EntryLanguageChange_RebuildsAnatomy()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAdditionCreate(started.LDraftId, "Korean", "", 0));
        long reflexId = Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftId;
        answered = engine.TEngineRequestApply(TInterface.TReflexTextCreate(started.LDraftId, reflexId, "림"));
        Assert.True(Assert.Single(answered.LDraftContent.LEntryDraftReflexes).LReflexDraftAnatomy.LAnatomyBlank);

        answered = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, TAnatomyLanguage));

        LReflexDraft rebuilt = Assert.Single(answered.LDraftContent.LEntryDraftReflexes);
        Assert.Equal(("ㄹ", "ㅣ", "ㅁ", ""), TAnatomyIpaRead(rebuilt.LReflexDraftAnatomy));
    }

    private static LEntryDraft TAnatomyDraftCreate(IReadOnlyList<LReflexDraft> reflexes)
    {
        return TInterface.TEntryDraftCreate(
            "林",
            TAnatomyLanguage,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a wood", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
    }

    private static (string, string, string, string) TAnatomyIpaRead(LAnatomy anatomy) =>
        (anatomy.LAnatomyOnsetIpa, anatomy.LAnatomyVowelIpa, anatomy.LAnatomyCodaIpa, anatomy.LAnatomyToneIpa);

    private static (string, string, string, string) TAnatomyRespellingRead(LAnatomy anatomy) =>
        (
            anatomy.LAnatomyOnsetRespelling,
            anatomy.LAnatomyVowelRespelling,
            anatomy.LAnatomyCodaRespelling,
            anatomy.LAnatomyToneRespelling);
}
