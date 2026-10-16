using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSittingSide(string CSittingSideText, IReadOnlyList<CImageDraft> CSittingSidePictures)
{
    internal string? CSittingSideSound { get; init; }

    public bool CSittingSideAudible => CSittingSideSound is not null;
}
