using System.Collections.Generic;

namespace Llyn.Core;

public interface LEtymologyVault
{
    LEtymology? LEtymologyRead(long entryId);

    IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId);

    LEtymology? LEtymologySave(long entryId, LEtymology? etymology);

    IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds);
}
