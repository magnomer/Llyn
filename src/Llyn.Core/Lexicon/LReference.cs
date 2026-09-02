namespace Llyn.Core;

public sealed record LReference(
    string LReferenceId,
    LStateValue LReferenceTitle,
    LStateValue LReferenceProgram,
    LStateValue LReferenceChannel,
    LStateValue LReferenceYear,
    LStateValue LReferenceUrl,
    LState LReferenceAuthorState);
