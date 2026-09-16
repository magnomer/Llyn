namespace Llyn.Core;

public sealed record LAnatomyPiece(
    string LAnatomyPieceOnset = "",
    string LAnatomyPieceVowel = "",
    string LAnatomyPieceCoda = "",
    string LAnatomyPieceTone = "")
{
    public static readonly LAnatomyPiece LAnatomyPieceEmpty = new();

    public string LAnatomyPieceOnset { get; init; } = LAnatomyPieceOnset ?? string.Empty;

    public string LAnatomyPieceVowel { get; init; } = LAnatomyPieceVowel ?? string.Empty;

    public string LAnatomyPieceCoda { get; init; } = LAnatomyPieceCoda ?? string.Empty;

    public string LAnatomyPieceTone { get; init; } = LAnatomyPieceTone ?? string.Empty;
}
