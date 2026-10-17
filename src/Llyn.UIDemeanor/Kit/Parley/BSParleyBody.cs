using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal interface BSParleyBody
{
    int BSParleyBodyWidth { get; }

    int BSParleyBodyHeight { get; }

    string BSParleyBodyPosition { get; }

    bool BSParleyBodyObserve(ConsoleKeyInfo key);

    IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role);
}
