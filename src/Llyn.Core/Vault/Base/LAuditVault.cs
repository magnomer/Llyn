using System;

namespace Llyn.Core;

public interface LAuditVault
{
    string? LAuditRecord(Exception exception);
}
