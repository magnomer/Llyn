using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTallyMark(string CTallyMarkText, int CTallyMarkCount, IReadOnlyList<string> CTallyMarkCharacters);
