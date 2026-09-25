using System;

namespace Llyn.Core;

public interface LUsher
{
    bool LUsherPathExist(string? path);

    void LUsherPathDelete(string path);

    bool LUsherLockCheck(Exception exception);

    void LUsherOpen(string target);
}
