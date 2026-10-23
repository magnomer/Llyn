using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed record BRoofItem(
    string BRoofItemLabel,
    ConsoleKey? BRoofItemKey,
    ConsoleKey? BRoofItemHotkey,
    Action BRoofItemAction,
    Func<bool> BRoofItemLive,
    IReadOnlyList<BRoofItem?>? BRoofItemSubmenu = null);
