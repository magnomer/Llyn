using System;

namespace Llyn.UIDemeanor;

internal sealed record BInletRow(
    string BInletRowLead,
    string BInletRowMark,
    string BInletRowTail,
    string BInletRowCount,
    Action<bool, long> BInletRowChoice);
