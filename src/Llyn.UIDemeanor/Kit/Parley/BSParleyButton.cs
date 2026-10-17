using System;

namespace Llyn.UIDemeanor;

internal sealed class BSParleyButton
{
    internal BSParleyButton(string label, ConsoleKey? key = null, string? legend = null)
    {
        ArgumentNullException.ThrowIfNull(label);
        if (key is not null)
        {
            ArgumentNullException.ThrowIfNull(legend);
        }

        BSParleyButtonLabel = label;
        BSParleyButtonKey = key;
        BSParleyButtonLegend = legend;
    }

    internal string BSParleyButtonLabel { get; }

    internal bool BSParleyButtonEnabled { get; set; } = true;

    internal ConsoleKey? BSParleyButtonKey { get; }

    internal string? BSParleyButtonLegend { get; }
}
