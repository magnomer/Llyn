using System.Collections.Generic;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LPressQueue
{
    Task<IReadOnlyList<LPressPrinter>> LPressQueueRead();
}
