using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSentenceFrame(
    CSentenceOrder CSentenceFrameOrder,
    IReadOnlyList<string> CSentenceFrameParticle,
    IReadOnlyList<string> CSentenceFrameDependence);
