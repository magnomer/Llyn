namespace Llyn.Core;

public sealed record LMentionPiece(
    int LMentionPieceOffset,
    int LMentionPieceLength,
    LMention? LMentionPieceStored);
