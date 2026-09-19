using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LFanqieSource
{
    Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieSourceFind(
        LFanqieBook book, string character, CancellationToken cancellation);
}
