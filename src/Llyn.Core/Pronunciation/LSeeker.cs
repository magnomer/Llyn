using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LSeeker
{
    Task<IReadOnlyList<LCandidate>> LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation);
}
