using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    public bool LTenureFlaggedCheck()
    {
        string language = LTenureLanguageRead();
        return language.Length > 0 && _lEngine.LEngineLanguage.LEngineFlaggedCheck(language);
    }

    public IReadOnlyList<LVariety> LTenureVarietyRead()
    {
        string language = LTenureLanguageRead();
        return language.Length == 0 ? [] : _lEngine.LEngineLanguage.LEngineVarietyRead(language);
    }

    public IReadOnlyList<string> LTenureVarietyNames =>
        LTenureVarietyRead().Select(static variety => variety.LVarietyName).ToList();

    public bool LTenureReflexCheck()
    {
        return _lEngine.LEngineReflex.LEngineReflexRead(LTenureLanguageRead()).Count > 0
            || LTenureRead()?.LDraftContent.LEntryDraftReflected == true;
    }
}
