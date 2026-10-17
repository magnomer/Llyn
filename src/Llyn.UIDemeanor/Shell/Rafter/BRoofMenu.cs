using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed record BRoofMenu(
    string BRoofMenuTitle,
    ConsoleKey? BRoofMenuHotkey,
    IReadOnlyList<BRoofItem?> BRoofMenuItems);
