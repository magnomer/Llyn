namespace Llyn.Core;

public sealed record LMarkupSentence(
    LMarkupExample? LMarkupSentenceExample,
    LStateValue LMarkupSentenceParticle,
    LStateValue LMarkupSentenceDependence)
{
    public LStateValue LMarkupSentenceParticle { get; init; } =
        LMarkupSentenceParticle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupSentenceDependence { get; init; } =
        LMarkupSentenceDependence ?? LStateValue.LStateValueUnspecified;
}
