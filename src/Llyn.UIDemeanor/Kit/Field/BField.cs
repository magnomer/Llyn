using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.UIDemeanor;

internal sealed class BField : BSParleyBody
{
    private readonly Action<string> _bFieldEdited;

    internal BField(Action<string> edited)
    {
        ArgumentNullException.ThrowIfNull(edited);

        _bFieldEdited = edited;
    }

    public int BSParleyBodyWidth => BSParley.BSParleyWidth;

    public int BSParleyBodyHeight => 1;

    public string BSParleyBodyPosition => string.Empty;

    internal string BFieldText { get; private set; } = string.Empty;

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        string text;
        if (key.Key == ConsoleKey.Backspace)
        {
            List<int> starts = BFieldStartRead(BFieldText);
            text = starts.Count == 0 ? BFieldText : BFieldText[..starts[^1]];
        }
        else if (char.IsControl(key.KeyChar))
        {
            return false;
        }
        else
        {
            text = BFieldText + key.KeyChar;
        }

        BFieldText = text;
        _bFieldEdited(text);
        return true;
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        if (width <= 0)
        {
            yield break;
        }

        List<int> starts = BFieldStartRead(BFieldText);
        int from = BFieldText.Length;
        int cells = 0;
        for (int index = starts.Count - 1; index >= 0; index--)
        {
            int size = measure.BCanvasWidthResolve(BFieldText.AsSpan(starts[index], from - starts[index]));
            if (cells + size > width - 1)
            {
                break;
            }

            cells += size;
            from = starts[index];
        }

        yield return new BCanvasText(
            row, column, width, string.Concat(BFieldText[from..], new string(' ', width - cells)), "field");
    }

    private static List<int> BFieldStartRead(string text)
    {
        List<int> starts = [];
        for (int index = 0; index < text.Length; index += StringInfo.GetNextTextElementLength(text, index))
        {
            starts.Add(index);
        }

        return starts;
    }
}
