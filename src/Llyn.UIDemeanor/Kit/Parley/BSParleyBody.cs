using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal interface BSParleyBody
{
    int BSParleyBodyWidth { get; }

    int BSParleyBodyHeight { get; }

    string BSParleyBodyPosition { get; }

    BKeymap? BSParleyBodyKeymap => null;

    bool BSParleyBodyObserve(ConsoleKeyInfo key);

    void BSParleyBodyResume(bool backward)
    {
    }

    IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        int height,
        bool focused,
        string role,
        BCanvasWidth measure);
}
