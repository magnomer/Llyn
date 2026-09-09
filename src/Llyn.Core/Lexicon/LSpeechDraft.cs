namespace Llyn.Core;

public sealed record LSpeechDraft(
    string? LSpeechDraftValue,
    string? LSpeechDraftCustom,
    string LSpeechDraftName = "")
{
    public string LSpeechDraftName { get; init; } =
        string.IsNullOrEmpty(LSpeechDraftName)
            ? LSpeechDraftCustom ?? LSpeechDraftValue ?? string.Empty
            : LSpeechDraftName;

    public static LSpeechDraft LSpeechDraftCreate(string name)
    {
        return new LSpeechDraft(null, name, name);
    }

    public bool LSpeechDraftEmpty =>
        string.IsNullOrWhiteSpace(LSpeechDraftValue) && string.IsNullOrWhiteSpace(LSpeechDraftCustom);
}
