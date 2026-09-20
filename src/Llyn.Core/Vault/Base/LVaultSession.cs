using System;

namespace Llyn.Core;

public interface LVaultSession : IDisposable
{
    void LVaultSessionCommit();
}
