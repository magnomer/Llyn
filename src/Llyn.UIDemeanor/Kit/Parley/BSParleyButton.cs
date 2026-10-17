using System;

namespace Llyn.UIDemeanor;

internal sealed class BSParleyButton
{
    internal BSParleyButton(string label, ConsoleKey? key = null)
    {
        ArgumentNullException.ThrowIfNull(label);

        BSParleyButtonLabel = label;
        BSParleyButtonKey = key;
    }

    internal string BSParleyButtonLabel { get; }

    internal bool BSParleyButtonEnabled { get; set; } = true;

    internal ConsoleKey? BSParleyButtonKey { get; }
}
