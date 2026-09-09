using System;

namespace Llyn.Core;

public static class LPortraitFrame
{
    public static string LPortraitFrameRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark)
    {
        ArgumentNullException.ThrowIfNull(sentence);
        ArgumentNullException.ThrowIfNull(order);

        string particle = LPortraitText.LPortraitTextRead(sentence.LSentenceDraftParticle, mark);
        string dependence = LPortraitText.LPortraitTextRead(sentence.LSentenceDraftDependence, mark);

        string first = order.LSentenceOrderParticle == 0 ? particle : dependence;
        string second = order.LSentenceOrderParticle == 0 ? dependence : particle;
        string frame = first.Length == 0 || second.Length == 0
            ? first + second
            : first + " " + second;

        return frame.Length == 0 ? string.Empty : "(+" + frame + ")";
    }
}
