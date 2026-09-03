using System.Globalization;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAnthologyItem
{
    internal PAnthologyItem(LExample example, int usage, string source, string unreadable, string unwritten)
    {
        PAnthologyItemId = example.LExampleId;
        PAnthologyItemText = PAnthologyTextRead(example.LExampleText, unreadable) ?? unwritten;
        PAnthologyItemLanguage = example.LExampleLanguage;
        PAnthologyItemFlag = PLangcodeIndicator.PLangcodeIndicatorFind(example.LExampleLanguage);
        PAnthologyItemSource = source;
        PAnthologyItemUsage = usage.ToString(CultureInfo.CurrentCulture);
    }

    public string PAnthologyItemId { get; }

    public string PAnthologyItemText { get; }

    public string PAnthologyItemLanguage { get; }

    public ImageSource? PAnthologyItemFlag { get; }

    public string PAnthologyItemSource { get; }

    public string PAnthologyItemUsage { get; }

    private static string? PAnthologyTextRead(LStateValue value, string unreadable)
    {
        return value.LStateValueState switch
        {
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => null,
        };
    }
}
