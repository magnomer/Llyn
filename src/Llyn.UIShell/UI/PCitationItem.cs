using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PCitationItem
{
    internal PCitationItem(string id, string name)
    {
        PCitationItemId = id;
        PCitationItemName = name;
    }

    public string PCitationItemId { get; }

    public string PCitationItemName { get; }

    internal static PCitationItem PCitationItemCreate(LReference reference)
    {
        return new PCitationItem(
            reference.LReferenceId,
            PCitationTextRead(reference.LReferenceTitle)
                ?? PCitationTextRead(reference.LReferenceProgram)
                ?? PCitationTextRead(reference.LReferenceChannel)
                ?? PCitationTextRead(reference.LReferenceUrl)
                ?? reference.LReferenceId);
    }

    private static string? PCitationTextRead(LStateValue value)
    {
        return value.LStateValueState == LState.LStateSpecified
            && !string.IsNullOrWhiteSpace(value.LStateValueText)
                ? value.LStateValueText
                : null;
    }
}
