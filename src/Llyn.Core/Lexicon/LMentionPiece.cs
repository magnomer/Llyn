namespace Llyn.Core;

public sealed record LMentionPiece(
    int LMentionPieceOffset,
    int LMentionPieceLength,
    LMention? LMentionPieceStored)
{
    public int LMentionPieceEnd => LMentionPieceOffset + LMentionPieceLength;

    public bool LMentionPieceLinked => LMentionPieceStored is { LMentionLinked: true };

}
