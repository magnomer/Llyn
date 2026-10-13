namespace Llyn.Conduct;

public sealed record CMentionLabel(
    long CMentionLabelId,
    string CMentionLabelWord,
    string CMentionLabelName,
    string CMentionLabelSense,
    bool CMentionLabelLinked)
{
    public string? CMentionLabelKey => CMentionLabelLinked ? null : "Mention.Silent";
}
