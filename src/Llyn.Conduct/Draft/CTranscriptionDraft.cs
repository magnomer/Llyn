namespace Llyn.Conduct;

public sealed record CTranscriptionDraft(
    long CTranscriptionDraftId,
    string CTranscriptionDraftScheme,
    string CTranscriptionDraftText)
{
    public string CTranscriptionDraftKey => CScheme.CSchemeKeyRead(CTranscriptionDraftScheme);
}
