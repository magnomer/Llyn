using System.Collections.Generic;

namespace Llyn.Core;

public interface LSourceFactory
{
    IReadOnlyList<LSource> LSourceFactoryCreate(IReadOnlyList<LSourceSpec> specs);
}
