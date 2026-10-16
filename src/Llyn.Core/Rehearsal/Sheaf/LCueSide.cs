using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LCueSide(
    string LCueSideText, IReadOnlyList<string> LCueSidePictures, IReadOnlyList<string> LCueSideSounds);
