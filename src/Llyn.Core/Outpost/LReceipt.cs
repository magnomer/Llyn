using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReceipt(
    int LReceiptSaved,
    int LReceiptKept,
    int LReceiptRemoved,
    IReadOnlyList<string> LReceiptFailed);
