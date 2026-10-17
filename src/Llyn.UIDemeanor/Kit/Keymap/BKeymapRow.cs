using System;

namespace Llyn.UIDemeanor;

internal sealed record BKeymapRow(
    ConsoleKey BKeymapRowKey,
    ConsoleModifiers BKeymapRowModifiers,
    BKeymapFocus BKeymapRowFocus,
    string BKeymapRowLabel,
    Action<ConsoleKeyInfo> BKeymapRowAction,
    Func<BKeymapLive> BKeymapRowLive);
