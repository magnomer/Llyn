namespace Llyn.Tests;

internal static class TDraftCoverageSetting
{
    public static readonly string[] TDraftTypes =
    [
        "LCardDraft",
        "LEntryDraft",
        "LExampleDraft",
        "LForm",
        "LGlossDraft",
        "LImageDraft",
        "LInflection",
        "LMentionDraft",
        "LPronunciationDraft",
        "LReflexDraft",
        "LRegisterDraft",
        "LSentenceDraft",
        "LSituationDraft",
        "LSpeechDraft",
        "LSyllable",
        "LTagDraft",
        "LTranscriptionDraft",
        "LVideoDraft",
    ];

    public static readonly string[] TDraftWaiver =
    [
        "LCardDraftId",
        "LCardDraftPosition",
        "LExampleDraftId",
        "LFormEntryId",
        "LFormPosition",
        "LGlossDraftId",
        "LImageDraftId",
        "LInflectionEntryId",
        "LInflectionId",
        "LInflectionPosition",
        "LInflectionRegular",
        "LMentionDraftId",
        "LPronunciationDraftId",
        "LPronunciationDraftSeeded",
        "LReflexDraftAnatomy",
        "LReflexDraftAnchors",
        "LReflexDraftId",
        "LRegisterDraftId",
        "LSentenceDraftId",
        "LSituationDraftId",
        "LSpeechDraftCustom",
        "LSyllablePosition",
        "LSyllablePronunciationId",
        "LTagDraftId",
        "LTranscriptionDraftId",
        "LTranscriptionDraftSeeded",
        "LVideoDraftId",
    ];

    public static readonly string[] TDraftPortrait =
    [
        "src/Llyn.Core/Portrait/*.cs",
        "src/Llyn.Application/Portrait/*.cs",
        "src/Llyn.ShellEngine/Tool/LPortraitFacade.cs",
    ];

    public static readonly string[] TPortraitWaiver =
    [
        "LEntryDraftInflections",
        "LInflectionMorphology",
        "LInflectionSpeechId",
        "LMentionDraftLength",
        "LMentionDraftOffset",
        "LMentionDraftSense",
        "LPronunciationDraftAudio",
        "LPronunciationDraftSource",
        "LPronunciationDraftSyllables",
        "LReflexDraftOwned",
        "LSituationDraftDescription",
        "LSituationDraftKind",
        "LSpeechDraftName",
        "LSpeechDraftValue",
        "LSyllableCoda",
        "LSyllableMedial",
        "LSyllableNucleus",
        "LSyllableOnset",
        "LSyllableToneNumber",
        "LSyllableTonePoints",
    ];

    public static readonly string[] TDraftMarkup =
    [
        "src/Llyn.Core/Markup/*.cs",
        "src/Llyn.Application/Markup/LMarkupClerk.cs",
    ];

    public static readonly string[] TMarkupWaiver =
    [
        "LSpeechDraftValue",
    ];

    public static readonly string[] TDraftExemplar =
    [
        "tests/Llyn.Tests.Interface/TExemplar.cs",
    ];

    public static readonly string[] TExemplarWaiver = [];
}
