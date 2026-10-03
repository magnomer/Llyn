using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TInterfaceConductDialog
{
    internal static CSCustoms TCustomsCreate(
        IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets) => new(entries, targets);

    internal static IReadOnlyList<CSCustomsRow> TCustomsRowsRead(this CSCustoms customs) =>
        customs.LSCustomsRowsRead();

    internal static CSCustomsRow TCustomsRowRead(this CSCustoms customs, int row) => customs.CSCustomsRowRead(row);

    internal static bool TCustomsModeSet(this CSCustoms customs, int row, CSCustomsMode mode) =>
        customs.CSCustomsModeSet(row, mode);

    internal static bool TCustomsTargetSet(this CSCustoms customs, int row, long target) =>
        customs.CSCustomsTargetSet(row, target);

    internal static bool TCustomsReadyCheck(this CSCustoms customs) => customs.CSCustomsReadyCheck();

    internal static bool TCoinageWordingCheck(string? wording) => CSCoinage.CSCoinageWordingCheck(wording);
}
