namespace Llyn.Conduct;

public sealed record CMentionPiece(
    int CMentionPieceOffset,
    string CMentionPieceText,
    bool? CMentionPieceLinked);
