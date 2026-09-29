namespace Llyn.Core;

public sealed record LReferenceRow(
    long LReferenceRowId,
    string LReferenceRowLead,
    string LReferenceRowMark,
    string LReferenceRowTail,
    string LReferenceRowCount);
