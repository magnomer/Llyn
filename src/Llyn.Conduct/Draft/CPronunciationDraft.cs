namespace Llyn.Conduct;

public sealed record CPronunciationDraft(
    long CPronunciationDraftId,
    string CPronunciationDraftIpa,
    string CPronunciationDraftRespelling,
    string CPronunciationDraftVariety,
    string CPronunciationDraftAudio);
