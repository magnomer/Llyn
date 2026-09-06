namespace Llyn.Core;

public sealed record LReference(
    string LReferenceId,
    LStateValue LReferenceTitle,
    LStateValue LReferenceProgram,
    LStateValue LReferenceChannel,
    LStateValue LReferenceYear,
    LStateValue LReferenceUrl,
    LState LReferenceAuthorState)
{
    public string LReferenceNameRead()
    {
        return LReferenceTextRead(LReferenceTitle)
            ?? LReferenceTextRead(LReferenceProgram)
            ?? LReferenceTextRead(LReferenceChannel)
            ?? LReferenceTextRead(LReferenceUrl)
            ?? LReferenceId;
    }

    private static string? LReferenceTextRead(LStateValue value)
    {
        return value.LStateValueState == LState.LStateSpecified
            && !string.IsNullOrWhiteSpace(value.LStateValueText)
                ? value.LStateValueText
                : null;
    }
}
