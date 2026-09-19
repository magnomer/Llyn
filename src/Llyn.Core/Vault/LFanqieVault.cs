using System.Collections.Generic;

namespace Llyn.Core;

public interface LFanqieVault
{
    void LFanqieSave(string language, string character, IReadOnlyList<LFanqieRow> rows);

    IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character);
}
