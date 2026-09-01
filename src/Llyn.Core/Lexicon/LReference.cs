namespace Llyn.Core;

public sealed record LReference(
    string LReferenceId,
    LReferenceValue LReferenceTitle,
    LReferenceValue LReferenceProgram,
    LReferenceValue LReferenceChannel,
    LReferenceValue LReferenceYear,
    LReferenceValue LReferenceUrl,
    LState LReferenceAuthorState);
