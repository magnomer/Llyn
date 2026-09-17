namespace Llyn.Core;

public sealed record LMentionLabel(
    long LMentionLabelId,
    string LMentionLabelWord,
    long LMentionLabelEntry,
    string LMentionLabelName,
    string LMentionLabelSense);
