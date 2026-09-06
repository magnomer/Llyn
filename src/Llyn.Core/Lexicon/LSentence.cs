namespace Llyn.Core;

public sealed record LSentence(
    string LSentenceId,
    string LSentenceMeaningId,
    int LSentencePosition,
    LExample LSentenceExample,
    LExample? LSentenceRevision,
    LStateValue LSentenceParticle,
    LStateValue LSentenceDependence)
{
    public LStateValue LSentenceParticle { get; init; } = LSentenceParticle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSentenceDependence { get; init; } =
        LSentenceDependence ?? LStateValue.LStateValueUnspecified;
}
