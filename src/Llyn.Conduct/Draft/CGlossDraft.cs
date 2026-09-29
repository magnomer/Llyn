namespace Llyn.Conduct;

public sealed record CGlossDraft(
    long CGlossDraftId, string CGlossDraftLanguage, CStateValue CGlossDraftText, bool CGlossDraftNamed)
{
    public string? CGlossDraftHint => CGlossDraftNamed ? null : "Example.Language";
}
