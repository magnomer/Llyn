using System.Collections.Generic;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed record QContourItem(
    string QContourItemText,
    IReadOnlyList<int> QContourItemLevels,
    IReadOnlyList<Brush> QContourItemBrushes,
    bool QContourItemToned);
