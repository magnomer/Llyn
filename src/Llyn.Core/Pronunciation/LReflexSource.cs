using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LReflexSource
{
    Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexSourceFind(
        LReflexRule rule, string character, CancellationToken cancellation);
}
