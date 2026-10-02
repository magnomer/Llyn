namespace Llyn.Core;

public sealed record LMentionPiece(
    int LMentionPieceOffset,
    LMention? LMentionPieceStored,
    string LMentionPieceText);
