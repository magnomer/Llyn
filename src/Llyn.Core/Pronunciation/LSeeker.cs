using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LSeeker
{
    Task LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation);
}
