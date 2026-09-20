using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LSource
{
    string LSourceName { get; }

    Task<LAnswer> LSourceFind(string word, CancellationToken cancellation);
}
