namespace Convention.Tests;

internal static class TAuditDraftSetting
{
    public const int TAuditGeneration = 10;

    public static readonly string[] TAuditDraftInclude =
    [
        "src/Llyn.Core/Lexicon/*.cs",
    ];

    public static readonly string[] TAuditDraftTypes =
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

    public static readonly string[] TAuditDraftWaiver =
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

    public static readonly string[] TAuditDraftPortrait =
    [
        "src/Llyn.Core/Portrait/*.cs",
        "src/Llyn.Application/Portrait/*.cs",
        "src/Llyn.ShellEngine/LEnginePortrait*.cs",
    ];

    public static readonly string[] TAuditPortraitWaiver =
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

    public static readonly string[] TAuditDraftMarkup =
    [
        "src/Llyn.Core/Markup/*.cs",
        "src/Llyn.Application/Markup/LMarkupClerk.cs",
    ];

    public static readonly string[] TAuditMarkupWaiver =
    [
        "LSpeechDraftValue",
    ];

    public static readonly string[] TAuditDraftExemplar =
    [
        "tests/Llyn.Tests/Interface/TExemplar.cs",
    ];

    public static readonly string[] TAuditExemplarWaiver = [];
}
