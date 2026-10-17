namespace Llyn.UIDemeanor;

internal readonly record struct BCanvasText(
    int BCanvasTextRow, int BCanvasTextColumn, int BCanvasTextSpan, string BCanvasTextValue, string BCanvasTextRole);
