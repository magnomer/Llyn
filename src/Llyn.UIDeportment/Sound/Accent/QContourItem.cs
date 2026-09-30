using System.Collections.Generic;

namespace Llyn.UIDeportment;

public sealed record QContourItem(
    string QContourItemText,
    IReadOnlyList<int> QContourItemLevels,
    bool QContourItemToned);
