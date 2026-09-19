using System;

namespace Llyn.Core;

public sealed record LSentenceOrder(
    int LSentenceOrderParticle,
    int LSentenceOrderDependence)
{
    public static LSentenceOrder LSentenceOrderDefault { get; } = new(0, 1);

    public bool LSentenceOrderLeading => LSentenceOrderParticle == 0;

    public string LSentenceOrderFormat(string particle, string dependence)
    {
        ArgumentNullException.ThrowIfNull(particle);
        ArgumentNullException.ThrowIfNull(dependence);

        string first = LSentenceOrderLeading ? particle : dependence;
        string second = LSentenceOrderLeading ? dependence : particle;
        string frame = first.Length == 0 || second.Length == 0 ? first + second : first + " " + second;
        return frame.Length == 0 ? string.Empty : "(+" + frame + ")";
    }

    public string LSentenceOrderFormat(string particle, string dependence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string head = LSentenceOrderFormat(particle, dependence);
        return head.Length == 0 || text.Length == 0 ? head + text : head + " " + text;
    }
}
