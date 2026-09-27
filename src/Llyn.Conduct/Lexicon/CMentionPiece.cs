namespace Llyn.Conduct;

public sealed record CMentionPiece(
    int CMentionPieceOffset,
    int CMentionPieceEnd,
    string CMentionPieceText,
    bool? CMentionPieceLinked);
