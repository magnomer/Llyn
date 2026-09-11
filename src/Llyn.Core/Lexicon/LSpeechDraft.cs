namespace Llyn.Core;

public sealed record LSpeechDraft(
    long LSpeechDraftValue,
    string? LSpeechDraftCustom,
    string LSpeechDraftName = "")
{
    public string LSpeechDraftName { get; init; } =
        string.IsNullOrEmpty(LSpeechDraftName)
            ? LSpeechDraftCustom ?? string.Empty
            : LSpeechDraftName;

    public static LSpeechDraft LSpeechDraftCreate(string name)
    {
        return new LSpeechDraft(0, name, name);
    }

    public static LSpeechDraft LSpeechDraftCreate(long valueId, string name)
    {
        return new LSpeechDraft(valueId, null, name);
    }

    public bool LSpeechDraftEmpty =>
        LSpeechDraftValue <= 0 && string.IsNullOrWhiteSpace(LSpeechDraftCustom);
}
