using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LSource
{
    string LSourceName { get; }

    Task<string?> LSourceFind(string word, CancellationToken cancellation);
}
