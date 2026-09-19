using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LScriptSource
{
    Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptSourceFind(
        LScriptStyle style, string character, CancellationToken cancellation);
}
