using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLiveryCard
{
    private const string TLiveryPicture =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==";

    [Fact]
    public void LiveryFormat_ScriptImage_AddsImageParcel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLiveryPage page = TLiveryPageRead(engine, "整");
        byte[] picture = Convert.FromBase64String(TLiveryPicture);
        LScriptImage image = TInterface.TScriptImageCreate("整", "Seal", 0, "Shuowen", "A quote", picture);
        page = page with
        {
            LLiveryPageScript = [TInterface.TScriptGroupCreate(string.Empty, "Seal", "A quote", [image])],
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);

        LParcel parcel = Assert.Single(note.LLiveryNoteParcel);
        Assert.Equal("image/png", parcel.LParcelMime);
        string body = note.LLiveryNoteBody;
        int style = body.IndexOf("<span class=\"llyn-style\">Seal</span>", StringComparison.Ordinal);
        int shown = body.IndexOf("<img src=\":/" + parcel.LParcelId + "\"", StringComparison.Ordinal);
        int caption = body.IndexOf("<span class=\"llyn-caption\">Shuowen</span>", StringComparison.Ordinal);
        int quote = body.IndexOf("<span class=\"llyn-quote\">A quote</span>", StringComparison.Ordinal);
        Assert.True(style >= 0 && style < shown && shown < caption && caption < quote);
    }

    [Fact]
    public void LiveryFormat_MeaningCard_KeepsContentOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLiveryPage page = TLiveryPageRead(engine, "break");
        LStateValue none = LStateValue.LStateValueUnspecified;
        LCardDraft child = TInterface.TCardDraftCreate("Snap", none, none, [], [], [], [], [], 1);
        LCardDraft card = TInterface.TCardDraftCreate(
            "Physically break",
            none,
            "to come apart",
            [TInterfaceExample.TSentenceDraftCreate("It broke", 5, TInterface.TStateAnchorCreate(9))],
            [TInterface.TSituationDraftCreate("at home", 1)],
            [3],
            ["testtag"],
            [TInterface.TImageDraftCreate("https://example.test/a.png")],
            1,
            77,
            [TInterface.TVideoDraftCreate("https://example.test/v.mp4", "00:01 - 00:02")],
            [TInterface.TRegisterDraftCreate("Formal", 1)]).TCardChildSet(child);
        page = page with
        {
            LLiveryPageDraft = page.LLiveryPageDraft with { LEntryDraftMeanings = [card] },
            LLiveryPageTarget = new Dictionary<long, IReadOnlyList<LTranslationTarget>>
            {
                [77] = [TInterface.TTranslationTargetCreate(3, "부수다", "Korean")],
            },
            LLiveryPageSource = new Dictionary<long, string> { [9] = "Testing Tom (2024)" },
        };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int[] places =
        [
            body.IndexOf("## Display.MeaningPlural", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-title\">Physically break</span>", StringComparison.Ordinal),
            body.IndexOf("to come apart", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-situation\">at home</span>", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-register\">Formal</span>", StringComparison.Ordinal),
            body.IndexOf("[부수다](:/3)", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-number\">1.1</span>", StringComparison.Ordinal),
            body.IndexOf("- It broke <span class=\"llyn-byline\">Testing Tom (2024)</span>", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-tag\">testtag</span>", StringComparison.Ordinal),
            body.IndexOf("(<https://example.test/a.png>)", StringComparison.Ordinal),
            body.IndexOf("(<https://example.test/v.mp4>)", StringComparison.Ordinal),
        ];
        Assert.All(places, static place => Assert.True(place >= 0));
        Assert.Equal(places.Order(), places);
    }

    [Fact]
    public void LiveryFormat_StoredBanner_DrawsLanguageFlags()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLiveryPage page = TLiveryPageRead(engine, "break");
        string path = Path.Combine(workspace.TWorkspaceFolder, "korean.png");
        File.WriteAllBytes(path, Convert.FromBase64String(TLiveryPicture));
        LStateValue none = LStateValue.LStateValueUnspecified;
        LExampleDraft example = TInterfaceExample.TExampleDraftCreate(
            "It broke", 5, LStateAnchor.LStateAnchorUnspecified, "English") with
        {
            LExampleDraftGloss = [TInterface.TGlossDraftCreate(1, "Korean", "부서졌다")],
        };
        LCardDraft card = TInterface.TCardDraftCreate(
            "Physically break", none, none, [TInterface.TSentenceDraftCreate(example)], [], [], [], [], 1, 77);
        page = page with
        {
            LLiveryPageDraft = page.LLiveryPageDraft with { LEntryDraftMeanings = [card] },
            LLiveryPageTarget = new Dictionary<long, IReadOnlyList<LTranslationTarget>>
            {
                [77] = [TInterface.TTranslationTargetCreate(3, "부수다", "Korean")],
            },
            LLiveryPageIncoming =
                [TInterface.TUsageCreate(1, LOwner.LOwnerMeaning, 42, "부수다", "Korean", none)],
            LLiveryPageBanner = new Dictionary<string, string>(StringComparer.Ordinal) { ["Korean"] = path },
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);

        string flag = "<img src=\":/" + Assert.Single(note.LLiveryNoteParcel).LParcelId + "\" class=\"llyn-flag\">";
        string body = note.LLiveryNoteBody;
        Assert.Contains("<span class=\"llyn-target\">" + flag + " [부수다](:/3)", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-language\">" + flag + " Korean</span>", body, StringComparison.Ordinal);
        Assert.Contains(flag + " <span class=\"llyn-gloss\">부서졌다</span>", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_IncomingUsage_LinksEntryNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLiveryPage page = TLiveryPageRead(engine, "부수다");
        LUsage usage = TInterface.TUsageCreate(
            1, LOwner.LOwnerMeaning, 42, "弄坏", "Mandarin", LStateValue.LStateValueUnspecified);
        page = page with { LLiveryPageIncoming = [usage] };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int heading = body.IndexOf("## Display.Translated", StringComparison.Ordinal);
        int link = body.IndexOf(
            "[弄坏](:/42) <span class=\"llyn-owner\">Display.MeaningSingle</span>", StringComparison.Ordinal);
        Assert.True(heading >= 0 && heading < link);
    }

    [Fact]
    public void LiveryFormat_VideoFile_AddsVideoParcel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLiveryPage page = TLiveryPageRead(engine, "break");
        string path = Path.Combine(workspace.TWorkspaceFolder, "clip.mp4");
        File.WriteAllBytes(path, [1, 2, 3, 4]);
        LStateValue none = LStateValue.LStateValueUnspecified;
        LCardDraft card = TInterface.TCardDraftCreate(
            "Physically break", none, none, [], [], [], [], [], 1, video: [TInterface.TVideoDraftCreate(path)]);
        page = page with { LLiveryPageDraft = page.LLiveryPageDraft with { LEntryDraftMeanings = [card] } };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);

        LParcel parcel = Assert.Single(note.LLiveryNoteParcel);
        Assert.Equal(("video/mp4", "video.mp4"), (parcel.LParcelMime, parcel.LParcelTitle));
        Assert.Contains(
            "[" + parcel.LParcelTitle + "](:/" + parcel.LParcelId + ")",
            note.LLiveryNoteBody,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<video", note.LLiveryNoteBody, StringComparison.Ordinal);
    }

    private static LLiveryPage TLiveryPageRead(LEngine engine, string headword)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a sense", 1)], []));
        return Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
    }
}
