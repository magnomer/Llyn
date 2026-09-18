using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    public bool LTenureFlaggedCheck()
    {
        string language = LTenureLanguageRead();
        return language.Length > 0 && _lEngine.LEngineFlaggedCheck(language);
    }

    public IReadOnlyList<LVariety> LTenureVarietyRead()
    {
        string language = LTenureLanguageRead();
        return language.Length == 0 ? [] : _lEngine.LEngineVarietyRead(language);
    }
}
