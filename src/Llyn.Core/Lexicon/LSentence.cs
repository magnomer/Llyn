namespace Llyn.Core;

public sealed record LSentence(
    string LSentenceId,
    string LSentenceOwnerId,
    int LSentencePosition,
    LExample? LSentenceExample,
    LStateValue LSentenceParticle,
    LStateValue LSentenceDependence)
{
    public LStateValue LSentenceParticle { get; init; } = LSentenceParticle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSentenceDependence { get; init; } =
        LSentenceDependence ?? LStateValue.LStateValueUnspecified;
}
