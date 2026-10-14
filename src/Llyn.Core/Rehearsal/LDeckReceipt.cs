using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDeckReceipt(
    int LDeckReceiptCards,
    int LDeckReceiptCopied,
    int LDeckReceiptRemoved,
    IReadOnlyList<string> LDeckReceiptFailed);
