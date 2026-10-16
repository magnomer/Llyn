using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSittingSide(string CSittingSideText, IReadOnlyList<CImageDraft> CSittingSidePictures)
{
    internal IReadOnlyList<string> CSittingSideSounds { get; init; } = [];

    public bool CSittingSideAudible => CSittingSideSounds.Count > 0;
}
