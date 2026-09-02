using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PReference
{
    internal PReference(string id, string name)
    {
        PReferenceId = id;
        PReferenceName = name;
    }

    public string PReferenceId { get; }

    public string PReferenceName { get; }

    internal static PReference PReferenceCreate(LReference reference)
    {
        return new PReference(
            reference.LReferenceId,
            PReferenceTextRead(reference.LReferenceTitle)
                ?? PReferenceTextRead(reference.LReferenceProgram)
                ?? PReferenceTextRead(reference.LReferenceChannel)
                ?? PReferenceTextRead(reference.LReferenceUrl)
                ?? reference.LReferenceId);
    }

    private static string? PReferenceTextRead(LStateValue value)
    {
        return value.LStateValueState == LState.LStateSpecified
            && !string.IsNullOrWhiteSpace(value.LStateValueText)
                ? value.LStateValueText
                : null;
    }
}
