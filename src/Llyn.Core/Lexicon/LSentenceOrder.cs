namespace Llyn.Core;

public sealed record LSentenceOrder(
    int LSentenceOrderParticle,
    int LSentenceOrderDependence)
{
    public static LSentenceOrder LSentenceOrderDefault { get; } = new(0, 1);
}
