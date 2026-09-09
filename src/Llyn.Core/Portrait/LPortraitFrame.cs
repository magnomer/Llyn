using System;

namespace Llyn.Core;

public static class LPortraitFrame
{
    public static string LPortraitFrameRead(
        LExampleDraft example, LSentenceOrder order, string mark)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(order);

        string particle = LPortraitText.LPortraitTextRead(example.LExampleDraftParticle, mark);
        string dependence = LPortraitText.LPortraitTextRead(example.LExampleDraftDependence, mark);

        string first = order.LSentenceOrderParticle == 0 ? particle : dependence;
        string second = order.LSentenceOrderParticle == 0 ? dependence : particle;
        string frame = first.Length == 0 || second.Length == 0
            ? first + second
            : first + " " + second;

        return frame.Length == 0 ? string.Empty : "(+" + frame + ")";
    }
}
