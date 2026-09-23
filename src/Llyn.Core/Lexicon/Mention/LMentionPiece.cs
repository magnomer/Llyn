namespace Llyn.Core;

public sealed record LMentionPiece(
    int LMentionPieceOffset,
    int LMentionPieceLength,
    LMention? LMentionPieceStored,
    string LMentionPieceText)
{
    public int LMentionPieceEnd => LMentionPieceOffset + LMentionPieceLength;

    public bool LMentionPieceLinked => LMentionPieceStored is { LMentionLinked: true };
}
