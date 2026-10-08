namespace Llyn.Conduct;

public sealed record CSentenceOrder(int CSentenceOrderParticle, int CSentenceOrderDependence)
{
    public static CSentenceOrder CSentenceOrderPlain { get; } = new(0, 1);
}
