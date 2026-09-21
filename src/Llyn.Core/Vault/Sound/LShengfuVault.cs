using System.Collections.Generic;

namespace Llyn.Core;

public interface LShengfuVault
{
    void LShengfuSave(string language, LShengfu row);

    LShengfu? LShengfuRead(string language, string character);

    IReadOnlyList<LShengfu> LShengfuScan(string language, IReadOnlyList<string> characters);
}
