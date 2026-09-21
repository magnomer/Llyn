using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LShengfuSource
{
    Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuSourceFind(
        LShengfuRule rule, string character, CancellationToken cancellation);
}
